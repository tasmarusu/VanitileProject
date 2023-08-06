using System;
using System.Collections;
using UniRx;
using UnityEngine;

namespace VANITILE
{
    /// <summary>
    /// プレイヤーの移動系
    /// ◆MovementState と PlayerData しか PlayerController は呼ばない◆
    /// TODO:判定系と処理を分けるべきかも知れない 
    /// </summary>
    public partial class PlayerMove : MonoBehaviour
    {
        /// <summary>
        /// プレイヤーオブジェクト
        /// </summary>
        [SerializeField, Header("Rigitbody")] private Rigidbody2D rig2D;

        /// <summary>
        /// ContactFilter2D
        /// </summary>
        [SerializeField, Header("ContactFilter2D")] private ContactFilter2D filter2D;

        /// <summary>
        /// PlayerDataScriptable
        /// </summary>
        [SerializeField, Header("PlayerDataScriptable")] private PlayerDataScriptable playerData;

        /// <summary>
        /// 壁滑り中の落下か
        /// </summary>
        private float wallDownValue = .0f;

        /// <summary>
        /// 右向きか
        /// これ PlayerAnimation に移動するべきか…
        /// </summary>
        public bool IsRightAngle { get; private set; }

        /// <summary>
        /// 開始時のプレイヤーの重力
        /// </summary>
        public float GravityStartSpeed { get; private set; }

        /// <summary>
        /// 壁ジャンプ挙動
        /// </summary>
        public Coroutine WallFallCoroutine { get; private set; } = null;

        /// <summary>
        /// 壁ジャンプの操作受付無効状態
        /// </summary>
        public bool IsWallJumping { get; set; } = true;

        /// <summary>
        /// 壁ジャンプした時からのフレーム
        /// </summary>
        public int WallJumpingProgressFrame { get; set; } = 0;

        /// <summary>
        /// 壁ジャンプ力
        /// </summary>
        public float WallJumpPower { get; set; } = .0f;

        /// <summary>
        /// 壁ジャンプの購読用Disposable
        /// </summary>
        public IDisposable WallJumpDisposable { get; set; }

        /// <summary>
        /// 上ジャンプ可能か
        /// </summary>
        public bool IsUpJump => this.IsTouchGround == true && Mathf.Abs(this.rig2D.velocity.y) <= .1f;

        /// <summary>
        /// 壁ジャンプ可能か
        /// </summary>
        public bool IsWallJump => this.Movement == MovementState.Wall;

        /// <summary>
        /// 動作挙動
        /// </summary>
        public MovementState Movement { get; private set; } = MovementState.Wait;

        /// <summary>
        /// 動作挙動の変更監視
        /// </summary>
        public Subject<MovementState> ChangeMovementStateSubject = new Subject<MovementState>();


        /// <summary>
        /// プレイヤーの挙動状態
        /// </summary>
        public enum MovementState
        {
            /// <summary>
            /// 待機
            /// </summary>
            Wait,

            /// <summary>
            /// 移動
            /// </summary>
            Move,

            /// <summary>
            /// 壁接触中
            /// </summary>
            Wall,

            /// <summary>
            /// 空中
            /// </summary>
            Air,
        }

        /// <summary>
        /// デバッグのみ。後で消す
        /// </summary>
        public float Speed;

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
            this.GravityStartSpeed = this.rig2D.gravityScale;
            this.StartCheckWallDetection();
            this.StartCheckGroundDetection();
            this.StartWallSticking();
        }

        /// <summary>
        /// Update
        /// </summary>
        void Update()
        {
            // 入力方向によって回転させる
            this.Rotate(InputManager.Instance.Horizontal);

            // 壁張り付き時間
            this.WallJumpingProgressFrame++;
        }

        /// <summary>
        /// 挙動変更
        /// </summary>
        /// <param name="movement">変更後挙動</param>
        public void SetMovementState(MovementState movement)
        {
#if UNITY_EDITOR
            Debug.Log($"[MovementState]変更 {movement} {Time.frameCount}");
#endif
            this.Movement = movement;
            this.ChangeMovementStateSubject.OnNext(this.Movement);
        }

        /// <summary>
        /// 壁接触時にパラメーターのリセット
        /// </summary>
        public void ResetParamOnContactWall()
        {
            Debug.Log($"壁リセット  {Time.frameCount}");
            this.WallJumpPower = .0f;
            this.IsWallJumping = true;
            this.rig2D.gravityScale = this.GravityStartSpeed;
            this.rig2D.velocity = Vector2.zero;
            this.WallJumpDisposable?.Dispose();

            if (this.WallFallCoroutine != null)
            {
                this.StopCoroutine(this.WallFallCoroutine);
            }
        }

        /// <summary>
        /// 地面接触時にパラメーターのリセット
        /// </summary>
        public void ResetParamOnContactGround()
        {
            Debug.Log($"地面リセット  {Time.frameCount}");
            this.WallJumpPower = .0f;
            this.IsWallJumping = true;
            this.rig2D.gravityScale = this.GravityStartSpeed;
            this.rig2D.velocity = Vector2.zero;
            this.WallJumpDisposable?.Dispose();

            if (this.WallFallCoroutine != null)
            {
                this.StopCoroutine(this.WallFallCoroutine);
            }
        }

        /// <summary>
        /// 横移動処理
        /// </summary>
        public void Horizontal()
        {
            // 入力と壁ジャンプ力を徐々に入力に割合として加算していく
            var input = InputManager.Instance.Horizontal * (1.0f - Mathf.Abs(this.WallJumpPower / playerData.JumpWallSidePower));

            // 壁ジャンプ開始中か
            if (this.IsWallJumping == false)
            {
                input = 0f;
            }

            // 横移動
            var vel = this.rig2D.velocity;
            float move = (input * playerData.MoveSpeed * Time.fixedDeltaTime) + this.WallJumpPower;
            this.Speed = WallJumpPower;
            this.rig2D.velocity = new Vector2(move, vel.y);

            // 重力制限
            if (vel.y >= playerData.GravityMaxSpeed)
            {
                this.rig2D.velocity = new Vector2(vel.x, playerData.GravityMaxSpeed);
            }
        }

        /// <summary>
        /// 入力方向でY軸回転させる
        /// 右0度 左180度
        /// </summary>
        /// <param name="value">Input</param>
        public void Rotate(float value)
        {
            // 壁ジャンプ中は終了
            if (!this.IsWallJumping)
            {
                return;
            }

            // Y軸回転
            var euler = this.transform.eulerAngles;
            var input = .1f;
            var angle = value > input ? .0f :   // 右向き
                        value < -input ? 180f : // 左向き
                        euler.y;                // 現在の向き
            this.transform.eulerAngles = new Vector3(euler.x, angle, euler.z);

            // 右向きか
            this.IsRightAngle = angle == .0f;
        }

        /// <summary>
        /// 上ジャンプ
        /// </summary>
        public bool UpJump()
        {
            // ジャンプ入力
            if (InputManager.Instance.Jump == false)
            {
                return false;
            }

#if UNITY_EDITOR
            Debug.Log($"[Player]IsUpJump:{this.IsUpJump}");
            Debug.Log($"[Player]IsTouchGround:{this.IsTouchGround}");
#endif
            // 上ジャンプ不可
            if (!this.IsUpJump)
            {
                return false;
            }

            // ジャンプ開始
            SoundManager.Instance.PlaySe(DefineData.SeType.Jump);
            this.rig2D.AddForce(this.transform.up * playerData.JumpUpPower);
            this.SetMovementState(MovementState.Air);
#if UNITY_EDITOR
            Debug.Log($"[ジャンプ]上");
#endif

            return true;
        }

        /// <summary>
        /// 壁ジャンプ
        /// </summary>
        public bool WallJump()
        {
            // ジャンプ入力
            if (InputManager.Instance.Jump == false)
            {
                return false;
            }

#if UNITY_EDITOR
            Debug.Log($"[Player]IsWallJump:{this.IsWallJump}");
#endif
            // 壁ジャンプ可能か
            if (!this.IsWallJump)
            {
                return false;
            }

            // 壁ジャンプSE
            SoundManager.Instance.PlaySe(DefineData.SeType.Jump);

            // 壁横ジャンプ 壁の方向とは逆に力を加える
            var rightValue = this.IsRightAngle ? -1.0f : 1.0f;
            this.WallJumpPower = playerData.JumpWallSidePower * rightValue;
            this.Rotate(rightValue);

            // 壁ジャンプ上
            this.rig2D.AddForce(this.transform.up * playerData.JumpWallUpPower);

            // 壁ジャンプ後、一定時間操作不可
            this.IsWallJumping = false;

            // 壁ジャンプ開始時の進行フレームのリセット
            this.WallJumpingProgressFrame = 0;

            // 重力を元に戻す
            this.rig2D.gravityScale = this.GravityStartSpeed;

            // 壁ジャンプ開始から速度が下がる
            this.StartDownWallJumpPower();

            // ステートを空中判定へ
            this.SetMovementState(MovementState.Air);

            return true;
        }

        /// <summary>
        /// 壁滑り
        /// MovementStateがWallの時
        /// </summary>
        public void SlipWall()
        {
            this.rig2D.gravityScale = .0f;
            this.rig2D.velocity = new(this.rig2D.velocity.x, -this.wallDownValue);
        }

        /// <summary>
        /// 壁ジャンプ開始から壁ジャンプ速度を下げる
        /// </summary>
        public void StartDownWallJumpPower()
        {
            var jumpOnlyTimer = .0f;    // 操作受付不可時間
            var wallDownTimer = .0f;    // 壁ジャンプ速度低下時間
            this.WallJumpDisposable = Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    if (this.Movement != MovementState.Air)
                    {
#if UNITY_EDITOR
                        Debug.Log($"[壁ジャンプ]空中にいなくなったので壁ジャンプ終了");
#endif
                        this.WallJumpDisposable.Dispose();
                    }

                    // 操作受付不可解除まで待機
                    jumpOnlyTimer += Time.deltaTime;
                    if (jumpOnlyTimer < playerData.JumpWallOnlyTime)
                    {
                        return;
                    }

                    // 操作解除して速度低下開始
                    this.IsWallJumping = true;

                    // 壁ジャンプ力を一定時間掛けて0にする
                    wallDownTimer += Time.deltaTime * playerData.JumpWallDownTime;
                    this.WallJumpPower = Mathf.Lerp(this.WallJumpPower, .0f, wallDownTimer);

                    // 終了
                    if (this.WallJumpPower == .0f)
                    {
#if UNITY_EDITOR
                        Debug.Log($"[壁ジャンプ]壁ジャンプ力が0になった");
#endif
                        this.WallJumpDisposable.Dispose();
                    }
                }).AddTo(this);
        }

        /// <summary>
        /// 壁滑り落下挙動開始
        /// </summary>
        /// <returns>IEnumerator</returns>
        public void StartWallFallCoroutine()
        {
#if UNITY_EDITOR
            Debug.Log($"[壁ジャンプ]壁滑り開始");
#endif
            this.WallFallCoroutine = this.StartCoroutine(this.StartWallFallCoroutineImpl());
        }

        /// <summary>
        /// 壁滑り落下挙動開始
        /// </summary>
        /// <returns>IEnumerator</returns>
        private IEnumerator StartWallFallCoroutineImpl()
        {
            // 重力無視で待機
            this.wallDownValue = .0f;

            // 一時停止
            yield return new WaitForSeconds(playerData.WallStickTimer);

            // 壁張り付き状態で一定速度落下 
            this.wallDownValue = playerData.WallStickVelocity;

            // 地面に着くかジャンプするまで待機
            yield return new WaitUntil(() => this.IsTouchGround);

            // 壁滑り重力
            this.wallDownValue = .0f;

            // ステートを待機に変更
            this.SetMovementState(MovementState.Wait);
        }
    }
}
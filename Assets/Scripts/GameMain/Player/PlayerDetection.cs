using UniRx;
using UnityEngine;

namespace VANITILE
{
    public partial class PlayerMove : MonoBehaviour
    {
        /// <summary>
        /// 壁判定
        /// </summary>
        [SerializeField, Header("壁判定")] private PlayerCollision wallCollision;

        /// <summary>
        /// 地面判定
        /// </summary>
        [SerializeField, Header("地面判定")] private PlayerCollision groundCollision;

        /// <summary>
        /// 地面接触判定
        /// </summary>
        public bool IsTouchGround { get; private set; } = false;

        /// <summary>
        /// 壁接触判定
        /// </summary>
        public bool IsTouchWall { get; private set; } = false;

        /// <summary>
        /// 壁張り付き判定開始
        /// </summary>
        private void StartWallSticking()
        {
            Observable.EveryLateUpdate()
                .Where(_ => this.Movement == MovementState.Air) // 空中
                .Where(_ => this.IsTouchWall)              // 壁接触中
                .Where(_ => this.WallJumpingProgressFrame >= 10)    // 壁ジャンプ中でない TODO:ここが原因で壁ジャンプ時に上に上がる処理が止まらない
                .Subscribe(deltaTime =>
                {
                    // ステートを壁判定へ
                    this.SetMovementState(MovementState.Wall);
                }).AddTo(this);
        }

        /// <summary>
        /// 壁チェック
        /// </summary>
        private void StartCheckWallDetection()
        {
            this.wallCollision.ObserveEveryValueChanged(x => x.IsTouch)
                .Subscribe(isTouch =>
                {
#if UNITY_EDITOR
                    Debug.Log($"[Wall]壁チェック {isTouch}");
#endif
                    this.IsTouchWall = isTouch;

                    // 壁滑り中に地面に着いているかでステートを変更
                    if (this.IsTouchWall == false && this.IsTouchGround == false && this.IsWallJumping == true && this.Movement == MovementState.Wall)
                    {
                        this.SetMovementState(MovementState.Air);
                        this.ResetParamOnContactGround();
                    }
                }).AddTo(this);
        }

        /// <summary>
        /// 地面判定チェック
        /// </summary>
        private void StartCheckGroundDetection()
        {
            this.groundCollision.ObserveEveryValueChanged(x => x.IsTouch)
                .Subscribe(isTouch =>
                {
#if UNITY_EDITOR
                    var str = isTouch ? $"<color=cyan>{isTouch}</color>" : $"< color = red >{isTouch}</ color >";
                    Debug.Log($"[Collision]地面チェック {str}");
#endif
                    // 地面判定の切り替え
                    this.IsTouchGround = isTouch;

                    // 自由落下時に空中ステートに変更
                    if (this.IsTouchGround == false && this.IsTouchWall == false)
                    {
                        this.SetMovementState(MovementState.Air);
                        return;
                    }

                    // 空中にいるので終了
                    if (this.IsTouchGround == false)
                    {
                        return;
                    }

                    // 地面に着いていたらステートを変える
                    var input = Input.GetAxis("Horizontal");
                    Debug.Log($"[input]{input}");
                    if ((this.IsRightAngle == true && input >= .5f) || (this.IsRightAngle == false && input <= -.5f))
                    {
                        this.SetMovementState(MovementState.Move);
                    }
                    else
                    {
                        this.SetMovementState(MovementState.Wait);
                    }
                }).AddTo(this);
        }
    }
}
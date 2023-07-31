using UniRx;
using UnityEngine;

namespace VANITILE
{
    /// <summary>
    /// プレイヤー操作
    /// </summary>
    public class PlayerController : CommonPart, IPart
    {
        /// <summary>
        /// PlayerMove
        /// </summary>
        [SerializeField, Header("PlayerMove")] private PlayerMove move;

        /// <summary>
        /// PlayerAnimator
        /// </summary>
        [SerializeField, Header("PlayerAnimator")] private PlayerAnimation animator;

        /// <summary>
        /// 操作可能
        /// </summary>
        public bool IsControll { get; set; } = false;

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            this.move.Init();
            this.animator.Init();
            this.CheckMovementState();
        }

        /// <summary>
        /// idの設定
        /// </summary>
        /// <param name="id"></param>
        public void SetId(int id)
        {
            this.Id = id;
        }

        /// <summary>
        /// ゴールした
        /// </summary>
        public void Goal()
        {
            GameObject.Destroy(this.gameObject);
        }

        /// <summary>
        /// 失敗
        /// </summary>
        public void Miss()
        {
            GameObject.Destroy(this.gameObject);
        }

        /// <summary>
        /// Update
        /// </summary>
        private void Update()
        {
            // 操作許可が無ければ終了
            if (this.IsControll == false)
            {
                return;
            }

            // 横移動
            this.move.Horizontal();

            switch (this.move.Movement)
            {
                case PlayerMove.MovementState.Wait:
                case PlayerMove.MovementState.Move:
                    if (this.move.UpJump())
                    {
                        this.animator.JumpOnTrigger();
                    }

                    break;

                case PlayerMove.MovementState.Air:
                    break;

                case PlayerMove.MovementState.Wall:
                    this.move.SlipWall();

                    if (this.move.WallJump())
                    {
                        this.animator.JumpOnTrigger();
                    }

                    break;

                default:
#if UNITY_EDITOR
                    Debug.LogError($"{this.GetType().Name}のUpdateで予期せぬ値が入ってます。Movement:{this.move.Movement}");
#endif
                    break;
            }

            this.animator.SetIsGround(this.move.IsTouchGround);
            this.animator.SetIsWallTouch(this.move.IsTouchWall);
        }

        /// <summary>
        /// MovementStateの監視
        /// 変化があった最初のフレームのみ入る
        /// </summary>
        private void CheckMovementState()
        {
            this.move.ChangeMovementStateSubject.Subscribe(state =>
            {
                switch (state)
                {
                    case PlayerMove.MovementState.Wall:
                        SoundManager.Instance.PlaySe(DefineData.SeType.Other);
                        this.move.ResetParamOnContactWall();
                        this.move.StartWallFallCoroutine();
                        break;

                    case PlayerMove.MovementState.Wait:
                    case PlayerMove.MovementState.Move:
                        this.move.ResetParamOnContactGround();
                        break;

                    case PlayerMove.MovementState.Air:
                        break;

                    default:
#if UNITY_EDITOR
                        Debug.LogError($"{this.GetType().Name}のUpdateで予期せぬ値が入ってます。Movement:{this.move.Movement}");
#endif
                        break;
                }
            }).AddTo(this);
        }
    }
}

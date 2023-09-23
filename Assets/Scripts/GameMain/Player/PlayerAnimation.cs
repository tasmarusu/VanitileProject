namespace VANITILE
{
    using UniRx;
    using UnityEngine;

    /// <summary>
    /// プレイヤーアニメーション
    /// </summary>
    public class PlayerAnimation : MonoBehaviour
    {
        /// <summary>
        /// Animator
        /// </summary>
        [SerializeField, Header("Animator")] private Animator animator = null;

        /// <summary>
        /// Rigidbody2D
        /// </summary>
        private Rigidbody2D rig2D = null;

        /// <summary>
        /// Animatorのパラメーター名
        /// 1 -20 float
        /// 21-40 bool
        /// 41-60 trigger
        /// Note：float とかが多くなれば配列でfor文回して入れる形に変更する
        /// </summary>
        private enum StateName
        {
            /// <summary>
            /// 歩行
            /// </summary>
            Walk = 1,

            /// <summary>
            /// 重力
            /// </summary>
            Gravity = 10,

            /// <summary>
            /// 壁タッチ
            /// </summary>
            TouchWall = 21,

            /// <summary>
            /// 地面判定
            /// </summary>
            IsGround = 22,

            /// <summary>
            /// ジャンプ
            /// </summary>
            Jump = 41,
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init(Rigidbody2D rig2D)
        {
            Debug.Assert(this.animator != null, $"[Player]Animatorがオブジェクトにアタッチされていません");

            this.SetInputSpeed();
            this.rig2D = rig2D;
        }

        /// <summary>
        /// 地面判定の設定
        /// </summary>
        /// <param name="isGround"></param>
        public void SetIsGround(bool isGround)
        {
            this.animator.SetBool(StateName.IsGround.ToString(), isGround);
        }

        /// <summary>
        /// 壁判定の設定
        /// </summary>
        /// <param name="isGround"></param>
        public void SetIsWallTouch(bool isGround)
        {
            this.animator.SetBool(StateName.TouchWall.ToString(), isGround);
        }

        /// <summary>
        /// ジャンプ
        /// </summary>
        public void JumpOnTrigger()
        {
            this.animator.SetTrigger(StateName.Jump.ToString());
        }

        /// <summary>
        /// プレイヤーの状態によってアニメーターを変化させる
        /// </summary>
        private void SetInputSpeed()
        {
            Observable.EveryUpdate().Subscribe(_ =>
            {
                // 横移動
                var input = Mathf.Abs(InputManager.Instance.Horizontal);
                this.animator.SetFloat(StateName.Walk.ToString(), input);

                // 重力方向
                this.animator.SetFloat(StateName.Gravity.ToString(), this.rig2D.velocity.y);

            }).AddTo(this);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


namespace VANITILE
{
    /// <summary>
    /// プレイヤーアニメーション
    /// </summary>
    public class PlayerAnimation : MonoBehaviour
    {
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
        /// Animator
        /// </summary>
        [SerializeField, Header("Animator")] private Animator animator = null;

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
            Debug.Assert(this.animator != null, $"[Player]Animatorがオブジェクトにアタッチされていません");

            this.SetInputSpeed();
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
                var input = Mathf.Abs(Input.GetAxis("Horizontal"));
                this.animator.SetFloat(StateName.Walk.ToString(),input);
            }).AddTo(this);
        }
    }
}
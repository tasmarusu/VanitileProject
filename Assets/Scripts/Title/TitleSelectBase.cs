using System.Collections;
using UnityEngine;

namespace VANITILE
{
    /// <summary>
    /// タイトルセレクトのベース
    /// オプションとステージセレクトで使用予定
    /// </summary>
    public abstract class TitleSelectBase : MonoBehaviour
    {
        /// <summary>
        /// Animator
        /// </summary>
        [SerializeField] private Animator animator = null;

        /// <summary>
        /// 初期化
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// 終了処理
        /// </summary>
        public abstract IEnumerator Finalize();

        /// <summary>
        /// 表示アニメーション再生
        /// </summary>
        /// <returns></returns>
        protected IEnumerator In()
        {
            this.animator.Play("In", 0, .0f);
            yield return null;
            yield return new WaitUntil(() => this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        }

        /// <summary>
        /// 表示アニメーション再生
        /// </summary>
        /// <returns></returns>
        protected IEnumerator Out()
        {
            this.animator.Play("Out", 0, .0f);
            yield return null;
            yield return new WaitUntil(() => this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        }
    }
}

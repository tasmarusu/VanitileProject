using System.Collections;
using UnityEngine;
using DG.Tweening;

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
        [SerializeField] private Transform root = null;

        /// <summary>
        /// Animation時間
        /// </summary>
        [SerializeField] private float animationTime = .25f;

        /// <summary>
        /// 初期化
        /// </summary>
        public abstract IEnumerator Init();

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
            //this.animator.Play("TitleSelectOpenAnimation", 0, .0f);
            //yield return null;
            //yield return new WaitUntil(() => this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

            this.root.localScale = Vector3.zero;
            this.root.DOScale(Vector3.one, this.animationTime).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(this.animationTime);
        }

        /// <summary>
        /// 表示アニメーション再生
        /// </summary>
        /// <returns></returns>
        protected IEnumerator Out()
        {
            //this.animator.Play("TitleSelectCloseAnimation", 0, .0f);
            //yield return null;
            //yield return new WaitUntil(() => this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

            this.root.DOScale(Vector3.zero, this.animationTime).SetEase(Ease.InBack);
            yield return new WaitForSeconds(this.animationTime);
        }
    }
}

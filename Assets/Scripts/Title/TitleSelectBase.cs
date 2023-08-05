using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace VANITILE
{
    /// <summary>
    /// タイトルセレクトのベース
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
        /// 進行度
        /// </summary>
        protected abstract void InputController();

        /// <summary>
        /// 終了処理
        /// </summary>
        protected virtual IEnumerator Finalize()
        {
            yield return this.Out();
        }

        /// <summary>
        /// 表示アニメーション再生
        /// </summary>
        /// <returns></returns>
        protected IEnumerator In()
        {
            this.animator.Play("In" , 0, .0f);
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

using System.Collections;
using UnityEngine;
using DG.Tweening;
using UniRx;

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
        /// CompositeDisposable
        /// </summary>
        protected CompositeDisposable controllDisposables { get; } = new CompositeDisposable();

        /// <summary>
        /// 初期化
        /// </summary>
        public abstract IEnumerator Init();

        /// <summary>
        /// 終了処理
        /// 基本生成後に呼んで終了待機を行う
        /// </summary>
        public abstract IEnumerator Finalize();

        /// <summary>
        /// 決定ボタン押下
        /// </summary>
        protected abstract void OnDecideButton();

        /// <summary>
        /// 戻るボタン押下
        /// </summary>
        protected abstract void OnBackButton();

        /// <summary>
        /// 表示アニメーション再生
        /// </summary>
        /// <returns> IEnumerator </returns>
        protected IEnumerator In()
        {
            this.root.localScale = Vector3.zero;
            this.root.DOScale(Vector3.one, this.animationTime).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(this.animationTime);
        }

        /// <summary>
        /// 閉じるアニメーション再生後、各々が削除する。
        /// </summary>
        /// <returns> IEnumerator </returns>
        protected IEnumerator Out()
        {
            //this.animator.Play("TitleSelectCloseAnimation", 0, .0f);
            //yield return null;
            //yield return new WaitUntil(() => this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

            // 操作の終了
            this.controllDisposables.Clear();

            // 閉じるアニメーション開始
            this.root.DOScale(Vector3.zero, this.animationTime).SetEase(Ease.InBack);
            yield return new WaitForSeconds(this.animationTime);
        }

        /// <summary>
        /// ボタン系の操作の開始
        /// </summary>
        protected void StartButtonController()
        {
            // 決定ボタン押下
            InputManager.Instance.ObserveEveryValueChanged(x => x.Decide)
                .Where(x => x)
                .Subscribe(_ => this.OnDecideButton())
                .AddTo(this.controllDisposables);

            // 戻るボタン押下
            InputManager.Instance.ObserveEveryValueChanged(x => x.Back)
                .Where(x => x)
                .Subscribe(_ => this.OnBackButton())
                .AddTo(this.controllDisposables);
        }

        /// <summary>
        /// 破棄時に呼ばれる
        /// </summary>
        private void OnDestroy()
        {
            this.controllDisposables?.Clear();
        }
    }
}

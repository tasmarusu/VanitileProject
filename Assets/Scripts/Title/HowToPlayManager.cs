using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


namespace VANITILE
{
    /// <summary>
    /// ステージセレクトマネージャー
    /// </summary>
    public class HowToPlayManager : TitleSelectBase
    {
        /// <summary>
        /// CompositeDisposable
        /// </summary>
        private CompositeDisposable disposables = new CompositeDisposable();

        /// <summary>
        /// 初期化
        /// </summary>
        /// <returns></returns>
        public override IEnumerator Init()
        {
            TitleDataModel.Instance.PlayingState = DefineData.TitlePlayingState.HowToPlay;

            yield return this.In();

            // 決定ボタン押下
            InputManager.Instance.ObserveEveryValueChanged(x => x.Decide)
                .Where(x => x)
                .Subscribe(_ =>
                {
                    TitleDataModel.Instance.PlayingState = DefineData.TitlePlayingState.TitleSelect;
                }).AddTo(this.disposables);

            // 戻るボタン押下
            InputManager.Instance.ObserveEveryValueChanged(x => x.Back)
                .Where(x => x)
                .Subscribe(_ =>
                {
                    TitleDataModel.Instance.PlayingState = DefineData.TitlePlayingState.TitleSelect;
                }).AddTo(this.disposables);
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        /// <returns>IEnumerator</returns>
        public override IEnumerator Finalize()
        {
            yield return new WaitUntil(() => TitleDataModel.Instance.IsTitleSelect);

            this.disposables.Clear();
            yield return this.Out();

            GameObject.Destroy(this.gameObject);
        }

    }
}

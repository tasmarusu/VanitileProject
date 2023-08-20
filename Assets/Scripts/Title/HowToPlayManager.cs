namespace VANITILE
{
    using System.Collections;
    using System.Collections.Generic;
    using UniRx;
    using UnityEngine;

    /// <summary>
    /// ステージセレクトマネージャー
    /// </summary>
    public class HowToPlayManager : TitleSelectBase
    {
        /// <summary>
        /// 初期化
        /// </summary>
        /// <returns></returns>
        public override IEnumerator Init()
        {
            TitleDataModel.Instance.PlayingState = DefineData.TitlePlayingState.HowToPlay;

            // 表示アニメーション再生
            yield return this.In();

            // 操作開始
            this.StartButtonController();
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        /// <returns>IEnumerator</returns>
        public override IEnumerator Finalize()
        {
            // 待機
            yield return new WaitUntil(() => TitleDataModel.Instance.IsTitleSelect);

            // 操作を終了して閉じる
            yield return this.Out();

            // 削除
            GameObject.Destroy(this.gameObject);
        }

        /// <summary>
        /// 決定ボタン押下
        /// </summary>
        protected override void OnDecideButton()
        {
            TitleDataModel.Instance.PlayingState = DefineData.TitlePlayingState.TitleSelect;
        }

        /// <summary>
        /// 戻るボタン押下
        /// </summary>
        protected override void OnBackButton()
        {
            TitleDataModel.Instance.PlayingState = DefineData.TitlePlayingState.TitleSelect;
        }
    }
}

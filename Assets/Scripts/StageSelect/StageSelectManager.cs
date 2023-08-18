using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static DefineData;

namespace VANITILE
{
    /// <summary>
    /// ステージセレクトマネージャー
    /// </summary>
    public class StageSelectManager : TitleSelectBase, IPointerClickHandler
    {
        /// <summary>
        /// 使用するステージ情報
        /// </summary>
        [SerializeField, Header("使用するステージ情報")] private StageTransitionScriptable stageTransitionData = null;

        /// <summary>
        /// 選択UI群
        /// </summary>
        [SerializeField, Header("選択UI群")] private List<StageNumberPart> stageNumberParts = null;

        /// <summary>
        /// 選択する中心番号
        /// </summary>
        [SerializeField] private int centerSelectNum = 3;

        /// <summary>
        /// 選択中の番号
        /// </summary>
        private int currentSelectNum = 0;

        /// <summary>
        /// CompositeDisposable
        /// </summary>
        private CompositeDisposable disposables = new CompositeDisposable();

        /// <summary>
        /// 初期化
        /// </summary>
        public override IEnumerator Init()
        {
            Debug.Log($"[StageSelectManager]Init");

            TitleDataModel.Instance.PlayingState = DefineData.TitlePlayingState.StageSelect;
            this.SelectPart(this.currentSelectNum);
            this.UpdateNumberPart();

            yield return this.In();
            this.InputController();
        }

        /// <summary>
        /// 事後処理
        /// </summary>
        /// <returns></returns>
        public override IEnumerator Finalize()
        {
            // ポップアップ消えるまで待機
            yield return new WaitUntil(() => TitleDataModel.Instance.IsTitleSelect);

            // 操作削除して閉じる
            this.disposables.Clear();
            yield return this.Out();

            // 完全削除
            GameObject.Destroy(this.gameObject);
        }

        ///// <summary>
        ///// マウスがUI上に来る
        ///// </summary>
        ///// <param name="eventData"></param>
        //public void OnPointerMove(PointerEventData eventData)
        //{
        //    var index = this.selectables.FindIndex(x => x.gameObject.GetHashCode() == eventData.pointerEnter.gameObject.GetHashCode());
        //    this.SelectPart(index);
        //}

        /// <summary>
        /// クリック
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            StageDataModel.Instance.CurrentStageId = GameSaveDataModel.Instance.PlayLastStageId;
            SceneManager.LoadScene(SceneName.GameMainScene.ToString());
        }

        /// <summary>
        /// ステージ番号の更新
        /// </summary>
        private void UpdateNumberPart()
        {
            for (int i = 0; i < this.stageNumberParts.Count; i++)
            {
                var str = "";
                var num = this.currentSelectNum - this.centerSelectNum + i;

                if (num >= 0 && num < this.stageTransitionData.StageDatas.Count)
                {
                    // 0 始めなので 1 足す
                    str = (num + 1).ToString();
                }

                this.stageNumberParts[i].UpdateStageText(str);
            }
        }

        /// <summary>
        /// 操作開始
        /// </summary>
        /// <returns></returns>
        private void InputController()
        {
            // 決定ボタン押下
            InputManager.Instance.ObserveEveryValueChanged(x => x.Decide)
                .Where(x => x)
                .Where(_ => TitleDataModel.Instance.IsStageSelect)
                .Subscribe(_ =>
                {
                    TitleDataModel.Instance.PlayingState = DefineData.TitlePlayingState.TitleSelect;

                    //StageDataModel.Instance.CurrentStageId = this.currentSelectNum;
                    //SceneManager.LoadScene(SceneName.GameMainScene.ToString());
                }).AddTo(this.disposables);

            // 上下移動
            InputManager.Instance.VerticalOneSubject
                .Subscribe(value =>
                {
                    this.currentSelectNum -= value;
                    this.SelectPart(this.currentSelectNum);
                    this.UpdateNumberPart();

                }).AddTo(this.disposables);
        }

        /// <summary>
        /// 選択
        /// </summary>
        private void SelectPart(int num)
        {
            this.currentSelectNum = Mathf.Clamp(num, 0, this.stageTransitionData.StageDatas.Count - 1);
        }

        /// <summary>
        /// ステージ番号の移動
        /// </summary>
        private void MovePart()
        {

        }
    }
}
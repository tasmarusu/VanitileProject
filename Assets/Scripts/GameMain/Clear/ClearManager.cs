using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static VANITILE.StageDataModel;

namespace VANITILE
{
    /// <summary>
    /// ゲームクリア時に出るUI
    /// </summary>
    public class ClearManager : TitleSelectBase
    {
        /// <summary>
        /// 選択UI群
        /// </summary>
        [SerializeField, Header("選択UI群")] private List<ButtonListClass> selectables = new List<ButtonListClass>();

        /// <summary>
        /// スコアランク
        /// </summary>
        [SerializeField, Header("スコアランク")] private TextMeshProUGUI scoreRankText = null;

        /// <summary>
        /// NewRecordテキスト
        /// </summary>
        [SerializeField, Header("NewRecordテキスト")] private TextMeshProUGUI newRecordText = null;

        /// <summary>
        /// タイム
        /// </summary>
        [SerializeField, Header("タイム")] private TextMeshProUGUI timeText = null;

        /// <summary>
        /// 選択中の番号
        /// </summary>
        private int currentSelectNum = 0;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <returns> IEnumerator </returns>
        public override IEnumerator Init()
        {
            // 一個目を選択中
            this.selectables[this.currentSelectNum].MyButton.Select();

            // 時間をテキストに反映
            this.scoreRankText.text = $"A_";
            this.timeText.text = TimeManager.GetClearTimeStr(StageDataModel.Instance.TimeMana.processTimer);
            this.newRecordText.gameObject.SetActive(StageDataModel.Instance.TimeMana.IsRewriteBestTime());

            // in アニメーション
            yield return this.In();

            // 操作開始
            this.StartButtonController();
            this.InputController();
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        /// <returns> IEnumerator </returns>
        public override IEnumerator Finalize()
        {
            // 遷移ステートに入るまで待機
            yield return new WaitUntil(() => StageDataModel.Instance.CurrentGameState == MainGameState.Transition);

            // 操作を先に終了
            this.controllDisposables?.Clear();

            // 遷移から切り替わったら削除
            yield return new WaitUntil(() => StageDataModel.Instance.CurrentGameState != MainGameState.Transition);
            GameObject.Destroy(this.gameObject);
        }

        /// <summary>
        /// 戻るボタン
        /// </summary>
        protected override void OnBackButton()
        {
            Debug.Log($"[Clear]戻るボタンの機能無し");
        }

        /// <summary>
        /// 決定ボタン
        /// </summary>
        protected override void OnDecideButton()
        {
            // クリアステートの終了
            StageDataModel.Instance.SetEndClearState();

            // ボタン種類から次の状態へ
            switch (this.selectables[this.currentSelectNum].Type)
            {
                // 次のステージへ
                case ButtonType.NextStage:
                    GameMain.Instance.GameMainTrans.TransitionNextStage();
                    break;

                // リスタート
                case ButtonType.Restart:
                    GameMain.Instance.GameMainTrans.TransitionCurrentStage();
                    break;

                // タイトルへ
                case ButtonType.ToTitle:
                    SceneManager.LoadScene(DefineData.SceneName.TitleScene.ToString());
                    break;
            }
        }

        /// <summary>
        /// 操作開始
        /// </summary>
        private void InputController()
        {
            this.controllDisposables.Add(InputManager.Instance.StartVerticalSubject());

            InputManager.Instance.VerticalOneSubject
                .TakeUntilDestroy(this)
                .Subscribe(value =>
                {
                    this.currentSelectNum -= value;
                    this.SelectPart(this.currentSelectNum);
                }).AddTo(this.controllDisposables);
        }

        /// <summary>
        /// 選択
        /// </summary>
        private void SelectPart(int num)
        {
            this.currentSelectNum = (int)Mathf.Repeat(num, this.selectables.Count);
            this.selectables[this.currentSelectNum].MyButton.Select();
        }

        /// <summary>
        /// 削除
        /// </summary>
        private void OnDestroy()
        {
            this.controllDisposables?.Clear();
        }

        /// <summary>
        /// ボタンの設定と種類
        /// </summary>
        [System.Serializable]
        public class ButtonListClass
        {
            /// <summary>
            /// ボタン
            /// </summary>
            [field: SerializeField] public Button MyButton { get; private set; }

            /// <summary>
            /// ボタンの種類
            /// </summary>
            [field: SerializeField] public ButtonType Type { get; private set; }
        }

        /// <summary>
        /// ボタンの種類
        /// </summary>
        public enum ButtonType
        {
            /// <summary>
            /// 次のステージへ
            /// </summary>
            NextStage,

            /// <summary>
            /// リスタート
            /// </summary>
            Restart,

            /// <summary>
            /// タイトルシーンへ
            /// </summary>
            ToTitle,
        }
    }
}
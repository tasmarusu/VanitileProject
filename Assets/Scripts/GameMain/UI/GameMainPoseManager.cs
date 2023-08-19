using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using static VANITILE.StageDataModel;

namespace VANITILE
{
    /// <summary>
    /// タイトルで使用するオプション画面 
    /// </summary>
    public class GameMainPoseManager : TitleSelectBase
    {
        /// <summary>
        /// 選択UI群
        /// </summary>
        [SerializeField] private List<ButtonListClass> selectables = new List<ButtonListClass>();

        /// <summary>
        /// ハイスコアテキスト
        /// </summary>
        [SerializeField] private TextMeshProUGUI hiScoreText = null;

        /// <summary>
        /// ステージ番号
        /// </summary>
        [SerializeField] private TextMeshProUGUI stageText = null;

        /// <summary>
        /// 選択中の番号
        /// </summary>
        private int currentSelectNum = 0;

        /// <summary>
        /// ポーズ終了
        /// </summary>
        private bool IsPoseEnd = false;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <returns> IEnumerator </returns>
        public override IEnumerator Init()
        {
            this.IsPoseEnd = false;
            StageDataModel.Instance.StartPoseMode();

            // 一個目を選択中
            this.selectables[this.currentSelectNum].MyButton.Select();
            this.stageText.text = $"Stage{StageDataModel.Instance.CurrentStageId + 1}";

            // 開始
            yield return this.In();

            // 操作開始
            this.StartButtonController();
            this.InputController();
        }

        /// <summary>
        /// 終了
        /// </summary>
        /// <returns> IEnumerator </returns>
        public override IEnumerator Finalize()
        {
            yield return new WaitUntil(() => this.IsPoseEnd || StageDataModel.Instance.IsGamePlaying() == false);
            yield return this.Out();

            StageDataModel.Instance.EndPoseMode();

            // 削除
            GameObject.Destroy(this.gameObject);
        }

        /// <summary>
        /// 戻るボタン
        /// </summary>
        protected override void OnBackButton()
        {
            this.IsPoseEnd = true;
        }

        /// <summary>
        /// 決定ボタン
        /// </summary>
        protected override void OnDecideButton()
        {
            // ボタン種類から次の状態へ
            switch (this.selectables[this.currentSelectNum].Type)
            {
                // 再開
                case ButtonType.Resume:
                    this.IsPoseEnd = true;
                    break;

                // リスタート
                case ButtonType.Restart:
                    GameObject.Destroy(this.gameObject);
                    GameMain.Instance.GameMainTrans.TransitionCurrentStage();
                    break;

                // タイトルへ
                case ButtonType.ToTitle:
                    GameObject.Destroy(this.gameObject);
                    SceneManager.LoadScene(DefineData.SceneName.TitleScene.ToString());
                    break;

                // ゲーム終了
                case ButtonType.GameEnd:
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#elif !UNITY_EDITOR
                        UnityEngine.Application.Quit();
#endif
                    break;
            }
        }

        /// <summary>
        /// 操作開始
        /// </summary>
        /// <returns></returns>
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
            /// 再開
            /// </summary>
            Resume,

            /// <summary>
            /// リスタート
            /// </summary>
            Restart,

            /// <summary>
            /// タイトルシーンへ
            /// </summary>
            ToTitle,
            
            /// <summary>
            /// ゲーム終了
            /// </summary>
            GameEnd,
        }
    }
}
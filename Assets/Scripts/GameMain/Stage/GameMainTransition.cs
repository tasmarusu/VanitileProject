namespace VANITILE
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// ゲームメインの遷移
    /// </summary>
    public class GameMainTransition : MonoBehaviour
    {
        /// <summary>
        /// 次のステージへ遷移
        /// </summary>
        public void TransitionNextStage()
        {
            this.StartCoroutine(this.TransitionNextStageImpl());
        }

        /// <summary>
        /// 現在のステージを再リロード
        /// </summary>
        public void TransitionCurrentStage()
        {
            this.StartCoroutine(this.TransitionNextStageImpl(false));
        }

        /// <summary>
        /// ステージ遷移
        /// </summary>
        public void Transition()
        {
            // ステージ遷移
            var stageIds = GameMain.Instance.StageTransitionData.StageDatas;
            var changeStage = new ChangeStage(this);
            changeStage.Transition(stageIds[StageDataModel.Instance.CurrentStageId]);
            GameMain.Instance.CurrentStageData = changeStage.CurrentStageData;

            // 終了直後に State の変更を行う
            // TODO:初期化後に配置するかも
            StageDataModel.Instance.StartGame();

            // 各モデルの初期化
            GameMain.Instance.StageManagerInit();
        }

        /// <summary>
        /// 次のステージへ遷移
        /// </summary>
        /// <param name="isNext"> 同ステージを再開するなら false </param>
        /// <returns>IEnumerator</returns>
        private IEnumerator TransitionNextStageImpl(bool isNext = true)
        {
            Debug.Log($"[Stage]遷移開始");
            var transition = GameObject.Instantiate(Resources.Load<Transition>("Prefabs/Common/Transition"));
            yield return transition.In();

#if UNITY_EDITOR
            // GameDesignScene なら終了する
            if (SceneManager.GetActiveScene().name == "StageDesignScene")
            {
                UnityEditor.EditorApplication.isPlaying = false;
                Debug.Log($"[Stage]クリアしたので強制終了");
                yield break;
            }
#endif

            if (isNext)
            {
                this.NextStageId();
                this.Transition();
            }
            else
            {
                this.Transition();
            }

            yield return transition.Out();
            GameObject.Destroy(transition.gameObject);

            // Main開始
            Debug.Log($"[Stage]遷移終了");
        }

        /// <summary>
        /// 次のステージIDの設定
        /// </summary>
        private void NextStageId()
        {
            // クリアステージ数を保存
            GameSaveDataModel.Instance.CurrentClearStageNum = StageDataModel.Instance.CurrentStageId;

            // 現在のステージで終了か
            if (GameMain.Instance.StageTransitionData.StageDatas[StageDataModel.Instance.CurrentStageId].IsEndStage())
            {
                Debug.Log($"[Stage]設定されているステージを全てクリア");
                SceneManager.LoadScene(DefineData.SceneName.TitleScene.ToString());
            }
            else
            {
                // 次のステージへ
                StageDataModel.Instance.CurrentStageId++;

                // 最新のプレイステージIdを保存
                GameSaveDataModel.Instance.PlayLastStageId = StageDataModel.Instance.CurrentStageId;

                Debug.Log($"[Stage]arrayId:{StageDataModel.Instance.CurrentStageId} :{GameMain.Instance.StageTransitionData.StageDatas.Count}");
            }
        }
    }
}
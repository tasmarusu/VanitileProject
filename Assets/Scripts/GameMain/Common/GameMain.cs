using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VANITILE
{
    /// <summary>
    /// ゲームメイン
    /// </summary>
    public class GameMain : SingletonMono<GameMain>
    {
        /// <summary>
        /// 地形マネージャー
        /// </summary>
        [SerializeField, Header("地形マネージャー")] private List<StageManagerBase> managers = new List<StageManagerBase>();

        /// <summary>
        /// シーン直起動時のロードステージid
        /// </summary>
        [SerializeField, Header("デバッグ：ステージID")] private int debugStageId = 0;

        /// <summary>
        /// シーン直起動時のロードステージid
        /// </summary>
        [SerializeField, Header("StageTransitionScriptable")] private StageTransitionScriptable stageTransitionData = null;

        /// <summary>
        /// 現在のステージ番号
        /// </summary>
        private int currentStageId = -1;

        /// <summary>
        /// StageDataManager
        /// </summary>
        public StageSaveData.Data CurrentStageData { get; protected set; }

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
            GameSaveDataModel.Instance.ClearStageCount = this.currentStageId;

            // 現在のステージで終了か
            if (this.stageTransitionData.StageDatas[this.currentStageId].IsEndStage())
            {
                Debug.Log($"[Stage]設定されているステージを全てクリア");
                SceneManager.LoadScene(DefineData.SceneName.TitleScene.ToString());
            }
            else
            {
                // 次のステージへ
                this.currentStageId++;

                // 最新のプレイステージIdを保存
                GameSaveDataModel.Instance.PlayLastStageId = this.currentStageId;

                Debug.Log($"[Stage]arrayId:{this.currentStageId} :{this.stageTransitionData.StageDatas.Count}");
            }
        }

        /// <summary>
        /// ステージ遷移
        /// </summary>
        private void Transition()
        {
            // ステージ遷移
            var stageIds = this.stageTransitionData.StageDatas;
            var changeStage = new ChangeStage(this);
            changeStage.Transition(stageIds[this.currentStageId]);
            this.CurrentStageData = changeStage.CurrentStageData;

            // 終了直後に State の変更を行う
            // TODO:初期化後に配置するかも
            StageDataModel.Instance.GameStart();

            // 各モデルの初期化
            foreach (var mana in this.managers)
            {
                mana.Init();
            }
        }

        /// <summary>
        /// 初期化
        /// TODO:ステージセレクトシーンからstageidを引数として初期化する。StartがInit(stageid)になる予定。
        /// </summary>
        private void Start()
        {
            if (SceneManager.GetActiveScene().name != $"StageDesignManager")
            {
                // 開始ステージの設定 -1の初期値ならデバッグステージ番号の開始
                this.currentStageId = StageDataModel.Instance.CurrentStageId == -1 ? this.debugStageId : StageDataModel.Instance.CurrentStageId;
                this.Transition();
            }

            // メインBGM再生
            SoundManager.Instance.PlayBgm(DefineData.BgmType.Main);
        }

        /// <summary>
        /// 破棄時に呼ばれる
        /// </summary>
        private void OnDestroy()
        {
        }
    }
}
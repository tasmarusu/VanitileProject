using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
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
        public StageSaveData.Data CurrentStageData { get; private set; }

        /// <summary>
        /// 次のステージへ遷移
        /// </summary>
        public void TransitionNextStage()
        {
            this.StartCoroutine(this.TransitionNextStageImpl());
        }

        /// <summary>
        /// 次のステージへ遷移
        /// </summary>
        public void TransitionCurrentStage()
        {
            this.StartCoroutine(this.TransitionNextStageImpl(false));
        }

        /// <summary>
        /// 次のステージへ遷移
        /// </summary>
        private IEnumerator TransitionNextStageImpl(bool isNext = true)
        {
            Debug.Log($"[Stage]遷移開始");
            var transition = GameObject.Instantiate(Resources.Load<Transition>("Prefabs/Common/Transition"));
            yield return transition.In();

            if (isNext)
            {
                this.NextStageId();
            }

            this.Transition();

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
            // 次のステージへ
            this.currentStageId++;
            if (this.currentStageId > this.stageTransitionData.StageIds.Count)
            {
                this.currentStageId = this.stageTransitionData.StageIds.Count;
            }

            Debug.Log($"[Stage]arrayId:{this.currentStageId} :{this.stageTransitionData.StageIds.Count}");
        }

        /// <summary>
        /// ステージ遷移
        /// </summary>
        private void Transition()
        {
            if (this.currentStageId == this.stageTransitionData.StageIds.Count)
            {
                SceneManager.LoadScene(DefineData.SceneName.TitleScene.ToString());
                return;
            }

            // ステージ遷移
            var stageIds = this.stageTransitionData.StageIds;
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
            // 開始ステージの設定 -1の初期値ならデバッグステージ番号の開始
            this.currentStageId = StageDataModel.Instance.CurrentStageId == -1 ? this.debugStageId : StageDataModel.Instance.CurrentStageId;
            this.Transition();

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
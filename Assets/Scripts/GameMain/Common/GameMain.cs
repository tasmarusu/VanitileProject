using System.Collections;
using System.Collections.Generic;
using UniRx;
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
        /// Canvas
        /// </summary>
        [SerializeField, Header("Canvas")] private Transform uiCanvas = null;

        /// <summary>
        /// シーン直起動時のロードステージid
        /// </summary>
        [SerializeField, Header("デバッグ：ステージID")] private int debugStageId = 0;

        /// <summary>
        /// GameMainPoseManager
        /// </summary>
        [field: SerializeField, Header("GameMainPoseManager")] private GameMainPoseManager posePrefab = null;

        /// <summary>
        /// ClearManager
        /// </summary>
        [field: SerializeField, Header("ClearManager")] private ClearManager clearPrefab = null;

        /// <summary>
        /// GameMainTrans
        /// </summary>
        [field: SerializeField, Header("GameMainTrans")] public GameMainTransition GameMainTrans { get; private set; } = null;

        /// <summary>
        /// シーン直起動時のロードステージid
        /// </summary>
        [field: SerializeField] public StageTransitionScriptable StageTransitionData { get; private set; } = null;

        /// <summary>
        /// 操作CompositeDisposable
        /// </summary>
        private CompositeDisposable controllDisposables = new CompositeDisposable();

        /// <summary>
        /// StageDataManager
        /// </summary>
        public StageSaveData.Data CurrentStageData { get; set; }

        /// <summary>
        /// ステージ初期化
        /// </summary>
        public void StageManagerInit()
        {
            foreach (var mana in this.managers)
            {
                mana.Init();
            }
        }

        /// <summary>
        /// クリアUIの表示
        /// </summary>
        public void AppearClearUI()
        {
            var clear = GameObject.Instantiate(this.clearPrefab.gameObject, this.uiCanvas).GetComponent<ClearManager>();
            this.StartCoroutine(clear.Init());
            this.StartCoroutine(clear.Finalize());
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
                var id = StageDataModel.Instance.CurrentStageId;
                StageDataModel.Instance.CurrentStageId = id == -1 ? this.debugStageId : id;
                this.GameMainTrans.Transition();
            }

            // 操作
            this.InputController();

            // メインBGM再生
            SoundManager.Instance.PlayBgm(DefineData.BgmType.Main);
        }

        /// <summary>
        /// 操作
        /// </summary>
        private void InputController()
        {
            // 戻るボタン押下
            InputManager.Instance.ObserveEveryValueChanged(x => x.Back)
                .Where(x => x)
                .Where(_=> StageDataModel.Instance.IsAbleMovePlayer())  // プレイヤーが動作可能
                .Subscribe(_ =>
                {
                    // ポーズ開く
                    var select = GameObject.Instantiate(this.posePrefab.gameObject, this.uiCanvas).GetComponent<GameMainPoseManager>();
                    this.StartCoroutine(select.Init());
                    this.StartCoroutine(select.Finalize());
                })
                .AddTo(this.controllDisposables);
        }

        /// <summary>
        /// 破棄時に呼ばれる
        /// </summary>
        private void OnDestroy()
        {
            this.controllDisposables.Clear();
        }
    }
}
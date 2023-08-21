namespace VANITILE
{
    using System.Collections.Generic;
    using UniRx;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// ゲームメイン
    /// </summary>
    public class GameMain : SingletonMono<GameMain>
    {
        /// <summary>
        /// 地形マネージャー
        /// </summary>
        protected StageManagerBase[] managers = null;

        /// <summary>
        /// Canvas
        /// </summary>
        private Transform uiCanvas = null;

        /// <summary>
        /// 操作CompositeDisposable
        /// </summary>
        private CompositeDisposable controllDisposables = new CompositeDisposable();

        /// <summary>
        /// GameMainTrans
        /// </summary>
        public GameMainTransition GameMainTrans { get; private set; } = null;

        /// <summary>
        /// StageTransitionScriptable
        /// </summary>
        public StageTransitionScriptable StageTransitionData { get; private set; } = null;

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
            var clearPrefab = Resources.Load<ClearManager>("Prefabs/MainGameUI/GameMainClearRoot");
            var clear = GameObject.Instantiate(clearPrefab.gameObject, this.uiCanvas).GetComponent<ClearManager>();
            this.StartCoroutine(clear.Init());
            this.StartCoroutine(clear.Finalize());
        }

        /// <summary>
        /// 初期化
        /// TODO:ステージセレクトシーンからstageidを引数として初期化する。StartがInit(stageid)になる予定。
        /// </summary>
        private void Start()
        {
            // コンポーネントなど取得
            this.uiCanvas = this.transform.GetComponentInChildren<Canvas>().transform;
            this.GameMainTrans = this.transform.GetComponent<GameMainTransition>();
            this.StageTransitionData = Resources.Load<StageTransitionScriptable>("Scriptable/StageTransition");
            this.managers = this.transform.GetComponentsInChildren<StageManagerBase>();

            // シーン開始
            if (SceneManager.GetActiveScene().name != $"StageDesignManager")
            {
                // 開始ステージの設定 -1の初期値ならデバッグステージ番号の開始
                var id = StageDataModel.Instance.CurrentStageId;
                StageDataModel.Instance.CurrentStageId = id == -1 ? 0 : id;
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
                .Where(_ => StageDataModel.Instance.IsAbleMovePlayer())  // プレイヤーが動作可能
                .Subscribe(_ =>
                {
                    // ポーズ開く
                    var posePrefab = Resources.Load<GameMainPoseManager>("Prefabs/MainGameUI/GameMainPoseRoot");
                    var select = GameObject.Instantiate(posePrefab.gameObject, this.uiCanvas).GetComponent<GameMainPoseManager>();
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
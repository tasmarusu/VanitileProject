using System.Collections.Generic;
using UnityEngine;

namespace VANITILE
{
    /// <summary>
    /// プレイヤー操作
    /// </summary>
    public class PlayerManager : StageManagerBase
    {
        /// <summary>
        /// プレイヤー失敗ポイント
        /// </summary>
        [SerializeField, Header("プレイヤー失敗ポイント")] private PlayerMissPoint playerMissPoint = null;

        /// <summary>
        /// PlayerController 
        /// </summary>
        private List<PlayerController> playerControllers = new List<PlayerController>();

        /// <summary>
        /// プレイヤーの取得
        /// </summary>
        public List<PlayerController> PlayerControllers => this.playerControllers;

        /// 初期化
        /// </summary>
        /// <summary>
        public override void Init()
        {
            this.playerControllers.Clear();
            base.Init();
            this.playerMissPoint.Init();
            StageDataModel.Instance.SetPlayerCount(this.playerControllers.Count);
        }

        /// <summary>
        /// ゴールの作成と配置 
        /// </summary>
        protected override void CreateStageAndPlace()
        {
            var models = GameMain.Instance.CurrentStageData.Parts.FindAll(model => model.Type == DefineData.StagePartType.Player);
            Debug.Log($"[Load]Player生成 :{models.Count}体");
            foreach (var model in models)
            {
                var part = GameObject.Instantiate(
                    model.Prefab,
                    model.Point,
                    Quaternion.identity,
                    this.transform).
                    GetComponent<PlayerController>();

                this.playerControllers.Add(part);
                part.Initialize();
                part.IsControll = true;
            }
        }
    }
}

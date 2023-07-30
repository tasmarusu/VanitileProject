using System.Collections.Generic;
using UnityEngine;


namespace VANITILE
{
    /// <summary>
    /// ステージマネージャー
    /// </summary>
    public class BlockManager : StageManagerBase
    {
        private List<BlockPart> blockPart = new List<BlockPart>();

        /// <summary>
        /// ステージ初期化
        /// </summary>
        public override void Init()
        {
            this.blockPart.Clear();

            base.Init();

            StageDataModel.Instance.SetBlockCount(this.blockPart.Count);
        }

        /// <summary>
        /// ステージの作成と配置
        /// </summary>
        protected override void CreateStageAndPlace()
        {
            var models = GameMain.Instance.CurrentStageData.Parts.FindAll(model => model.Type == DefineData.StagePartType.Block);
            Debug.Log($"[Create]Blockmodels:{models.Count}");
            foreach (var model in models)
            {
                this.blockPart.Add(GameObject.Instantiate(model.Prefab, model.Point, Quaternion.identity, this.transform).GetComponent<BlockPart>());
                this.blockPart[this.blockPart.Count - 1].Initialize();
            }
        }
    }
}
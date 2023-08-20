namespace VANITILE
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 鍵管理クラス
    /// </summary>
    public class KeyManager : StageManagerBase
    {
        /// <summary>
        /// 全鍵情報 
        /// </summary>
        private List<KeyPart> keyParts = new List<KeyPart>();

        /// <summary>
        /// 鍵情報
        /// </summary>
        public List<KeyPart> KeyParts => this.keyParts;

        /// <summary>
        /// 全鍵取得したか
        /// </summary>
        /// <returns>bool</returns>
        public bool IsAllGetKey()
        {
            foreach (var key in this.keyParts)
            {
                // 取得していない鍵がある
                if (key.IsGet == false)
                {
                    return false;
                }
            }

            // 全鍵取得
            return true;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public override void Init()
        {
            this.keyParts.Clear();

            base.Init();
            StageDataModel.Instance.SetKeyCount(this.KeyParts.Count);
        }

        /// <summary>
        /// 鍵の作成と配置
        /// </summary>
        protected override void CreateStageAndPlace()
        {
            var models = GameMain.Instance.CurrentStageData.Parts.FindAll(model => model.Type == DefineData.StagePartType.Key);
            Debug.Log($"[Create]KeyModels:{models.Count}");
            foreach (var model in models)
            {
                var key = GameObject.Instantiate(
                    model.Prefab,
                    model.Point,
                    Quaternion.identity,
                    this.transform)
                    .GetComponent<KeyPart>();

                this.keyParts.Add(key);
                key.Initialize();
            }
        }
    }
}

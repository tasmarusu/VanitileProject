using System.Collections.Generic;
using UnityEngine;

namespace VANITILE
{
    /// <summary>
    /// ステージに配置する基底クラス
    /// </summary>
    public abstract class StageManagerBase : MonoBehaviour
    {
        /// <summary>
        /// 親になる地形種類の設定 TODO:なんでこれListなんやっけ？？？
        /// </summary>
        [field: SerializeField, Header("親になる地形種類")] public List<DefineData.StagePartType> PartTypes = new List<DefineData.StagePartType>();

        /// <summary>
        /// 初期化
        /// </summary>
        public virtual void Init()
        {
            this.CreateStageAndPlace();
        }

        /// <summary>
        /// TODO:派生クラスのみで使用したいから private だが派生クラス以外での使用は認めないやつ考える
        /// </summary>
        protected abstract void CreateStageAndPlace();
    }
}
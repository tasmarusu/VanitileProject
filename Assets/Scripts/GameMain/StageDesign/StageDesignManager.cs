using System.Collections.Generic;
using UnityEngine;

namespace VANITILE
{
    /// <summary>
    /// ステージ作成シーンのマネージャー
    /// </summary>
    public class StageDesignManager : MonoBehaviour
    {
        [field: SerializeField] public List<StageManagerBase> Managers { get; private set; } = new List<StageManagerBase>();
    }
}
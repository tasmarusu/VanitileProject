using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージスクリプタブル
/// </summary>
[CreateAssetMenu(fileName = "StageTransition", menuName = "ScriptableObjects/StageTransition", order = 2)]
public class StageTransitionScriptable : ScriptableObject
{
    /// <summary>
    /// ステージId
    /// 遷移可能Id
    /// </summary>
    [SerializeField, Header("使用するステージId")] private List<Data> stageIds = new List<Data>();

    /// <summary>
    /// データ集合体
    /// </summary>
    [System.Serializable]
    public class Data
    {
        /// <summary>
        /// Id
        /// </summary>
        [field: SerializeField] public int Id { get; private set; }

        /// <summary>
        /// Jsonデータ
        /// </summary>
        [field: SerializeField] public Object Json { get; private set; }
    }

    /// <summary>
    /// ステージId
    /// </summary>
    public List<Data> StageIds => this.stageIds;
}

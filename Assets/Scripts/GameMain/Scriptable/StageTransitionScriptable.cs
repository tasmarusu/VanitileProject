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
    [SerializeField, Header("使用するステージId")] private List<Data> StageDataLists = new List<Data>();

    /// <summary>
    /// 次のステージ種類
    /// </summary>
    public enum NextDataType
    {
        /// <summary>
        /// 開始ステージ
        /// </summary>
        Start,

        /// <summary>
        /// 次ステージ
        /// </summary>
        Next,

        /// <summary>
        /// 最後のステージ
        /// </summary>
        End,
    }

    /// <summary>
    /// データ集合体
    /// </summary>
    [System.Serializable]
    public class Data
    {
        /// <summary>
        /// Id
        /// 現状不使用
        /// </summary>
        [field: SerializeField] public int Id { get; private set; }

        /// <summary>
        /// 次のステージ情報
        /// </summary>
        [field: SerializeField] public NextDataType nextDataType { get; private set; } = NextDataType.Next;

        /// <summary>
        /// Jsonデータ
        /// </summary>
        [field: SerializeField] public Object Json { get; private set; }

        /// <summary>
        /// 最後のステージか
        /// </summary>
        /// <returns>bool</returns>
        public bool IsEndStage()
        {
            return this.nextDataType == NextDataType.End;
        }
    }

    /// <summary>
    /// ステージId
    /// </summary>
    public List<Data> StageDatas => this.StageDataLists;
}

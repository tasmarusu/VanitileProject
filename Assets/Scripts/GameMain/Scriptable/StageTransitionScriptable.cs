namespace VANITILE
{
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
        [SerializeField, Header("使用するステージId")] private List<Data> stageDataLists = new List<Data>();

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
        /// ステージId
        /// </summary>
        public List<Data> StageDatas => this.stageDataLists;

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
            [field: SerializeField] public NextDataType NextDataType { get; private set; } = NextDataType.Next;

            /// <summary>
            /// Jsonデータ
            /// </summary>
            [field: SerializeField] public UnityEngine.Object Json { get; private set; }

            /// <summary>
            /// クリア時のタイムからスコアを求める
            /// </summary>
            [field: SerializeField] public ClearScoreTime ClearScoreTimes { get; private set; }

            /// <summary>
            /// 最後のステージか
            /// </summary>
            /// <returns>bool</returns>
            public bool IsEndStage()
            {
                return this.NextDataType == NextDataType.End;
            }

            /// <summary>
            /// クリア時のタイムからスコアを求める
            /// </summary>
            [System.Serializable]
            public class ClearScoreTime
            {
                /// <summary>
                /// スコア S
                /// </summary>
                [field: SerializeField] public float STimer { get; private set; } = 10.0f;

                /// <summary>
                /// スコア A
                /// </summary>
                [field: SerializeField] public float ATimer { get; private set; } = 20.0f;

                /// <summary>
                /// スコア B
                /// </summary>
                [field: SerializeField] public float BTimer { get; private set; } = 30.0f;

                /// <summary>
                /// スコア C
                /// </summary>
                [field: SerializeField] public float CTimer { get; private set; } = 40.0f;
            }
        }
    }
}
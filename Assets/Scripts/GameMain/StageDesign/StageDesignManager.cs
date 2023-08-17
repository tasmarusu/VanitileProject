using System.Collections.Generic;
using UnityEngine;
using static VANITILE.StageSaveData.Data;

namespace VANITILE
{
    /// <summary>
    /// ステージ作成シーンのマネージャー
    /// </summary>
    public class StageDesignManager : GameMain
    {
        [field: SerializeField] public List<StageManagerBase> Managers { get; private set; } = new List<StageManagerBase>();

        /// <summary>
        /// 初期化
        /// </summary>
        private void Start()
        {
            this.ReLoadStage();



            // 終了直後に State の変更を行う
            // TODO:初期化後に配置するかも
            StageDataModel.Instance.GameStart();

            // 各モデルの初期化
            foreach (var mana in this.Managers)
            {
                mana.Init();
            }
        }

        /// <summary>
        /// ステージの再生成
        /// </summary>
        private void ReLoadStage()
        {
            // UnityEditor に配置されてるステージ情報を取得
            // これ作ってて思ったがこのやり方だと何かの種類増えた時の改修ポイント多くね～～～？
            var parts = new List<StageLoadData>();
            foreach (var mana in this.Managers)
            {
                switch (mana.PartTypes[0])
                {
                    case DefineData.StagePartType.Block:
                        var blocks = mana.GetComponentsInChildren<BlockPart>();
                        for (int i = 0; i < blocks.Length; i++)
                        {
                            parts.Add(new StageLoadData(mana.PartTypes[0], blocks[i].transform.position, null));
                        }
                        break;

                    case DefineData.StagePartType.Key:
                        var keys = mana.GetComponentsInChildren<KeyPart>();
                        for (int i = 0; i < keys.Length; i++)
                        {
                            parts.Add(new StageLoadData(mana.PartTypes[0], keys[i].transform.position, null));
                        }
                        break;

                    case DefineData.StagePartType.Goal:
                        var goals = mana.GetComponentsInChildren<GoalPart>();
                        for (int i = 0; i < goals.Length; i++)
                        {
                            parts.Add(new StageLoadData(mana.PartTypes[0], goals[i].transform.position, null));
                        }
                        break;

                    case DefineData.StagePartType.Player:
                        var players = mana.GetComponentsInChildren<PlayerController>();
                        for (int i = 0; i < players.Length; i++)
                        {
                            parts.Add(new StageLoadData(mana.PartTypes[0], players[i].transform.position, null));
                        }
                        break;

                    default:
                        Debug.LogError($"[Load]なんやねんこれ");
                        break;
                }
            }

            this.CurrentStageData = new StageSaveData.Data(parts);

            foreach (var mana in this.Managers)
            {
                for (int i = mana.transform.childCount - 1; i >= 0; i--)
                {
                    GameObject.Destroy(mana.transform.GetChild(i).gameObject);
                }
            }
        }
    }
}
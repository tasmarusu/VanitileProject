using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VANITILE
{
    public class StageDesignEditor : MonoBehaviour
    {
    }

    /// <summary>
    /// エディター拡張クラス
    /// </summary>
    [CustomEditor(typeof(StageDesignEditor))]
    public class StageDesignEditorWindow : Editor
    {
        /// <summary>
        /// 編集中のステージID
        /// </summary>
        private int stageId = 0;

        /// <summary>
        /// ステージデザインマネージャー
        /// </summary>
        private StageDesignManager manager;

        /// <summary>
        /// OnInspectorGUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            // 何かしら画面上の更新が入ったらこのメソッドに入り続ける

            base.OnInspectorGUI();

            var baseEditor = target as StageDesignEditor;
            var stageSaveData = new StageSaveData();

            if (this.manager == null)
            {
                this.manager = baseEditor.GetComponent<StageDesignManager>();
            }

            // ステージID
            this.stageId = EditorGUILayout.IntField("ステージID", this.stageId);

            // セーブボタン
            if (GUILayout.Button("セーブ"))
            {
                // ゲロカス糞重い処理だけど許して！
                var blocks = this.manager.GetComponentsInChildren<BlockPart>().Select(x => x.transform.position).Select(pos => new StageSaveData.Data.StageEachData(DefineData.StagePartType.Block, pos)).ToList();
                var keys = this.manager.GetComponentsInChildren<KeyPart>().Select(x => x.transform.position).Select(pos => new StageSaveData.Data.StageEachData(DefineData.StagePartType.Key, pos)).ToList();
                var goals = this.manager.GetComponentsInChildren<GoalPart>().Select(x => x.transform.position).Select(pos => new StageSaveData.Data.StageEachData(DefineData.StagePartType.Goal, pos)).ToList();
                var players = this.manager.GetComponentsInChildren<PlayerController>().Select(x => x.transform.position).Select(pos => new StageSaveData.Data.StageEachData(DefineData.StagePartType.Player, pos)).ToList();

                // ShowModelが閉じるまで待機される。
                var window = CreateInstance<PopUpCheckStageModelEditorWindow>();
                window.titleContent = new GUIContent("本当にセーブしますか？");
                window.Str = "セーブ";
                window.InfoStr = $" StageId:{this.stageId} \n 上書きセーブするか:{stageSaveData.IsExistSaveData(this.stageId)} \n ブロック数：{blocks.Count} \n 鍵数：{keys.Count} \n ゴール数：{goals.Count} \n プレイヤー数：{players.Count}";
                window.ShowModal();

                // セーブしない
                if (window.IsSave == false)
                {
                    Debug.Log($"[Save]セーブはしません");
                    return;
                }

                // ステージをセーブ
                var time = Time.realtimeSinceStartup;
                var saveData = new List<StageSaveData.Data.StageEachData>();
                saveData.AddRange(blocks);
                saveData.AddRange(keys);
                saveData.AddRange(goals);
                saveData.AddRange(players);
                stageSaveData.Save(new StageSaveData.Data.JsonData(this.stageId, saveData));
                Debug.Log($"[Load]ステージのセーブ完了:{Time.realtimeSinceStartup - time}");
            }

            // ロードボタン
            if (GUILayout.Button("ロード"))
            {
                // 保存されているステージのロード
                var loadData = stageSaveData.Load(this.stageId);
                if (loadData == default)
                {
                    Debug.LogError($"[Load]ステージのロード失敗");
                    return;
                }

                // 数数える
                Dictionary<DefineData.StagePartType, int> partCounts
                    = new Dictionary<DefineData.StagePartType, int>(System.Enum.GetValues(typeof(DefineData.StagePartType)).Length);
                foreach (var type in System.Enum.GetValues(typeof(DefineData.StagePartType)))
                {
                    partCounts.Add((DefineData.StagePartType)type, 0);
                }

                foreach (var part in loadData.Parts)
                {
                    partCounts[part.Type]++;
                }

                // ShowModelが閉じるまで待機される。
                var window = CreateInstance<PopUpCheckStageModelEditorWindow>();
                window.titleContent = new GUIContent("本当にロードしますか？");
                window.Str = "ロード";
                window.InfoStr = $" StageId:{this.stageId} \n ブロック数：{partCounts[DefineData.StagePartType.Block].ToString()} \n 鍵数：{partCounts[DefineData.StagePartType.Key]} \n ゴール数：{partCounts[DefineData.StagePartType.Goal]} \n プレイヤー数：{partCounts[DefineData.StagePartType.Player]}";
                window.ShowModal();

                // ロードしない
                if (window.IsSave == false)
                {
                    Debug.Log($"[Load]ロードはしません");
                    return;
                }

                // ロードしたステージの反映開始
                var time = Time.realtimeSinceStartup;

                // 全ブロック、プレイヤー、鍵、ゴール削除
                var commonParts = this.manager.GetComponentsInChildren<CommonPart>();
                for (int i = commonParts.Length - 1; i >= 0; i--)
                {
                    GameObject.DestroyImmediate(commonParts[i].gameObject);
                }

                // ステージを配置していく
                foreach (var part in loadData.Parts)
                {
                    var parent = this.manager.Managers.Find(x => x.PartTypes.Contains(part.Type)).transform;
                    GameObject.Instantiate(part.Prefab, part.Point, Quaternion.identity, parent);
                }

                Debug.Log($"[Load]ステージのロード完了:{Time.realtimeSinceStartup - time}");
            }
        }

        /// <summary>
        /// プレハブのロード
        /// </summary>
        private Object LoadPrefabFromResources(string path)
        {
            var prefab = Resources.Load(path);
            if (prefab == null)
            {
                Debug.LogError($"[Load]リソースのロードに失敗しました path:{path}");
                return null;
            }

            return prefab;
        }
    }

    /// <summary>
    /// ボタンを押下した際にポップアップを表示して確認を取る
    /// </summary>
    public class PopUpCheckStageModelEditorWindow : EditorWindow
    {
        /// <summary>
        /// 任意の文字を入れる
        /// </summary>
        public string Str { get; set; } = "セーブ";

        /// <summary>
        /// 表示情報
        /// </summary>
        public string InfoStr { get; set; } = "";

        /// <summary>
        /// セーブチェック
        /// </summary>
        public bool IsSave { get; private set; } = false;

        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label($"{this.Str} をするかの注意喚起", GUILayout.ExpandWidth(true));
            GUILayout.Space(10);

            GUILayout.Label($"***{Str}情報***", GUILayout.ExpandWidth(true));
            GUILayout.Label($"{InfoStr}", GUILayout.ExpandWidth(true));

            EditorGUILayout.BeginVertical();
            if (GUILayout.Button($"{this.Str} する", GUILayout.ExpandWidth(true)))
            {
                this.IsSave = true;
                this.Close();
            }
            GUILayout.Space(10);
            if (GUILayout.Button($"{this.Str} しない", GUILayout.ExpandWidth(true)))
            {
                this.IsSave = false;
                this.Close();
            }
            EditorGUILayout.EndVertical();
        }
    }
}

namespace VANITILE
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

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
        /// ステージデザインマネージャー
        /// </summary>
        private StageDesignManager manager;

        /// <summary>
        /// 現在のステージ番号 ロードしたら更新される
        /// </summary>
        private int currentStage = 0;

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

            var currentStageStr = this.currentStage == 0 ? "現在のステージ分からんからセーブかロードして下さい" : $"{this.currentStage}ステージ調整中";
            EditorGUILayout.HelpBox(currentStageStr, MessageType.Info);

            #region ボタン

            if (GUILayout.Button("セーブ先の表示"))
            {
                string[] guids = AssetDatabase.FindAssets(string.Empty, new string[] { $"Assets/Data/Resources/StageData" });
                string[] paths = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToArray();
                EditorGUIUtility.PingObject(AssetDatabase.LoadAllAssetsAtPath(paths[0])[0]);
            }

            if (GUILayout.Button("ステージ順の調整"))
            {
                string[] guids = AssetDatabase.FindAssets(string.Empty, new string[] { $"Assets/Data" });
                string[] paths = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToArray();

                for (int i = 0; i < paths.Length; i++)
                {
                    if (paths[i].ToString().Replace("Assets/Data/", string.Empty).Substring(0, 5) == "Stage")
                    {
                        EditorGUIUtility.PingObject(AssetDatabase.LoadAllAssetsAtPath(paths[i])[0]);
                        break;
                    }
                }
            }

            if (GUILayout.Button("ブロック調整"))
            {
                Debug.Log($"[調整]開始 浮動小数があるブロック位置の四捨五入をします");
                var blocks = this.manager.GetComponentsInChildren<BlockPart>().ToList();

                // 位置四捨五入
                foreach (var block in blocks)
                {
                    var pos = block.transform.position;
                    block.transform.position = new Vector3(Mathf.Floor(pos.x), Mathf.Floor(pos.y), .0f);
                    block.name = $"Block_x{block.transform.position.x}y{block.transform.position.y}";
                }

                // ここからちょっとごり押し
                // 位置重複のブロックを削除
                var array = blocks.Select(block => block.transform.position).Select(pos => new Vector2Int((int)pos.x, (int)pos.y)).ToList();
                Debug.Log($"[調整]blocks:{blocks.Count}");
                Debug.Log($"[調整]array:{array.Count}");

                // 不必要なブロックがある場合削除
                for (int i = blocks.Count - 1; i >= 0;)
                {
                    var pos = blocks[i].transform.position;
                    if (array.Contains(new Vector2Int((int)pos.x, (int)pos.y)))
                    {
                        var all = blocks.FindAll(block => new Vector2Int((int)block.transform.position.x, (int)block.transform.position.y) == new Vector2Int((int)pos.x, (int)pos.y));

                        i -= all.Count;
                        for (int j = 1; j < all.Count; j++)
                        {
                            GameObject.DestroyImmediate(all[j].gameObject);

                            // これ無しのやり方募
                            blocks = this.manager.GetComponentsInChildren<BlockPart>().ToList();
                            array = blocks.Select(block => block.transform.position).Select(pos => new Vector2Int((int)pos.x, (int)pos.y)).ToList();
                        }
                    }
                }

                Debug.Log($"[調整]終了 ");
            }

            #endregion

            GUILayout.Space(20);

            #region セーブ
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
                window.FilePath = stageSaveData.GetSaveFilePath();
                window.SaveInfos = this.GetSaveDataCounts();
                window.ShowModal();

                // セーブしない
                if (window.IsSave == false)
                {
                    Debug.Log($"[Save]セーブはしません");
                    return;
                }

                // ステージ番号
                this.currentStage = window.SaveNum;

                // ステージをセーブ
                var time = Time.realtimeSinceStartup;
                var saveData = new List<StageSaveData.Data.StageEachData>();
                saveData.AddRange(blocks);
                saveData.AddRange(keys);
                saveData.AddRange(goals);
                saveData.AddRange(players);
                stageSaveData.Save(new StageSaveData.Data.JsonData(window.SaveNum, saveData));
                Debug.Log($"[Load]ステージのセーブ完了:{Time.realtimeSinceStartup - time}");
            }
            #endregion

            #region ロード
            if (GUILayout.Button("ロード"))
            {
                // ShowModelが閉じるまで待機される。
                var window = CreateInstance<PopUpCheckStageModelEditorWindow>();
                window.titleContent = new GUIContent("本当にロードしますか？");
                window.Str = "ロード";
                window.FilePath = stageSaveData.GetSaveFilePath();
                window.SaveInfos = this.GetSaveDataCounts();
                window.ShowModal();

                // ロードしない
                if (window.IsSave == false)
                {
                    Debug.Log($"[Load]ロードはしません");
                    return;
                }

                // 保存されているステージのロード
                var loadData = stageSaveData.Load(window.SaveNum);
                if (loadData == default)
                {
                    Debug.LogError($"[Load]ステージのロード失敗");
                    return;
                }

                // ステージ番号
                this.currentStage = window.SaveNum;

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
            #endregion
        }

        /// <summary>
        /// セーブしている数を取得
        /// </summary>
        /// <returns></returns>
        private List<int> GetSaveDataCounts()
        {
            var loadCounts = Resources.LoadAll("StageData");
            var result = new List<int>();
            foreach (var obj in loadCounts)
            {
                var str = obj.name.ToString().Replace("Stage_", string.Empty);
                var stageNum = int.Parse(str.Substring(0, 3));
                result.Add(stageNum);
            }

            return result;
        }

        /// <summary>
        /// プレハブのロード
        /// </summary>
        private UnityEngine.Object LoadPrefabFromResources(string path)
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
    /// セーブロード分けるのはもうちょっとごちゃついたらさすがにする。
    /// </summary>
    public class PopUpCheckStageModelEditorWindow : EditorWindow
    {
        /// <summary>
        /// セーブ番号
        /// </summary>
        private string specialStr = string.Empty;

        /// <summary>
        /// 任意の文字を入れる
        /// </summary>
        public string Str { get; set; } = "セーブ";

        /// <summary>
        /// ファイルパス
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// 表示情報
        /// </summary>
        public List<int> SaveInfos { get; set; } = new List<int>();

        /// <summary>
        /// セーブチェック
        /// </summary>
        public bool IsSave { get; private set; } = false;

        /// <summary>
        /// セーブ番号
        /// </summary>
        public int SaveNum { get; private set; } = 0;

        /// <summary>
        /// OnGui
        /// </summary>
        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label($"{this.Str}しますか？選んで下さい トグル:ステージ番号:データ有無 ", GUILayout.ExpandWidth(true));
            GUILayout.Label($"ファイルパス {FilePath}", GUILayout.ExpandWidth(true));
            GUILayout.Space(10);

            for (int i = 1; i < 100; i += 10)
            {
                EditorGUILayout.BeginHorizontal();

                for (int j = i; j < i + 10; j++)
                {
                    var isExist = this.SaveInfos.Contains(j);
                    var str = j.ToString("D3").ToUpperInvariant();
                    str = isExist ? $"{str}[〇]" : $"{str}[✕]";

                    if (this.Str == "ロード")
                    {
                        using (new EditorGUI.DisabledScope(!isExist))
                        {
                            if (GUILayout.Toggle(this.SaveNum == j, $"{str}", GUILayout.ExpandWidth(true)))
                            {
                                this.SaveNum = j;
                            }
                        }
                    }
                    else
                    {
                        if (GUILayout.Toggle(this.SaveNum == j, $"{str}", GUILayout.ExpandWidth(true)))
                        {
                            this.specialStr = isExist ? "上書きするぞ！ " : "新規で作る";
                            this.SaveNum = j;
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            using (new EditorGUI.DisabledScope(this.SaveNum == 0))
            {
                if (GUILayout.Button($"{this.specialStr} {this.SaveNum} {this.Str} する", GUILayout.ExpandWidth(true)))
                {
                    this.IsSave = true;
                    this.Close();
                }
            }

            if (GUILayout.Button($"{this.Str} しない", GUILayout.ExpandWidth(true)))
            {
                this.IsSave = false;
                this.Close();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}

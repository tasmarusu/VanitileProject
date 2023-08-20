namespace VANITILE
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// ステージのセーブ/ロード
    /// </summary>
    public class StageSaveData
    {
        /// <summary>
        /// セーブ
        /// クラス -> Json に変換
        /// </summary>
        public void Save(Data.JsonData data)
        {
            try
            {
                var path = this.GetSaveDataPath(data.StageId);
                using (StreamWriter sw = new StreamWriter(path, false))
                {
                    sw.Write(JsonUtility.ToJson(data, true));
                    sw.Flush();
                    Debug.Log($"[Save]セーブ成功 ID:{data.StageId}");

                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

                    string[] guids = AssetDatabase.FindAssets(string.Empty, new string[] { $"Assets/Data/Resources/StageData" });
                    string[] paths = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToArray();

                    for (int i = 0; i < paths.Length; i++)
                    {
                        if (paths[i].ToString().Replace("Assets/Data/Resources/StageData/Stage_", string.Empty).Substring(0, 3) == data.StageId.ToString("D3"))
                        {
                            EditorGUIUtility.PingObject(AssetDatabase.LoadAllAssetsAtPath(paths[i])[0]);
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[Save]セーブに失敗しました {e}");
            }
        }

        /// <summary>
        /// ロード
        /// Json -> クラスに変換
        /// </summary>
        /// <param name="stageId"></param>
        public Data Load(int stageId)
        {
            // データの有無
            var path = this.GetSaveDataPath(stageId);
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    var jsonData = JsonUtility.FromJson<Data.JsonData>(sr.ReadToEnd());
                    Debug.Assert(jsonData != null, $"[Load]ロードが正常に完了しませんでした id:{stageId} null");
                    Debug.Assert(jsonData != default, $"[Load]ロードが正常に完了しませんでした id:{stageId} default");
                    return new Data(jsonData);
                }
            }

            return default;
        }

        /// <summary>
        /// 保存先セーブパス
        /// </summary>
        /// <returns>string</returns>
        public string GetSaveFilePath()
        {
            return $"{Application.dataPath}/Data/Resources/StageData";
        }

        /// <summary>
        /// セーブデータが存在するか
        /// </summary>
        /// <param name="stageId"></param>
        /// <returns></returns>
        public bool IsExistSaveData(int stageId)
        {
            return File.Exists(this.GetSaveDataPath(stageId));
        }

        /// <summary>
        /// パスの取得
        /// </summary>
        /// <param name="stageId"></param>
        /// <returns></returns>
        private string GetSaveDataPath(int stageId)
        {
            var fileName = $"{this.GetSaveFilePath()}/Stage_{stageId.ToString("D3")}.json";
            Debug.Log($"[File] fileName:{Application.dataPath}/Data/Resources/StageData/");
            return fileName;
        }

        /// <summary>
        /// ステージのデータ
        /// </summary>
        public class Data
        {
            /// <summary>
            /// リソースのパス
            /// </summary>
            private const string ResourcePath = "Prefabs/Stage/";

            /// <summary>
            /// コンストラクタ
            /// デザインシーンから入る際に使用
            /// </summary>
            /// <param name="parts"> ステージに使うパーツ </param>
            public Data(List<StageLoadData> parts)
            {
                for (int i = 0; i < parts.Count; i++)
                {
                    var type = parts[i].Type;
                    var point = parts[i].Point;
                    var prefab = this.LoadPrefabFromResources(type);
                    this.Parts.Add(new StageLoadData(type, point, prefab));
                }
            }

            /// <summary>
            /// コンストラクタ
            /// 通常起動時に使用
            /// </summary>
            /// <param name="jsonData">ロードしたステージデータ</param>
            public Data(JsonData loadJsonData)
            {
                this.StageId = loadJsonData.StageId;
                for (int i = 0; i < loadJsonData.Datas.Count; i++)
                {
                    var type = (DefineData.StagePartType)loadJsonData.Datas[i].Type;
                    var point = loadJsonData.Datas[i].Point;
                    var prefab = this.LoadPrefabFromResources(type);
                    this.Parts.Add(new StageLoadData(type, point, prefab));
                }
            }

            /// <summary>
            /// ステージid
            /// </summary>
            public int StageId { get; private set; }

            /// <summary>
            /// 保存されるモデル
            /// </summary>
            public List<StageLoadData> Parts { get; private set; } = new List<StageLoadData>();

            /// <summary>
            /// リソースからのロード
            /// Partを継承してるやつの名前を統一して 名前の後ろに Type の数字を入れる形が楽になりそう？管理怠い？
            /// </summary>
            /// <returns> リソースからのロードしたオブジェクト </returns>
            private GameObject LoadPrefabFromResources(DefineData.StagePartType type)
            {
                switch (type)
                {
                    case DefineData.StagePartType.Block:
                        return Resources.Load<GameObject>($"{ResourcePath}BlockPart");

                    case DefineData.StagePartType.Key:
                        return Resources.Load<GameObject>($"{ResourcePath}KeyPart");

                    case DefineData.StagePartType.Goal:
                        return Resources.Load<GameObject>($"{ResourcePath}GoalPart");

                    case DefineData.StagePartType.Player:
                        return Resources.Load<GameObject>($"{ResourcePath}PlayerController");

                    default:
                        Debug.LogError($"[Load]defaultに入りました。Typeの指定が無いが??? :{type}");
                        return null;
                }
            }

            /// <summary>
            /// 各ステージのロードした時のデータ
            /// </summary>
            [Serializable]
            public class StageLoadData
            {
                /// <summary>
                /// コンストラクタ
                /// </summary>
                /// <param name="Type">ステージパーツ</param>
                /// <param name="Point">座標</param>
                /// <param name="Prefab">プレハブ</param>
                public StageLoadData(DefineData.StagePartType type, Vector2 point, GameObject prefab)
                {
                    this.Type = type;
                    this.Point = point;
                    this.Prefab = prefab;
                }

                /// <summary>
                /// ステージの種類
                /// </summary>
                public DefineData.StagePartType Type { get; private set; }

                /// <summary>
                /// 生成する座標
                /// </summary>
                public Vector2 Point { get; private set; }

                /// <summary>
                /// プレハブ
                /// </summary>
                public GameObject Prefab { get; private set; }
            }

            /// <summary>
            /// Jsonでやる為。Jsonでやるならクラスデータ保存方法よりこっちが軽そう。
            /// でも増えるデータあればくっそめんどよなぁ…
            /// </summary>
            [Serializable]
            public class JsonData
            {
                /// <summary>
                /// ステージID
                /// </summary>
                [SerializeField] public int StageId = 0;

                /// <summary>
                /// セーブデータ
                /// </summary>
                [SerializeField] public List<StageEachData> Datas = new List<StageEachData>();

                /// <summary>
                /// コンストラクタ
                /// </summary>
                /// <param name="stageId">ステージid</param>
                /// <param name="models">保存モデル</param>
                public JsonData(int stageId, List<StageEachData> models)
                {
                    this.StageId = stageId;
                    this.Datas = models;
                }
            }

            /// <summary>
            /// 各ステージのセーブする時のデータ
            /// 変数の初期化をするとjsonとしてセーブされない？？
            /// </summary>
            [Serializable]
            public class StageEachData
            {
                /// <summary>
                /// ステージの種類
                /// </summary>
                [SerializeField] public int Type;

                /// <summary>
                /// 座標
                /// </summary>
                [SerializeField] public Vector2 Point;

                /// <summary>
                /// コンストラクタ
                /// </summary>
                /// <param name="point">パーツ</param>
                /// <param name="id">座標</param>
                public StageEachData(DefineData.StagePartType type, Vector2 point)
                {
                    this.Type = (int)type;
                    this.Point = point;
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VANITILE
{
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
                using (StreamWriter sw = new StreamWriter(this.GetSaveDataPath(data.StageId), false))
                {
                    sw.Write(JsonUtility.ToJson(data, true));
                    sw.Flush();
                    Debug.Log($"[Save]セーブ成功 ID:{data.StageId}");

                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
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
            var fileName = $"{Application.dataPath}/Data/Resources/StageData/Stage_{stageId.ToString("D3")}.json";
            Debug.Log($"[File] fileName:{Application.dataPath}/Data/Resources/StageData/");
            return fileName;
        }

        /// <summary>
        /// ステージのデータ
        /// </summary>
        public class Data
        {
            private const string resourcePath = "Prefabs/Stage/";

            /// <summary>
            /// ステージid
            /// </summary>
            public int StageId { get; private set; }

            /// <summary>
            /// 保存されるモデル
            /// </summary>
            public List<StageLoadData> Parts { get; private set; } = new List<StageLoadData>();

            /// <summary>
            /// コンストラクタ
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
            /// リソースからのロード
            /// Partを継承してるやつの名前を統一して 名前の後ろに Type の数字を入れる形が楽になりそう？管理怠い？
            /// </summary>
            /// <returns></returns>
            private GameObject LoadPrefabFromResources(DefineData.StagePartType type)
            {
                switch (type)
                {
                    case DefineData.StagePartType.Block:
                        return Resources.Load<GameObject>($"{resourcePath}BlockPart");

                    case DefineData.StagePartType.Key:
                        return Resources.Load<GameObject>($"{resourcePath}KeyPart");

                    case DefineData.StagePartType.Goal:
                        return Resources.Load<GameObject>($"{resourcePath}GoalPart");

                    case DefineData.StagePartType.Player:
                        return Resources.Load<GameObject>($"{resourcePath}PlayerController");

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

                /// <summary>
                /// コンストラクタ
                /// </summary>
                /// <param name="Type"></param>
                /// <param name="Point"></param>
                /// <param name="Prefab"></param>
                public StageLoadData(DefineData.StagePartType type, Vector2 point, GameObject prefab)
                {
                    this.Type = type;
                    this.Point = point;
                    this.Prefab = prefab;
                }
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
                /// <param name="point"></param>
                /// <param name="id"></param>
                public StageEachData(DefineData.StagePartType type, Vector2 point)
                {
                    this.Type = (int)type;
                    this.Point = point;
                }
            }
        }
    }
}
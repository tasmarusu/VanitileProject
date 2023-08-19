using UnityEngine;

namespace VANITILE
{
    /// <summary>
    /// サウンドマネージャー
    /// </summary>
    public class GameSaveDataModel : SingletonMono<GameSaveDataModel>
    {
        /// <summary>
        /// BGMの保存名
        /// </summary>
        private const string BgmKeyName = "BGMVolume";

        /// <summary>
        /// SEの保存名
        /// </summary>
        private const string SeKeyName = "SEVolume";

        /// <summary>
        /// 最後にプレイしたステージ
        /// </summary>
        private const string PlayLastStageName = "PlayLastStage";

        /// <summary>
        /// クリアステージ数の保存
        /// </summary>
        private const string ClearStageCountName = "ClearStageCount";

        /// <summary>
        /// 各ステージのベストスコアタイム
        /// </summary>
        private const string ClearStageTimeName = "ClearStageTime";

        /// <summary>
        /// Bgm音量
        /// </summary>
        public float BgmVolume
        {
            get
            {
                var num = PlayerPrefs.GetFloat(BgmKeyName);
                Debug.Log($"[UserData]BgmVolume:{num}");
                return num;
            }
            set
            {
                Debug.Log($"[GameSave]BgmVolume の保存:{value}");
                PlayerPrefs.SetFloat(BgmKeyName, value);
            }
        }

        /// <summary>
        /// Se音量
        /// </summary>
        public float SeVolume
        {
            get
            {
                var num = PlayerPrefs.GetFloat(SeKeyName);
                Debug.Log($"[UserData]SeVolume:{num}");
                return num;
            }
            set
            {
                Debug.Log($"[GameSave]SeVolume の保存:{value}");
                PlayerPrefs.SetFloat(SeKeyName, value);
            }
        }

        /// <summary>
        /// 最後にプレイしたステージId
        /// </summary>
        public int PlayLastStageId
        {
            get
            {
                var num = PlayerPrefs.GetInt(PlayLastStageName);
                Debug.Log($"[UserData]PlayLastStageId:{num}");
                return num;
            }
            set
            {
                Debug.Log($"[GameSave]PlayLastStage の保存:{value}");
                PlayerPrefs.SetInt(PlayLastStageName, value);
            }
        }

        /// <summary>
        /// クリアステージ数
        /// </summary>
        public int ClearStageCount
        {
            get
            {
                var num = PlayerPrefs.GetInt(ClearStageCountName);
                Debug.Log($"[UserData]ClearStageCount:{num}");
                return num;
            }
            set
            {
                Debug.Log($"[GameSave]ClearStageCount の保存:{value}");
                PlayerPrefs.SetInt(ClearStageCountName, value);
            }
        }

        /// <summary>
        /// クリアタイムの保存
        /// </summary>
        /// <param name="stageNum">ステージ番号</param>
        /// <param name="clearTime">クリアタイム</param>
        public void SetClearStageTime(int stageNum, float clearTime)
        {
            Debug.Log($"[GameSave]SetClearStageTime の保存:{stageNum} {clearTime}");
            PlayerPrefs.SetFloat($"{ClearStageTimeName}_{stageNum}", clearTime);
        }

        /// <summary>
        /// クリアタイムの取得
        /// </summary>
        /// <param name="stageNum">ステージ番号</param>
        /// <returns>クリアタイム</returns>
        public float GetClearStageTime(int stageNum)
        {
            Debug.Log($"[GameSave]SetClearStageTime の取得:{stageNum}");
            return PlayerPrefs.GetFloat($"{ClearStageTimeName}_{stageNum}");
        }

        /// <summary>
        /// ゲーム実行前に呼び出す
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            var obj = Resources.Load<GameSaveDataModel>($"Prefabs/Common/GameSaveDataModel");
            obj = GameObject.Instantiate(obj);
            GameObject.DontDestroyOnLoad(obj);
        }
    }
}
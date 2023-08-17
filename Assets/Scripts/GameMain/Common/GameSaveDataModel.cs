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
        public const string BgmKeyName = "BGMVolume";

        /// <summary>
        /// SEの保存名
        /// </summary>
        public const string SeKeyName = "SEVolume";

        /// <summary>
        /// 最後にプレイしたステージ
        /// </summary>
        public const string PlayLastStageName = "PlayLastStage";

        /// <summary>
        /// クリアステージ数の保存
        /// </summary>
        public const string ClearStageCountName = "ClearStageCount";

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
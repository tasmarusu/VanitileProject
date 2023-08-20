namespace VANITILE
{
    using UnityEngine;

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
        private const string CurrentClearStageNumName = "ClearStageCount";

        /// <summary>
        /// 各ステージのベストスコアタイム
        /// </summary>
        private const string ClearStageTimeName = "ClearStageTime";

        /// <summary>
        /// ステージクリア番号
        /// </summary>
        private const string ClearStageNumName = "ClearStageNum";

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
        /// 現在クリアしたステージ数
        /// </summary>
        public int CurrentClearStageNum
        {
            get
            {
                var num = PlayerPrefs.GetInt(CurrentClearStageNumName);
                Debug.Log($"[UserData]CurrentClearStageNum:{num}");
                return num;
            }

            set
            {
                Debug.Log($"[GameSave]ClearStageCount の保存:{value}");
                PlayerPrefs.SetInt(CurrentClearStageNumName, value);
            }
        }

        /// <summary>
        /// クリアタイムの保存
        /// </summary>
        /// <param name="stageNum">ステージ番号</param>
        /// <param name="clearTime">クリアタイム</param>
        public void SetClearStageTime(int stageNum, float clearTime)
        {
            Debug.Log($"[GameSave]SetClearStageTime の保存:{stageNum} clearTime{clearTime}");
            PlayerPrefs.SetFloat($"{ClearStageTimeName}_{stageNum}", clearTime);
        }

        /// <summary>
        /// クリアタイムの取得
        /// </summary>
        /// <param name="stageNum">ステージ番号</param>
        /// <returns>クリアタイム</returns>
        public float GetClearStageTime(int stageNum)
        {
            var time = PlayerPrefs.GetFloat($"{ClearStageTimeName}_{stageNum}");
            Debug.Log($"[GameSave]SetClearStageTime の取得:{stageNum} time:{time}");
            return time;
        }

        /// <summary>
        /// クリアしたステージの保存
        /// ステージ1 クリアは番号 0 なので 1 加算する
        /// </summary>
        /// <param name="clearStageNum"></param>
        /// <returns></returns>
        public void SetClearStageNum(int clearStageNum)
        {
            var saveNum = PlayerPrefs.GetInt($"{ClearStageNumName}");
            var clearNum = clearStageNum + 1;

            // ステージ番号が大きければ更新する
            if (clearNum > saveNum)
            {
                Debug.Log($"[GameSave]SetClearStageNum の保存:{clearNum} num{saveNum}");
                PlayerPrefs.SetInt($"{ClearStageNumName}", clearNum);
                return;
            }

            Debug.Log($"[GameSave]SetClearStageNum の保存しない:{clearNum} num{saveNum}");
        }

        /// <summary>
        /// クリアしたステージの取得
        /// </summary>
        /// <returns>数字の大きいステージ番号</returns>
        public int GetClearStageNum()
        {
            var num = PlayerPrefs.GetInt($"{ClearStageNumName}");
            Debug.Log($"[GameSave]SetClearStageNum の取得:{num}");
            return num;
        }

        /// <summary>
        /// ゲーム実行前に呼び出す
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            var obj = Resources.Load<GameSaveDataModel>($"Prefabs/Common/GameSaveDataModel");
            obj = GameObject.Instantiate(obj);
            GameObject.DontDestroyOnLoad(obj);
        }
    }
}
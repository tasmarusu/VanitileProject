namespace VANITILE
{
    using UnityEngine;

    /// <summary>
    /// サウンドマネージャー
    /// </summary>
    public class SoundManager : SingletonMono<SoundManager>
    {
        /// <summary>
        /// BGMの種類
        /// </summary>
        [SerializeField] private AudioSource sourceBgm = null;

        /// <summary>
        /// SEの種類 10以上は考慮しない
        /// </summary>
        [SerializeField] private AudioSource[] sourceSes = null;

        /// <summary>
        /// BGM音量設定
        /// </summary>
        public float BgmVolume
        {
            get
            {
                return this.sourceBgm.volume;
            }

            set
            {
                if (value < .0f && value > 1.0f)
                {
                    Debug.LogError($"[Audio]BGM音量設定の幅を超えています value:{value}");
                    return;
                }

                this.sourceBgm.volume = value;
            }
        }

        /// <summary>
        /// SE音量設定
        /// </summary>
        public float SeVolume
        {
            get
            {
                return this.sourceSes[0].volume;
            }

            set
            {
                if (value < .0f && value > 1.0f)
                {
                    Debug.LogError($"[Audio]SE音量設定の幅を超えています value:{value}");
                    return;
                }

                foreach(var source in this.sourceSes)
                {
                    source.volume = value;
                }
            }
        }

        /// <summary>
        /// BGM再生
        /// </summary>
        /// <param name="type">DefineData.BgmType</param>
        public void PlayBgm(DefineData.BgmType type)
        {
            var clipName = Resources.Load<AudioClip>($"Sounds/Bgm/{type.ToString()}");
            this.sourceBgm.clip = (AudioClip)clipName;
            this.sourceBgm.Play();
        }

        /// <summary>
        /// BGM再生
        /// </summary>
        /// <param name="type">DefineData.SeType</param>
        public void PlaySe(DefineData.SeType type)
        {
            var clipName = Resources.Load<AudioClip>($"Sounds/Se/{type.ToString()}");

            // SE再生していない source を取得して再生
            foreach (var source in this.sourceSes)
            {
                if (source.isPlaying == false)
                {
                    source.clip = (AudioClip)clipName;
                    source.Play();
                    break;
                }
            }
        }

        /// <summary>
        /// SE音量 0~1
        /// </summary>
        /// <param name="value">音量</param>
        public void SetSeValue(float value)
        {
            if (value < .0f && value > 1.0f)
            {
                Debug.LogError($"[Audio]SE音量設定の幅を超えています value:{value}");
                return;
            }

            this.sourceBgm.volume = value;
        }

        /// <summary>
        /// ゲーム実行前に呼び出す
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            var obj = Resources.Load<SoundManager>($"Prefabs/Common/SoundManager");
            obj = GameObject.Instantiate(obj);
            GameObject.DontDestroyOnLoad(obj);
        }
    }
}
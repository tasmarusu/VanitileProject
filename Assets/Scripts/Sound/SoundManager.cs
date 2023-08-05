using UnityEngine;

namespace VANITILE
{
    /// <summary>
    /// サウンドマネージャー
    /// </summary>
    public class SoundManager : SingletonMono<SoundManager>
    {
        /// <summary>
        /// BGMの種類
        /// </summary>
        [SerializeField] private AudioSource bgmSource = null;

        /// <summary>
        /// BGMの種類
        /// </summary>
        [SerializeField] private AudioSource seSource = null;

        /// <summary>
        /// BGM音量設定
        /// </summary>
        public float BgmVolume
        {
            get
            {
                return bgmSource.volume;
            }
            set
            {
                if (value < .0f && value > 1.0f)
                {
                    Debug.LogError($"[Audio]BGM音量設定の幅を超えています value:{value}");
                    return;
                }

                bgmSource.volume = value;
            }
        }

        /// <summary>
        /// SE音量設定
        /// </summary>
        public float SeVolume
        {
            get
            {
                return seSource.volume;
            }
            set
            {
                if (value < .0f && value > 1.0f)
                {
                    Debug.LogError($"[Audio]SE音量設定の幅を超えています value:{value}");
                    return;
                }

                bgmSource.volume = value;
            }
        }

        /// <summary>
        /// ゲーム実行前に呼び出す
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init()
        {
            var obj = Resources.Load<SoundManager>($"Prefabs/Common/SoundManager");
            obj = GameObject.Instantiate(obj);
            GameObject.DontDestroyOnLoad(obj);
        }

        /// <summary>
        /// BGM再生
        /// </summary>
        /// <param name="type">DefineData.BgmType</param>
        public void PlayBgm(DefineData.BgmType type)
        {
            var clipName = Resources.Load<AudioClip>($"Sounds/Bgm/{type.ToString()}");
            bgmSource.clip = (AudioClip)clipName;
            bgmSource.Play();
        }

        /// <summary>
        /// BGM再生
        /// </summary>
        /// <param name="type">DefineData.SeType</param>
        public void PlaySe(DefineData.SeType type)
        {
            var clipName = Resources.Load<AudioClip>($"Sounds/Se/{type.ToString()}");
            seSource.clip = (AudioClip)clipName;
            seSource.Play();
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

            bgmSource.volume = value;
        }
    }
}
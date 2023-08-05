using System.Collections.Generic;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VANITILE
{
    /// <summary>
    /// タイトルで使用するオプション画面 
    /// </summary>
    public class OptionManager : TitleSelectBase
    {
        /// <summary>
        /// 纏めるオブジェクト
        /// </summary>
        [SerializeField] private GameObject rootObj;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField] private List<Selectable> selectables = new List<Selectable>();

        /// <summary>
        /// BGM音量
        /// </summary>
        [SerializeField] private AudioProperity bgmProperity = null;

        /// <summary>
        /// SE音量
        /// </summary>
        [SerializeField] private AudioProperity seProperity = null;

        /// <summary>
        /// 戻るボタン
        /// </summary>
        [SerializeField] private Button backButton = null;

        /// <summary>
        /// 初期化
        /// </summary>
        public override void Init()
        {
            TitleDataModel.Instance.PlayingState = DefineData.TitlePlayingState.Option;
            EventSystem.current.SetSelectedGameObject(this.selectables[0].gameObject);

            this.bgmProperity.Init();
            this.seProperity.Init();
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        /// <returns></returns>
        public override IEnumerator Finalize()
        {
            yield return new WaitUntil(() => TitleDataModel.Instance.IsTitleSelect);
            this.SaveVolume();
            GameObject.Destroy(this.gameObject);
        }

        /// <summary>
        /// 戻るボタン押下
        /// </summary>
        public void OnBackButton()
        {
            TitleDataModel.Instance.PlayingState = DefineData.TitlePlayingState.TitleSelect;
        }

        /// <summary>
        /// 音量のセーブ
        /// </summary>
        private void SaveVolume()
        {
            GameSaveDataModel.Instance.BgmVolume = SoundManager.Instance.BgmVolume;
            GameSaveDataModel.Instance.SeVolume = SoundManager.Instance.SeVolume;
        }

        /// <summary>
        /// 音量調整クラス
        /// </summary>
        [System.Serializable]
        public class AudioProperity
        {
            /// <summary>
            /// 音量調節スライダー
            /// </summary>
            [SerializeField] private Slider slider = null;

            /// <summary>
            /// 音量テキスト
            /// </summary>
            [SerializeField] private TMPro.TextMeshProUGUI volumeText = null;

            /// <summary>
            /// 音量テキスト
            /// </summary>
            [SerializeField] private AudioType audioType = AudioType.Bgm;

            /// <summary>
            /// 音量タイプ
            /// </summary>
            private enum AudioType
            {
                Bgm,
                Se,
            }

            /// <summary>
            /// 初期化
            /// </summary>
            public void Init()
            {
                var volume = .0f;
                switch (this.audioType)
                {
                    case AudioType.Bgm: volume = GameSaveDataModel.Instance.BgmVolume; break;
                    case AudioType.Se: volume = GameSaveDataModel.Instance.SeVolume; break;
                    default: Debug.LogError($"[Audio]はいるな"); break;
                }

                this.SetAudioVolume(volume);
                this.slider.value = volume;
                this.slider.onValueChanged.AddListener(SetAudioVolume);
            }

            /// <summary>
            /// 音量設定
            /// </summary>
            /// <param name="volume"></param>
            private void SetAudioVolume(float volume)
            {
                switch (this.audioType)
                {
                    case AudioType.Bgm: SoundManager.Instance.BgmVolume = volume; break;
                    case AudioType.Se: SoundManager.Instance.SeVolume = volume; break;
                    default: Debug.LogError($"[Audio]はいるな"); break;
                }

                this.volumeText.text = volume.ToString("F2");
            }
        }
    }
}
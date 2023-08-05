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
        /// 音量調整クラス
        /// </summary>
        [System.Serializable]
        public class AudioProperity
        {
            /// <summary>
            /// BGM音量調節スライダー
            /// </summary>
            [SerializeField] private Slider slider = null;

            /// <summary>
            /// BGM音量テキスト
            /// </summary>
            [SerializeField] private TMPro.TextMeshProUGUI volumeText = null;

            /// <summary>
            /// 初期化
            /// </summary>
            public void Init()
            {
                this.slider.onValueChanged.AddListener(SetAudioVolume);
            }

            /// <summary>
            /// 音量設定
            /// </summary>
            /// <param name="volume"></param>
            private void SetAudioVolume(float volume)
            {
                SoundManager.Instance.BgmVolume = volume;
                this.volumeText.text = volume.ToString("F2");
            }
        }
    }
}
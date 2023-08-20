namespace VANITILE
{
    using System.Collections;
    using System.Collections.Generic;
    using UniRx;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    /// <summary>
    /// タイトルで使用するオプション画面 
    /// </summary>
    public class OptionManager : TitleSelectBase, IPointerMoveHandler
    {
        /// <summary>
        /// 選択UI群
        /// </summary>
        [SerializeField] private List<Selectable> selectables = new List<Selectable>();

        /// <summary>
        /// BGM音量
        /// </summary>
        [SerializeField] private AudioProperity properityBgm = null;

        /// <summary>
        /// SE音量
        /// </summary>
        [SerializeField] private AudioProperity properitySe = null;

        /// <summary>
        /// 選択中の番号
        /// </summary>
        private int currentSelectNum = 0;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <returns> IEnumerator </returns>
        public override IEnumerator Init()
        {
            TitleDataModel.Instance.PlayingState = DefineData.TitlePlayingState.Option;
            this.properityBgm.Init();
            this.properitySe.Init();

            // 一個目を選択中
            this.selectables[this.currentSelectNum].Select();

            yield return this.In();

            // 操作開始
            this.StartButtonController();
            this.InputController();
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        /// <returns></returns>
        public override IEnumerator Finalize()
        {
            yield return new WaitUntil(() => TitleDataModel.Instance.IsTitleSelect);

            yield return this.Out();

            // 削除
            GameObject.Destroy(this.gameObject);
        }

        /// <summary>
        /// マウスがUI上に来る
        /// </summary>
        /// <param name="eventData"> PointerEventData </param>
        public void OnPointerMove(PointerEventData eventData)
        {
            var index = this.selectables.FindIndex(x => x.gameObject.GetHashCode() == eventData.pointerEnter.gameObject.GetHashCode());
            this.SelectPart(index);
        }

        /// <summary>
        /// 決定ボタン押下
        /// </summary>
        protected override void OnDecideButton()
        {
            this.SaveVolume();
            TitleDataModel.Instance.PlayingState = DefineData.TitlePlayingState.TitleSelect;
        }

        /// <summary>
        /// 戻るボタン押下
        /// </summary>
        protected override void OnBackButton()
        {
            TitleDataModel.Instance.PlayingState = DefineData.TitlePlayingState.TitleSelect;
        }

        /// <summary>
        /// 操作開始
        /// </summary>
        /// <returns></returns>
        private void InputController()
        {
            InputManager.Instance.VerticalOneSubject
                .TakeUntilDestroy(this)
                .Subscribe(value =>
                {
                    this.currentSelectNum -= value;
                    this.SelectPart(this.currentSelectNum);
                }).AddTo(this.ControllDisposables);
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
        /// 選択
        /// </summary>
        private void SelectPart(int num)
        {
            this.currentSelectNum = (int)Mathf.Repeat(num, this.selectables.Count);
            this.selectables[this.currentSelectNum].Select();
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
                    case AudioType.Bgm:
                        volume = GameSaveDataModel.Instance.BgmVolume;
                        break;

                    case AudioType.Se:
                        volume = GameSaveDataModel.Instance.SeVolume;
                        break;

                    default:
                        Debug.LogError($"[Audio]はいるな");
                        break;
                }

                this.SetAudioVolume(volume);
                this.slider.value = volume;
                this.slider.onValueChanged.AddListener(this.SetAudioVolume);
            }

            /// <summary>
            /// 音量設定
            /// </summary>
            /// <param name="volume"></param>
            private void SetAudioVolume(float volume)
            {
                switch (this.audioType)
                {
                    case AudioType.Bgm:
                        SoundManager.Instance.BgmVolume = volume;
                        break;

                    case AudioType.Se:
                        SoundManager.Instance.SeVolume = volume;
                        break;

                    default:
                        Debug.LogError($"[Audio]はいるな");
                        break;
                }

                this.volumeText.text = volume.ToString("F2");
            }
        }
    }
}
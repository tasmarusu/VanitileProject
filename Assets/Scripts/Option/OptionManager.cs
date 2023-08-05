using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.EventSystems;

namespace VANITILE
{
    /// <summary>
    /// タイトルで使用するオプション画面 
    /// </summary>
    public class OptionManager : MonoBehaviour
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
        /// BGM音量調節スライダー
        /// </summary>
        [SerializeField] private Slider bgmSlider = null;

        /// <summary>
        /// BGM音量テキスト
        /// </summary>
        [SerializeField] private TMPro.TextMeshProUGUI bgmVolumeText = null;

        /// <summary>
        /// SE音量調節スライダー
        /// </summary>
        [SerializeField] private Slider seSlider = null;

        /// <summary>
        /// SE音量テキスト
        /// </summary>
        [SerializeField] private TMPro.TextMeshProUGUI seVolumeText = null;

        /// <summary>
        /// 戻るボタン
        /// </summary>
        [SerializeField] private Button backButton = null;

        /// <summary>
        /// 選択中番号
        /// </summary>
        private int selectNum = 0;

        /// <summary>
        /// CompositeDisposable
        /// </summary>
        private CompositeDisposable disposables = new CompositeDisposable();

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
            TitleDataModel.Instance.PlayingState = DefineData.TitlePlayingState.Option;
            this.SetSoundValue();
            EventSystem.current.SetSelectedGameObject(this.selectables[0].gameObject);
            //this.UpdateSelectionStatus();

            // この辺稼働中メソッドのような物で共通参照出来る様にしてぇ
            // ボタン
            this.backButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    TitleDataModel.Instance.PlayingState = DefineData.TitlePlayingState.TitleSelect;
                    GameObject.Destroy(this.gameObject);
                }).AddTo(this);

            //// 選択状況の変更
            //InputManager.Instance.VerticalOneSubject
            //    .Subscribe(input =>
            //    {
            //        var tempSelectNum = this.selectNum;

            //        if (input > .0f)
            //        {
            //            this.selectNum = --this.selectNum < 0 ? this.selectables.Count - 1 : this.selectNum;
            //        }
            //        else if (input < .0f)
            //        {
            //            this.selectNum = ++this.selectNum > this.selectables.Count - 1 ? 0 : this.selectNum;
            //        }

            //        Debug.Log($"[Option]this.selectNum :{this.selectNum }");
            //        this.UpdateSelectionStatus();

            //    }).AddTo(this.disposables);
        }
        
        /// <summary>
        /// 戻るボタン押下
        /// </summary>
        public void OnBackButton()
        {
            TitleDataModel.Instance.PlayingState = DefineData.TitlePlayingState.TitleSelect;
            GameObject.Destroy(this.gameObject);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetSoundValue()
        {
            this.bgmSlider.value = SoundManager.Instance.BgmVolume;
            this.seSlider.value = SoundManager.Instance.SeVolume;

            this.bgmSlider.OnValueChangedAsObservable().Subscribe(value =>
            {
                var v = Mathf.Round(value * 10.0f);
                SoundManager.Instance.BgmVolume = v * 0.1f;
                this.bgmVolumeText.text = v.ToString();
            }).AddTo(this);

            this.seSlider.OnValueChangedAsObservable().Subscribe(value =>
            {
                var v = Mathf.Round(value * 10.0f);
                SoundManager.Instance.SeVolume = v * 0.1f;
                this.seVolumeText.text = v.ToString();
            }).AddTo(this);
        }

        /// <summary>
        /// 選択状況の更新
        /// </summary>
        private void UpdateSelectionStatus()
        {
            EventSystem.current.SetSelectedGameObject(this.selectables[this.selectNum].gameObject);
            Debug.Log($"[Option]:{EventSystem.current.currentSelectedGameObject.name}");
        }

        private void OnDestroy()
        {
            this.disposables.Clear();
        }

        /// <summary>
        /// オプション画面で使用するUI
        /// </summary>
        [System.Serializable]
        public class SelectableUI
        {
            /// <summary>
            /// 選択UI
            /// </summary>
            [field: SerializeField] public Selectable Selectable { get; private set; } = null;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VANITILE
{
    /// <summary>
    /// タイトルの選択画面
    /// </summary>
    public class TitleSelectController : TitleSelectBase
    {
        /// <summary>
        /// タイトルセレクトの各パーツ
        /// </summary>
        [SerializeField] private List<TitleSelectPart> titleSelectParts = new List<TitleSelectPart>();

        /// <summary>
        /// 選択中番号
        /// </summary>
        private int selectNum = 0;

        /// <summary>
        /// 入力状況
        /// </summary>
        private IDisposable disposable = null;

        /// <summary>
        /// 選択画面で決定ボタンを押下した時に流れる
        /// </summary>
        public Subject<DefineData.TitleSelectType> SelectSubject { get; private set; } = new Subject<DefineData.TitleSelectType>();

        /// <summary>
        /// 初期化
        /// </summary>
        public override void Init()
        {
            foreach(var part in this.titleSelectParts)
            {
                part.Init();
            }

            // 初期選択y
            EventSystem.current.SetSelectedGameObject(this.titleSelectParts[0].SelectButton.gameObject);

            this.UpdateSelectPart();
            this.InputController();
        }

        /// <summary>
        /// オプションボタン押下
        /// </summary>
        public void OnOptionButton()
        {
            this.SelectSubject.OnNext(this.titleSelectParts[3].SelectType);
        }

        /// <summary>
        /// 入力状況
        /// </summary>
        protected override void InputController()
        {
            // ボタン押下
            //Observable.EveryUpdate()
            //    .Where(x => InputManager.Instance.Decide)
            //    .Where(x => TitleDataModel.Instance.IsTitleSelect)
            //    .Subscribe(x =>
            //    {
            //        this.SelectSubject.OnNext(this.titleSelectParts[this.selectNum].SelectType);
            //    }).AddTo(this);

            // 上下入力監視
            var inputObservable = Observable.EveryUpdate()
                .Where(x => TitleDataModel.Instance.IsTitleSelect)
                .Select(_ => InputManager.Instance.Vertical)
                .Select(input => Mathf.Floor(input));

            //// 上下入力あり 
            //inputObservable.DistinctUntilChanged()
            //    .Where(input => input != .0f)
            //    .Subscribe(input =>
            //    {
            //        this.disposable?.Dispose();

            //        // 上下入力の監視
            //        this.disposable = Observable.Timer(System.TimeSpan.FromSeconds(0f), System.TimeSpan.FromSeconds(.25f))
            //            .Subscribe(_ =>
            //            {
            //                // 上
            //                if (input > 0)
            //                {
            //                    this.selectNum = --this.selectNum < 0 ? this.titleSelectParts.Count - 1 : this.selectNum;
            //                }
            //                else
            //                {
            //                    this.selectNum = ++this.selectNum >= this.titleSelectParts.Count ? 0 : this.selectNum;
            //                }

            //                this.UpdateSelectPart();

            //            }).AddTo(this);
            //    }).AddTo(this);

            // 入力無し
            inputObservable.DistinctUntilChanged()
                .Where(input => input == .0f)
                .Subscribe(input =>
                {
                    // 一応止める
                    this.disposable?.Dispose();
                }).AddTo(this);
        }

        /// <summary>
        /// セレクトの各パーツの更新
        /// </summary>
        private void UpdateSelectPart()
        {
            EventSystem.current.SetSelectedGameObject(this.titleSelectParts[this.selectNum].SelectButton.gameObject);
        }
    }
}
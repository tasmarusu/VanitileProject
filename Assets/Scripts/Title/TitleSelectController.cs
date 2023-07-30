using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace VANITILE
{
    /// <summary>
    /// タイトルの選択画面
    /// </summary>
    public class TitleSelectController : MonoBehaviour
    {
        /// <summary>
        /// タイトルの選択画面の種類
        /// </summary>
        public enum TitleSelectType
        {
            /// <summary>
            /// はじめから
            /// </summary>
            Start = 1,

            /// <summary>
            /// 続きから
            /// </summary>
            Continue = 5,

            /// <summary>
            /// ステージセレクト
            /// </summary>
            StageSelect = 10,

            /// <summary>
            /// 設定
            /// </summary>
            Option = 20,

            /// <summary>
            /// ゲーム終了
            /// </summary>
            Exit = 99,
        }

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
        public Subject<TitleSelectType> SelectSubject { get; private set; } = new Subject<TitleSelectType>();

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
            this.UpdateSelectPart();
            this.CheckInput();
        }

        /// <summary>
        /// 入力状況
        /// </summary>
        private void CheckInput()
        {
            // セレクト押下
            Observable.EveryUpdate()
                .Where(x => Input.GetButtonDown("Decide"))
                .Subscribe(x =>
                {
                    this.SelectSubject.OnNext(this.titleSelectParts[this.selectNum].SelectType);
                }).AddTo(this);

            // 上下入力監視
            var inputObservable = Observable.EveryUpdate()
                .Select(_ => Input.GetAxis("Vertical"))
                .Select(input => Mathf.Floor(input));

            // 上下入力あり 
            inputObservable.DistinctUntilChanged()
                .Where(input => input != .0f)
                .Subscribe(input =>
                {
                    this.disposable?.Dispose();

                    // 上下入力の監視
                    this.disposable = Observable.Timer(System.TimeSpan.FromSeconds(0f), System.TimeSpan.FromSeconds(.25f))
                        .Subscribe(_ =>
                        {
                            // 上
                            if (input > 0)
                            {
                                this.selectNum = --this.selectNum < 0 ? this.titleSelectParts.Count - 1 : this.selectNum;
                            }
                            else
                            {
                                this.selectNum = ++this.selectNum >= this.titleSelectParts.Count ? 0 : this.selectNum;
                            }

                            this.UpdateSelectPart();

                        }).AddTo(this);
                }).AddTo(this);

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
            // セレクトの各パーツの更新
            for (int i = 0; i < this.titleSelectParts.Count; i++)
            {
                if (i == this.selectNum)
                {
                    this.titleSelectParts[i].Select();
                }
                else
                {
                    this.titleSelectParts[i].Deselect();
                }
            }
        }
    }
}
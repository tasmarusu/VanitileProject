using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


namespace VANITILE
{
    /// <summary>
    /// ゲームプレイ中のゲーム進行タイム
    /// </summary>
    public class TimeManager 
    {
        /// <summary>
        /// タイム IDisposable
        /// </summary>
        private IDisposable timeDisposable = null;

        /// <summary>
        /// 経過時間
        /// </summary>
        public float processTimer { get; private set; } = .0f;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TimeManager()
        {
            this.processTimer = .0f;
            this.timeDisposable?.Dispose();

            this.timeDisposable = Observable.EveryUpdate()
                .Where(_ => StageDataModel.Instance.IsAbleMovePlayer())
                .Subscribe(_ =>
                {
                    this.processTimer += Time.deltaTime;
                });
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~TimeManager()
        {
            this.timeDisposable?.Dispose();
        }

        /// <summary>
        /// 時間停止
        /// </summary>
        public void Stop()
        {
            this.timeDisposable?.Dispose();
        }
    }
}
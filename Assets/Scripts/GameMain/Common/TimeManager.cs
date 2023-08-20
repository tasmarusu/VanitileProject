﻿using System;
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
        /// ゲーム開始時のベストタイム
        /// </summary>
        private float gameStartBestTime = .0f;

        /// <summary>
        /// 時間を文字列に変換
        /// </summary>
        /// <param name="clearTime"> クリア時間 </param>
        /// <returns> string </returns>
        public static string GetClearTimeStr(float clearTime)
        {
            var span = new TimeSpan(0, 0, (int)clearTime);
            var mmss = span.ToString(@"mm\:ss");
            var decima = Mathf.Floor((clearTime - Mathf.Floor(clearTime)) * 100);
            return $"{mmss}:{decima}";
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TimeManager()
        {
            var bestTime = GameSaveDataModel.Instance.GetClearStageTime(StageDataModel.Instance.CurrentStageId);
            this.gameStartBestTime = bestTime == .0f ? float.MaxValue : bestTime;
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

            // 記録がない ベストスコア更新した 場合上書きする
            var clearTime = GameSaveDataModel.Instance.GetClearStageTime(StageDataModel.Instance.CurrentStageId);
            if (clearTime == .0f || this.processTimer < clearTime)
            {
                GameSaveDataModel.Instance.SetClearStageTime(StageDataModel.Instance.CurrentStageId, this.processTimer);
            }
        }

        /// <summary>
        /// ベストタイムの更新が行われたか
        /// </summary>
        /// <returns></returns>
        public bool IsRewriteBestTime()
        {
            return this.processTimer - this.gameStartBestTime < 0;
        }
    }
}
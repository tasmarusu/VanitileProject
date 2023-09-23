namespace VANITILE
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 実行ステージの状況
    /// </summary>
    public class StageDataModel : Singleton<StageDataModel>
    {
        /// <summary>
        /// ゴール可能ブロック数
        /// </summary>
        private const int AbleBlockCount = 1;

        /// <summary>
        /// 現在のステート
        /// </summary>
        private MainGameState currentGameState = MainGameState.Before;

        /// <summary>
        /// メインゲームステート
        /// </summary>
        public enum MainGameState
        {
            /// <summary>
            /// 作動前
            /// </summary>
            Before = 1,

            /// <summary>
            /// 全鍵取得前
            /// </summary>
            NotAbleGoal = 10,

            /// <summary>
            /// 全鍵取得後
            /// </summary>
            AbleGoal = 20,

            /// <summary>
            /// クリア
            /// </summary>
            Clear = 30,

            /// <summary>
            /// 失敗
            /// </summary>
            Miss = 40,

            /// <summary>
            /// ポーズ
            /// </summary>
            Pose = 50,

            /// <summary>
            /// 遷移中
            /// </summary>
            Transition = 80,

            /// <summary>
            /// 作動後
            /// </summary>
            After = 99,
        }

        /// <summary>
        /// 現在実行中のステージ番号
        /// </summary>
        public int CurrentStageId { get; set; } = -1;

        /// <summary>
        /// 残り鍵数
        /// </summary>
        public int RemainKeyCount { get; private set; } = 0;

        /// <summary>
        /// 残りプレイヤーゴール数
        /// </summary>
        public int RemainPlayerCount { get; private set; } = 0;

        /// <summary>
        /// 残りブロック数
        /// </summary>
        public int RemainBlockCount { get; private set; } = 0;

        /// <summary>
        /// タイムマネージャー
        /// </summary>
        public TimeManager TimeMana { get; private set; } = null;

        /// <summary>
        /// 現在のゲームステート
        /// </summary>
        public MainGameState CurrentGameState
        {
            get
            {
                return this.currentGameState;
            }

            private set
            {
                this.BeforeGameState = this.currentGameState;
                this.currentGameState = value;
            }
        }

        /// <summary>
        /// 1個前のゲームステート
        /// </summary>
        public MainGameState BeforeGameState { get; private set; } = MainGameState.Before;

        /// <summary>
        /// ゴール可能か
        /// </summary>
        /// <returns></returns>
        public bool IsAbleGoal() => this.CurrentGameState == MainGameState.AbleGoal;

        /// <summary>
        /// ポーズ中
        /// </summary>
        /// <returns></returns>
        public bool IsPose() => this.CurrentGameState == MainGameState.Pose;

        /// <summary>
        /// プレイヤーの操作許可
        /// </summary>
        /// <returns></returns>
        public bool IsAbleMovePlayer()
        {
            var state = new List<MainGameState>() { MainGameState.NotAbleGoal, MainGameState.AbleGoal };
            return state.Contains(this.CurrentGameState);
        }

        /// <summary>
        /// ゲームプレイ中か
        /// </summary>
        /// <returns></returns>
        public bool IsGamePlaying()
        {
            var state = new List<MainGameState>() { MainGameState.NotAbleGoal, MainGameState.AbleGoal, MainGameState.Pose };
            return state.Contains(this.CurrentGameState);
        }

        /// <summary>
        /// 特定のステート中か
        /// </summary>
        /// <returns>bool</returns>
        public bool IsSpecificAction(List<MainGameState> actions)
        {
            return actions.Contains(this.CurrentGameState);
        }

        /// <summary>
        /// ゲーム開始
        /// TODO:ここにこれを書くことが間違っている可能性
        /// </summary>
        public void StartGame()
        {
            this.CurrentGameState = MainGameState.NotAbleGoal;
            this.TimeMana = null;
            this.TimeMana = new TimeManager();
        }

        /// <summary>
        /// プレイヤーの失敗
        /// </summary>
        public void MissPlayer()
        {
            Debug.Log($"[Player]Missしたので同ステージの再開");
            this.CurrentGameState = MainGameState.Miss;
            GameMain.Instance.GameMainTrans.TransitionCurrentStage();
        }

        /// <summary>
        /// プレイヤー数の設定
        /// </summary>
        /// <param name="keyCount"></param>
        public void SetPlayerCount(int playerCount)
        {
            this.RemainPlayerCount = playerCount;
        }

        /// <summary>
        /// プレイヤーがゴール
        /// </summary>
        public void GoalPlayer()
        {
            this.RemainPlayerCount--;

            // 全プレイヤーがゴールしたら遷移
            if (this.RemainPlayerCount <= AbleBlockCount - 1)
            {
                // ゴールステートに遷移
                this.CurrentGameState = MainGameState.Clear;

                // 時間を止めて取得
                this.TimeMana.Stop();
                GameSaveDataModel.Instance.SetClearStageNum(this.CurrentStageId);

                // クリアUIの表示
                GameMain.Instance.AppearClearUI();
            }
        }

        /// <summary>
        /// 残り鍵数の設定
        /// </summary>
        /// <param name="keyCount"></param>
        public void SetKeyCount(int keyCount)
        {
            this.RemainKeyCount = keyCount;
        }

        /// <summary>
        /// ブロック数の設定
        /// </summary>
        /// <param name="keyCount"></param>
        public void SetBlockCount(int blockCount)
        {
            this.RemainBlockCount = blockCount;
        }

        /// <summary>
        /// プレイヤーが鍵を取得
        /// </summary>
        public void GetPlayerKey()
        {
            this.RemainKeyCount--;
        }

        /// <summary>
        /// ブロックと接触
        /// </summary>
        public void TouchBlockInPlayer()
        {
            this.RemainBlockCount--;

            // 全ブロック破壊したらゴール可能へ
            this.CurrentGameState = this.RemainBlockCount <= AbleBlockCount ? MainGameState.AbleGoal : this.CurrentGameState;

            // ゴール可能になればSEならす。ここでいいのか
            if (this.RemainBlockCount <= AbleBlockCount)
            {
                SoundManager.Instance.PlaySe(DefineData.SeType.AbleGoal);
            }

            Debug.Log($"[]Block:{this.RemainBlockCount} State:{ this.CurrentGameState}");
        }

        /// <summary>
        /// ポーズ開始
        /// </summary>
        public void StartPoseMode()
        {
            if (this.CurrentGameState == MainGameState.Pose)
            {
                Debug.LogError($"[Pose]ポーズ中にポーズを呼ぶとは何事か :{this.CurrentGameState}");
            }

            this.CurrentGameState = MainGameState.Pose;
        }

        /// <summary>
        /// ポーズ終了
        /// </summary>
        public void EndPoseMode()
        {
            if (this.CurrentGameState != MainGameState.Pose)
            {
                Debug.LogError($"[Pose]ポーズ中じゃないのにポーズ終了を呼ぶとは何事か :{this.CurrentGameState}");
            }

            this.CurrentGameState = this.BeforeGameState;
        }

        /// <summary>
        /// クリアステートの終了 遷移に移動する
        /// </summary>
        public void SetEndClearState()
        {
            this.currentGameState = MainGameState.Transition;
        }
    }
}
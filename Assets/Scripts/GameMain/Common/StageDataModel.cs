using UnityEngine;

namespace VANITILE
{
    /// <summary>
    /// 実行ステージの状況
    /// </summary>
    public class StageDataModel : Singleton<StageDataModel>
    {
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
        /// 現在のゲームステート
        /// </summary>
        public MainGameState CurrentGameState { get; private set; } = MainGameState.Before;

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
            /// ゴール
            /// </summary>
            InGoal = 30,

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
        /// ゴール可能か
        /// </summary>
        /// <returns></returns>
        public bool IsAbleGoal()
        {
            return this.CurrentGameState == MainGameState.AbleGoal;
        }

        /// <summary>
        /// ゲーム開始
        /// TODO:ここにこれを書くことが間違っている可能性
        /// </summary>
        public void GameStart()
        {
            this.CurrentGameState = MainGameState.NotAbleGoal;
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

            // 全鍵取得したら遷移
            if (this.RemainPlayerCount <= 0)
            {
                GameMain.Instance.TransitionNextStage();
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
            this.CurrentGameState = this.RemainBlockCount <= 0 ? MainGameState.AbleGoal : this.CurrentGameState;

            // ゴール可能になればSEならす。ここでいいのか
            if (this.RemainBlockCount <= 0)
            {
                SoundManager.Instance.PlaySe(DefineData.SeType.AbleGoal);
            }

            Debug.Log($"[]Block:{this.RemainBlockCount} State:{ this.CurrentGameState}");
        }
    }
}
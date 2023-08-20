namespace VANITILE
{
    /// <summary>
    /// ステージの管理 StageManagerと名前被って分かり辛い
    /// Jsonでロードしたデータからプレイヤーやゴールの位置などを求める
    /// </summary>
    public class StageDataManager
    {
        /// <summary>
        /// ロードしたデータ
        /// </summary>
        public StageSaveData Data { get; private set; }

        /// <summary>
        /// セーブ
        /// </summary>
        public void Save(int stageId)
        {
        }

        /// <summary>
        /// 読み込み
        /// </summary>
        public void Load(int stageId)
        {
            ////var Model = new LoadStageModel(stageId);
        }

        /// <summary>
        /// ステージの初期情報
        /// </summary>
        public class LoadStageModel
        {
            /// <summary>
            /// ロード
            /// Jsonで保存したデータを取得
            /// TODO:saveはこのサイト真似て作る 
            /// </summary>
            /// <param name="stageId">ステージid</param>
            public LoadStageModel(int stageId)
            {
            }
        }
    }
}

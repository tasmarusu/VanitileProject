public class DefineData
{
    /// <summary>
    /// シーン名
    /// </summary>
    public enum SceneName
    {
        /// <summary>
        /// タイトルシーン
        /// </summary>
        TitleScene,

        /// <summary>
        /// ゲームシーン
        /// </summary>
        GameMainScene,
    }

    /// <summary>
    /// ステージで使用する種類
    /// </summary>
    [System.Serializable]
    public enum StagePartType
    {
        /// <summary>
        /// ブロック
        /// </summary>
        Block = 1,

        /// <summary>
        /// 鍵
        /// </summary>
        Key = 10,

        /// <summary>
        /// ゴール
        /// </summary>
        Goal = 20,

        /// <summary>
        /// プレイヤー
        /// </summary>
        Player = 90,
    }


    /// <summary>
    /// 判定タイプ
    /// </summary>
    [System.Serializable]
    public enum CollisionType
    {
        /// <summary>
        /// 地面判定
        /// </summary>
        Ground,

        /// <summary>
        /// 壁判定
        /// </summary>
        Wall
    }

    /// <summary>
    /// Bgmタイプ
    /// </summary>
    public enum BgmType
    {
        /// <summary>
        /// タイトル
        /// </summary>
        Title = 1,

        /// <summary>
        /// メイン
        /// </summary>
        Main = 5,
    }

    /// <summary>
    /// SE タイプ
    /// </summary>
    public enum SeType
    {
        /// <summary>
        /// プレイヤージャンプ
        /// </summary>
        Jump = 1,

        /// <summary>
        /// ブロック破壊
        /// </summary>
        BreakBlock = 10,

        /// <summary>
        /// ゴール可能
        /// </summary>
        AbleGoal = 20,

        /// <summary>
        /// ボタン押下
        /// </summary>
        Click = 50,

        /// <summary>
        /// 適当
        /// </summary>
        Other = 99,
    }

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
    /// 実行中の状態
    /// </summary>
    public enum TitlePlayingState
    {
        /// <summary>
        /// タイトルの選択画面
        /// </summary>
        TitleSelect,

        /// <summary>
        /// ステージセレクト
        /// </summary>
        StageSelect,

        /// <summary>
        /// オプション
        /// </summary>
        Option,
    }

    /// <summary>
    /// ロードするステージオブジェクトの種類
    /// </summary>
    public enum ResourcesStageType
    {

    }

    /// <summary>
    /// ロードするコモンオブジェクトの種類
    /// </summary>
    public enum ResourcesCommonType
    {

    }
}

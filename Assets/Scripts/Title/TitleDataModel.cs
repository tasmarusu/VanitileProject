namespace VANITILE
{
    /// <summary>
    /// タイトルの状況
    /// </summary>
    public class TitleDataModel : Singleton<TitleDataModel>
    {
        /// <summary>
        /// 実行中の状態
        /// </summary>
        public DefineData.TitlePlayingState PlayingState { get; set; } = DefineData.TitlePlayingState.TitleSelect;

        /// <summary>
        /// タイトルの選択画面
        /// </summary>
        /// <returns></returns>
        public bool IsTitleSelect => this.PlayingState == DefineData.TitlePlayingState.TitleSelect;

        /// <summary>
        /// ステージセレクトの画面中か
        /// </summary>
        /// <returns></returns>
        public bool IsStageSelect => this.PlayingState == DefineData.TitlePlayingState.StageSelect;

        /// <summary>
        /// オプションの画面中か
        /// </summary>
        /// <returns></returns>
        public bool IsOption => this.PlayingState == DefineData.TitlePlayingState.Option;
    }
}
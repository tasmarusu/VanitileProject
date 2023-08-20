namespace VANITILE
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// タイトルセレクトの各パーツ
    /// TODO:いらないかも知れないぞ
    /// </summary>
    [System.Serializable]
    public class TitleSelectPart
    {
        /// <summary>
        /// ボタンの文字
        /// </summary>
        [SerializeField] private string word = string.Empty;

        /// <summary>
        /// 選択中のボタン
        /// </summary>
        [field: SerializeField] public Button SelectButton { get; private set; } = null;

        /// <summary>
        /// タイトル画面の選択画面
        /// </summary>
        [field: SerializeField] public DefineData.TitleSelectType SelectType { get; private set; }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
            this.SelectButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"{this.word}";
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using static VANITILE.TitleSelectController;

namespace VANITILE
{
    /// <summary>
    /// タイトルセレクトの各パーツ
    /// </summary>
    public class TitleSelectPart : MonoBehaviour
    {
        /// <summary>
        /// 選択中の画像
        /// </summary>
        [SerializeField] private Image selectImage = null;

        /// <summary>
        /// タイトル画面の選択画面
        /// </summary>
        [field:SerializeField] public TitleSelectType SelectType { get; private set; }

        /// <summary>
        /// 選択する
        /// </summary>
        public void Select()
        {
            this.selectImage.enabled = true;
        }

        /// <summary>
        /// 選択解除
        /// </summary>
        public void Deselect()
        {
            this.selectImage.enabled = false;
        }
    }
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VANITILE
{
    /// <summary>
    /// ステージ番号の各
    /// </summary>
    public class StageNumberPart : MonoBehaviour
    {
        /// <summary>
        /// テキスト
        /// </summary>
        [SerializeField, Header("テキスト")] private TextMeshProUGUI text = null;

        /// <summary>
        /// Button
        /// </summary>
        [SerializeField, Header("Button")] private Button button = null;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public bool IsPlayingUpDownAnimation
        //{
        //    get
        //    {
        //        var info = this.animator.GetCurrentAnimatorStateInfo(0);
        //        var playUp = info.IsName(AnimatorName.Up.ToString());
        //        var playDown = info.IsName(AnimatorName.Down.ToString());
        //        var time = info.normalizedTime < 1.0f;
        //        return (playUp || playDown) && time;
        //    }
        //}

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
        }

        /// <summary>
        /// ステージ番号の設定
        /// Animationでも呼ぶ想定
        /// </summary>
        public void UpdateStageText(string str)
        {
            this.text.text = $"{str}";
        }

        /// <summary>
        /// 選択中
        /// </summary>
        public void Selected()
        {
            this.text.color = Color.red;
            this.button.Select();
        }

        /// <summary>
        /// 選択解除
        /// </summary>
        public void Deselect()
        {
            this.text.color = Color.white;
        }
    }
}
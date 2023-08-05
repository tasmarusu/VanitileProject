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
        /// AnimatorName
        /// </summary>
        private enum AnimatorName
        {
            /// <summary>
            /// 上
            /// </summary>
            Up,

            /// <summary>
            /// 下
            /// </summary>
            Down,

            /// <summary>
            /// 選択中
            /// </summary>
            Select,
        }

        /// <summary>
        /// テキスト
        /// </summary>
        [SerializeField, Header("テキスト")] private TextMeshProUGUI text = null;

        /// <summary>
        /// ステージの画像イメージ
        /// </summary>
        [SerializeField, Header("ステージの画像イメージ")] private Image stageImage = null;

        /// <summary>
        /// Animator
        /// </summary>
        private Animator animator = null;

        /// <summary>
        /// 表示するステージ番号
        /// </summary>
        private string stageNumStr = "";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsPlayingUpDownAnimation
        {
            get
            {
                var info = this.animator.GetCurrentAnimatorStateInfo(0);
                var playUp = info.IsName(AnimatorName.Up.ToString());
                var playDown = info.IsName(AnimatorName.Down.ToString());
                var time = info.normalizedTime < 1.0f;
                return (playUp || playDown) && time;
            }
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
            this.animator = this.GetComponent<Animator>();
        }

        /// <summary>
        /// ステージ番号の設定
        /// Animationでも呼ぶ想定
        /// </summary>
        public void UpdateStageText()
        {
            this.text.text = $"{this.stageNumStr}";
        }

        /// <summary>
        /// ステージ番号の設定
        /// </summary>
        public void SetStageNumStr(string str)
        {
            this.stageNumStr = str;
        }

        /// <summary>
        /// 上方向のアニメーション開始
        /// </summary>
        public void PlayUpAnimation()
        {
            this.animator.SetTrigger(AnimatorName.Up.ToString());
        }

        /// <summary>
        /// 下方向のアニメーション開始
        /// </summary>
        public void PlayDownAnimation()
        {
            this.animator.SetTrigger(AnimatorName.Down.ToString());
        }

        /// <summary>
        /// 選択中
        /// </summary>
        public void Selected()
        {
            this.PlaySelectedAnimation();
            this.animator.SetBool(AnimatorName.Select.ToString(), true);
            this.text.color = Color.red;
        }

        /// <summary>
        /// 選択解除
        /// </summary>
        public void Deselect()
        {
            this.text.color = Color.white;
            this.animator.SetBool(AnimatorName.Select.ToString(), false);
            //this.text.transform.localScale = this.initScale;
        }

        /// <summary>
        /// 選択中アニメーションの再生
        /// </summary>
        private void PlaySelectedAnimation()
        {
            //this.text.transform.DOPunchScale(Vector3.one * 2.0f, 0.5f);
        }
    }
}
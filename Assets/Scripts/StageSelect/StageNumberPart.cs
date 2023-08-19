using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
        /// テキストの元の大きさ
        /// </summary>
        private Vector3 originTextScale;

        /// <summary>
        /// 拡縮変調Tween
        /// </summary>
        private Tween boundTween = null;

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
            this.originTextScale = this.text.transform.localScale;
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
            this.boundTween = this.text.transform
                 .DOScale(new Vector3(1.5f, 1.5f, 1f), 0.3f)
                 .SetLoops(int.MaxValue, LoopType.Yoyo)
                 .SetEase(Ease.OutCirc)
                 .OnKill(() =>
                 {
                     this.text.transform.localScale = this.originTextScale;
                 });
        }

        /// <summary>
        /// 選択解除
        /// </summary>
        public void Deselect()
        {
            this.text.color = Color.white;
            this.boundTween.Kill();
            this.text.transform.localScale = this.originTextScale;
        }
    }
}
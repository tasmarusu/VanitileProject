using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace VANITILE
{
    /// <summary>
    /// ステージ遷移
    /// Commonシーンに入れるべき
    /// </summary>
    [ExecuteAlways]
    [ExecuteInEditMode] // Animationでシェーダー動かしたい
    public class Transition : MonoBehaviour
    {
        /// <summary>
        /// Image
        /// </summary>
        [SerializeField] private Image image = null;

        /// <summary>
        /// 透過度
        /// </summary>
        [SerializeField, HideInInspector] private float threshold = .0f;

        /// <summary>
        /// 遷移中のオブジェクト
        /// </summary>
        [SerializeField] private Animator animator = null;

        /// <summary>
        /// 開始
        /// </summary>
        /// <returns></returns>
        public IEnumerator In()
        {
            this.threshold = 1f;
            this.animator.Play("In", 0, .0f);
            yield return null;
            yield return new WaitUntil(() => this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
            Debug.Log($"[Transition]In 終了");
        }

        /// <summary>
        /// 終了
        /// 開始していなければ即終了
        /// </summary>
        /// <returns></returns>
        public IEnumerator Out()
        {
            this.threshold = 0f;
            this.animator.Play("Out", 0, .0f);
            yield return null;
            yield return new WaitUntil(() => this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
            Debug.Log($"[Transition]Out 終了");
        }

        private void Update()
        {
            // this.imageだと取って来てくれないから Get してる
            this.GetComponentInChildren<Image>().material.SetFloat("_Threshold", this.threshold);
        }
    }
}

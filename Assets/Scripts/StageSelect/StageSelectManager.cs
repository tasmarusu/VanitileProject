using UnityEngine;

namespace VANITILE
{
    /// <summary>
    /// ステージセレクトマネージャー
    /// </summary>
    public class StageSelectManager : MonoBehaviour
    {
        /// <summary>
        /// テキスト
        /// </summary>
        [SerializeField, Header("テキスト")] private StageNumberController stageNumberController = null;

        /// <summary>
        /// 初期化
        /// </summary>
        private void Start()
        {
            this.stageNumberController.Init();
        }
    }
}
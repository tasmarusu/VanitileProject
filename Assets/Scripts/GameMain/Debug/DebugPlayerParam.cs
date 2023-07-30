using TMPro;
using UnityEngine;


namespace VANITILE
{
    public class DebugPlayerParam : MonoBehaviour
    {
        /// <summary>
        /// ステート親
        /// </summary>
        [SerializeField, Header("ステート親")] private Transform movementTextTr;

        /// <summary>
        /// ステート
        /// </summary>
        [SerializeField, Header("ステート")] private TextMeshProUGUI movementText;

        /// <summary>
        /// ステート
        /// </summary>
        [SerializeField, Header("上ジャンプ有無")] private TextMeshProUGUI isJumpText;

        /// <summary>
        /// ステート
        /// </summary>
        [SerializeField, Header("壁ジャンプ有無")] private TextMeshProUGUI isWallText;

        /// <summary>
        /// ステート
        /// </summary>
        [SerializeField, Header("壁ジャンプ速度")] private TextMeshProUGUI walljumpPowerText;

        /// <summary>
        /// ステート
        /// </summary>
        [SerializeField, Header("移動速度")] private TextMeshProUGUI speedText;

        /// <summary>
        /// 行動保存
        /// </summary>
        //private PlayerController.MovementState movementTemp = PlayerController.MovementState.Wait;

        void Update()
        {
            //if(this.movementTemp != PlayerController.Instance.Movement)
            //{
            //    this.movementTemp = PlayerController.Instance.Movement;
            //    var prefab = GameObject.Instantiate(this.movementText.gameObject, this.movementTextTr);
            //    prefab.GetComponent<TextMeshProUGUI>().text = $"State:{PlayerController.Instance.Movement.ToString()} {Time.frameCount}";
            //    GameObject.Destroy(prefab, 5.0f);
            //}


            this.isJumpText.text = $"RemKey:{StageDataModel.Instance.RemainKeyCount.ToString()}";
            this.speedText.text = $"RemPla:{StageDataModel.Instance.RemainPlayerCount.ToString()}";
            this.isWallText.text = $"RemBlo:{StageDataModel.Instance.RemainBlockCount.ToString()}";
            this.walljumpPowerText.text = $"State:{StageDataModel.Instance.CurrentGameState.ToString()}";
        }
    }
}
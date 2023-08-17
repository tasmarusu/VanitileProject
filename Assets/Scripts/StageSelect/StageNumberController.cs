using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace VANITILE
{
    /// <summary>
    /// ステージの番号選択画面
    /// </summary>
    public class StageNumberController : MonoBehaviour
    {
        /// <summary>
        /// 選択中の番号
        /// </summary>
        [SerializeField, Header("選択中表示の番号")] private List<StageNumberPart> stageNumberParts = new List<StageNumberPart>();

        /// <summary>
        /// デバッグ用
        /// </summary>
        [SerializeField, Header("選択中番号最大値")] private int selectMax = 0;

        /// <summary>
        /// 選択中の番号
        /// </summary>
        private int currentStageNum = 0;

        /// <summary>
        /// セレクトスピードタイプ
        /// </summary>
        [SerializeField, Header("セレクトスピードタイプ")] private List<SelectInputType> selectInputTypes = new List<SelectInputType>();

        /// <summary>
        /// Coroutine
        /// </summary>
        private Coroutine selectCoroutine;

        /// <summary>
        /// Coroutine
        /// </summary>
        private Coroutine setStageNumCoroutine;

        /// <summary>
        /// セレクトスピードタイプ
        /// </summary>
        [System.Serializable]
        private class SelectInputType
        {
            /// <summary>
            /// 回数
            /// </summary>
            [field: SerializeField] public int count { get; private set; }

            /// <summary>
            /// スピード
            /// </summary>
            [field: SerializeField] public float speed { get; private set; }
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
            Debug.Assert(this.selectInputTypes.Count != 0, $"[Select]SelectSpeedType が設定されていません");

            this.InputControll();

            this.SetPartsStageNum();

            for (int i = 0; i < this.stageNumberParts.Count; i++)
            {
                this.stageNumberParts[i].Init();
            }

            this.stageNumberParts[0].Selected();
        }

        /// <summary>
        /// 上下操作
        /// </summary>
        private void InputControll()
        {

        }

        /// <summary>
        /// 各パーツにステージ番号を設定する
        /// </summary>
        private void SetPartsStageNum()
        {
            for (int i = 0; i < this.stageNumberParts.Count; i++)
            {
                // -1以下か
                //var stageNum = this.currentStageNum - this.selectingNum + i;
                //var str = (stageNum >= 0 && stageNum <= this.selectMax) ? $"{stageNum + 1}" : "";

                //this.stageNumberParts[i].SetStageNumStr(str);
                //this.stageNumberParts[i].UpdateStageText();
            }
        }
    }
}
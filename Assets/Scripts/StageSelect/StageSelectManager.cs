using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VANITILE
{
    /// <summary>
    /// ステージセレクトマネージャー
    /// </summary>
    public class StageSelectManager : TitleSelectBase, IPointerMoveHandler
    {
        /// <summary>
        /// テキスト
        /// </summary>
        [SerializeField, Header("テキスト")] private StageNumberController stageNumberController = null;

        /// <summary>
        /// 選択UI群
        /// </summary>
        [SerializeField] private List<Selectable> selectables = new List<Selectable>();

        /// <summary>
        /// 選択中の番号
        /// </summary>
        private int currentSelectNum = 0;

        /// <summary>
        /// 事後処理
        /// </summary>
        /// <returns></returns>
        public override IEnumerator Finalize()
        {
            yield return new WaitUntil(() => TitleDataModel.Instance.IsStageSelect);
            GameObject.Destroy(this.gameObject);
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public override void Init()
        {
            Debug.Log($"[StageSelectManager]Init");
            TitleDataModel.Instance.PlayingState = DefineData.TitlePlayingState.StageSelect;
            this.Animator();
        }

        /// <summary>
        /// マウスがUI上に来る
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerMove(PointerEventData eventData)
        {
            var index = this.selectables.FindIndex(x => x.gameObject.GetHashCode() == eventData.pointerEnter.gameObject.GetHashCode());
            this.SelectPart(index);
        }

        /// <summary>
        /// 選択
        /// </summary>
        private void SelectPart(int num)
        {
            this.currentSelectNum = (int)Mathf.Repeat(num, this.selectables.Count);
            this.selectables[this.currentSelectNum].Select();
        }

        /// <summary>
        /// Animator
        /// </summary>
        private void Animator()
        {

        }
    }
}
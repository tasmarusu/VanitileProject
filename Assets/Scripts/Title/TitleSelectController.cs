
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using static DefineData;

namespace VANITILE
{
    /// <summary>
    /// タイトルの選択画面
    /// </summary>
    public class TitleSelectController : MonoBehaviour
    {
        /// <summary>
        /// タイトルセレクトの各パーツ
        /// </summary>
        [SerializeField] private List<TitleSelectPart> titleSelectParts = new List<TitleSelectPart>();

        /// <summary>
        /// 選択画面で決定ボタンを押下した時に流れる
        /// </summary>
        public Subject<TitleSelectType> SelectSubject { get; private set; } = new Subject<TitleSelectType>();

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
            foreach (var part in this.titleSelectParts)
            {
                part.Init();
            }

            // 初期選択
            this.SetEventSelectedState(TitleSelectType.Start);
        }

        /// <summary>
        /// UI選択状態の設定
        /// </summary>
        /// <param name="state">TitlePlayingState</param>
        public void SetEventSelectedState(TitleSelectType type)
        {
            EventSystem.current.SetSelectedGameObject(this.titleSelectParts.Find(x => x.SelectType == type).SelectButton.gameObject);
        }

        /// <summary>
        /// オプションボタン押下
        /// </summary>
        /// <param name="type"></param>
        public void OnButton(int type)
        {
            this.SelectSubject.OnNext(this.titleSelectParts.Find(x => x.SelectType == (DefineData.TitleSelectType)type).SelectType);
        }
    }
}
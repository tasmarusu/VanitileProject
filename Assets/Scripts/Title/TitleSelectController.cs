namespace VANITILE
{
    using System.Collections.Generic;
    using UniRx;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using static DefineData;

    /// <summary>
    /// タイトルの選択画面
    /// </summary>
    public class TitleSelectController : MonoBehaviour, IPointerMoveHandler, IPointerClickHandler
    {
        /// <summary>
        /// タイトルセレクトの各パーツ
        /// </summary>
        [SerializeField] private List<TitleSelectPart> titleSelectParts = new List<TitleSelectPart>();

        /// <summary>
        /// 選択中の番号
        /// </summary>
        private int currentSelectNum = 0;

        /// <summary>
        /// CompositeDisposable
        /// </summary>
        private CompositeDisposable disposables = new CompositeDisposable();

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

            this.RestartInput();
        }

        /// <summary>
        /// インプットの再スタート
        /// </summary>
        public void RestartInput()
        {
            TitleDataModel.Instance.PlayingState = DefineData.TitlePlayingState.TitleSelect;

            // 初期選択
            this.titleSelectParts[this.currentSelectNum].SelectButton.Select();

            // 操作
            this.InputController();
        }

        /// <summary>
        /// マウスがUI上に来る
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerMove(PointerEventData eventData)
        {
            var index = this.titleSelectParts.FindIndex(x => x.SelectButton.gameObject.GetHashCode() == eventData.pointerEnter.gameObject.GetHashCode());
            this.SelectPart(index);
        }

        /// <summary>
        /// クリック
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            var index = this.titleSelectParts.FindIndex(x => x.SelectButton.gameObject.GetHashCode() == eventData.pointerEnter.gameObject.GetHashCode());
            this.SelectPart(index);
            this.Dedide();
        }

        /// <summary>
        /// 操作
        /// </summary>
        private void InputController()
        {
            // 決定ボタン押下
            InputManager.Instance.ObserveEveryValueChanged(x => x.Decide)
                .TakeUntilDestroy(this)
                .Where(x => x)
                .Where(_ => TitleDataModel.Instance.IsTitleSelect)
                .Subscribe(_ =>
                {
                    this.Dedide();
                }).AddTo(this.disposables);

            // 上下移動
            InputManager.Instance.VerticalOneSubject
                .TakeUntilDestroy(this)
                .Where(_ => TitleDataModel.Instance.IsTitleSelect)
                .Subscribe(value =>
                {
                    this.currentSelectNum -= value;
                    this.SelectPart(this.currentSelectNum);
                }).AddTo(this.disposables);
        }

        /// <summary>
        /// 決定
        /// </summary>
        private void Dedide()
        {
            this.disposables?.Clear();
            this.SelectSubject.OnNext(this.titleSelectParts[this.currentSelectNum].SelectType);
        }

        /// <summary>
        /// 選択
        /// </summary>
        private void SelectPart(int num)
        {
            this.currentSelectNum = (int)Mathf.Repeat(num, this.titleSelectParts.Count);
            this.titleSelectParts[this.currentSelectNum].SelectButton.Select();
        }
    }
}
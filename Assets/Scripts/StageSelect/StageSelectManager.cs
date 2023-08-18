using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using static DefineData;
using System.Linq;
using System;

namespace VANITILE
{
    /// <summary>
    /// ステージセレクトマネージャー
    /// </summary>
    public class StageSelectManager : TitleSelectBase, IPointerClickHandler
    {
        /// <summary>
        /// 使用するステージ情報
        /// </summary>
        [SerializeField, Header("使用するステージ情報")] private StageTransitionScriptable stageTransitionData = null;

        /// <summary>
        /// 選択UI群
        /// </summary>
        [SerializeField, Header("選択UI群")] private List<StageNumberPart> stageNumberParts = null;

        /// <summary>
        /// 選択する中心番号
        /// </summary>
        [SerializeField] private int centerSelectNum = 3;

        /// <summary>
        /// 選択する中心番号のオブジェクト
        /// </summary>
        private StageNumberPart centerSelectPart = null;

        /// <summary>
        /// 選択中の番号
        /// </summary>
        private int currentSelectNum = 0;

        /// <summary>
        /// 移動時間
        /// </summary>
        private float moveTime = .1f;

        /// <summary>
        /// 操作可能か
        /// </summary>
        private bool isControll = false;

        /// <summary>
        /// CompositeDisposable
        /// </summary>
        private CompositeDisposable disposables = new CompositeDisposable();

        /// <summary>
        /// 初期化
        /// </summary>
        public override IEnumerator Init()
        {
            Debug.Log($"[StageSelectManager]Init");

            TitleDataModel.Instance.PlayingState = DefineData.TitlePlayingState.StageSelect;

            // 各ステージ番号パーツの初期化
            foreach(var part in this.stageNumberParts)
            {
                part.Init();
            }
            this.IsSelectPart(this.currentSelectNum);
            this.UpdateNumberPart();

            // ポップアップ開始
            yield return this.In();

            // 操作開始
            this.InputController();

            // 選択中のパーツを選定し、選択状態にする
            this.centerSelectPart = this.stageNumberParts[this.centerSelectNum];
            this.centerSelectPart.Selected();
        }

        /// <summary>
        /// 事後処理
        /// </summary>
        /// <returns></returns>
        public override IEnumerator Finalize()
        {
            // ポップアップ消えるまで待機
            yield return new WaitUntil(() => TitleDataModel.Instance.IsTitleSelect);

            // 操作削除して閉じる
            this.disposables.Clear();
            yield return this.Out();

            // 完全削除
            GameObject.Destroy(this.gameObject);
        }

        ///// <summary>
        ///// マウスがUI上に来る
        ///// </summary>
        ///// <param name="eventData"></param>
        //public void OnPointerMove(PointerEventData eventData)
        //{
        //    var index = this.selectables.FindIndex(x => x.gameObject.GetHashCode() == eventData.pointerEnter.gameObject.GetHashCode());
        //    this.SelectPart(index);
        //}

        /// <summary>
        /// クリック
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            StageDataModel.Instance.CurrentStageId = GameSaveDataModel.Instance.PlayLastStageId;
            SceneManager.LoadScene(SceneName.GameMainScene.ToString());
        }

        /// <summary>
        /// ステージ番号の更新
        /// </summary>
        private void UpdateNumberPart()
        {
            for (int i = 0; i < this.stageNumberParts.Count; i++)
            {
                var str = "";
                var num = this.currentSelectNum - this.centerSelectNum + i;

                if (num >= 0 && num < this.stageTransitionData.StageDatas.Count)
                {
                    // 0 始めなので 1 足す
                    str = (num + 1).ToString();
                }

                this.stageNumberParts[i].UpdateStageText(str);
            }
        }

        /// <summary>
        /// 操作開始
        /// </summary>
        /// <returns></returns>
        private void InputController()
        {
            this.isControll = true;

            // 決定ボタン押下
            InputManager.Instance.ObserveEveryValueChanged(x => x.Decide)
                .Where(x => x)
                .Where(_ => this.isControll)
                .Where(_ => TitleDataModel.Instance.IsStageSelect)
                .Subscribe(_ =>
                {
                    TitleDataModel.Instance.PlayingState = DefineData.TitlePlayingState.TitleSelect;

                    //StageDataModel.Instance.CurrentStageId = this.currentSelectNum;
                    //SceneManager.LoadScene(SceneName.GameMainScene.ToString());
                }).AddTo(this.disposables);

            // 上下移動
            InputManager.Instance.VerticalOneSubject
                .Where(_ => this.isControll)
                .Do(value => this.currentSelectNum -= value)
                .Where(x => this.IsSelectPart(this.currentSelectNum) == false) // 数値が許容範囲なら移動開始
                .Subscribe(value =>
                {
                    this.isControll = false;
                    this.centerSelectPart.Deselect();

                    // 移動開始
                    Observable.FromCoroutine<Unit>(observer => this.MovePart(value, observer))
                        .TakeUntilDestroy(this)
                        .Subscribe(x =>
                        {
                            this.isControll = true;
                            this.centerSelectPart.Selected();
                            this.UpdateNumberPart();
                        });

                }).AddTo(this.disposables);
        }

        /// <summary>
        /// 選択
        /// </summary>
        private bool IsSelectPart(int num)
        {
            this.currentSelectNum = Mathf.Clamp(num, 0, this.stageTransitionData.StageDatas.Count - 1);
            return num < 0 || num > this.stageTransitionData.StageDatas.Count - 1;
        }

        /// <summary>
        /// ステージ番号の移動
        /// </summary>
        private IEnumerator MovePart(int value,IObserver<Unit> observer)
        {
            // Lerp で移動開始
            var poses = this.stageNumberParts.Select(x => x.transform.position).ToArray();
            var timer = .0f;
            while (timer < 1.0f)
            {
                timer += Time.deltaTime / moveTime;
                for (int i = 1; i < this.stageNumberParts.Count - 1; i++)
                {
                    var p = this.stageNumberParts[i].transform.position;
                    this.stageNumberParts[i].transform.position = Vector3.Lerp(poses[i], poses[i + value], timer);
                }

                yield return null;
            }

            // 位置を元に戻す
            for (int i = 0; i < this.stageNumberParts.Count; i++)
            {
                this.stageNumberParts[i].transform.position = poses[i];
            }

            // 終了通知
            observer.OnNext(Unit.Default);
            observer.OnCompleted();
        }
    }
}
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
        /// 選択中の番号 stageNumberParts の真ん中？
        /// </summary>
        [SerializeField, Header("選択中表示の番号")] private int selectingNum = 0;

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



            this.currentStageNum = 0;
            this.SetPartsStageNum();

            for (int i = 0; i < this.stageNumberParts.Count; i++)
            {
                this.stageNumberParts[i].Init();
                if (i == this.selectingNum)
                {
                    this.stageNumberParts[i].Selected();
                }
            }
        }

        /// <summary>
        /// 上下操作
        /// </summary>
        private void InputControll()
        {
            // 上下入力の監視
            var inputObservable = Observable.EveryUpdate()
                .Select(_ => InputManager.Instance.Horizontal)
                .Select(input => Mathf.Floor(input));

            // 値変化時の初回
            inputObservable.DistinctUntilChanged()
                .Subscribe(input =>
                {
                    if (this.selectCoroutine != null)
                    {
                        this.StopCoroutine(this.selectCoroutine);
                    }

                    // 入力無しなら移動無しでコルーチンを止めるだけ
                    if (input == .0f)
                    {
                        return;
                    }

                    // セレクト操作開始
                    this.selectCoroutine = this.StartCoroutine(this.StartControll(input > .0f));
                }).AddTo(this);
        }

        /// <summary>
        /// セレクト操作開始
        /// 入力時間が長いとその分長くなる
        /// </summary>
        private IEnumerator StartControll(bool isUp)
        {
            Debug.Log($"[Select]開始");

            var num = 0;
            var count = 0;
            var timer = this.selectInputTypes[0].speed;
            while (true)
            {
                timer += Time.deltaTime;
                if (timer >= this.selectInputTypes[num].speed)
                {
                    timer = .0f;

                    // 入力方向による上下移動
                    for (int i = 0; i < this.stageNumberParts.Count; i++)
                    {
                        if (isUp == true)
                        {
                            this.stageNumberParts[i].PlayUpAnimation();
                        }
                        else
                        {
                            this.stageNumberParts[i].PlayDownAnimation();
                        }

                        // 選択中の番号
                        if (i == this.selectingNum)
                        {
                            this.stageNumberParts[i].Selected();
                            continue;
                        }
                    }

                    // ステージの番号の前後へ移動
                    this.currentStageNum += isUp ? -1 : 1;
                    this.currentStageNum = Mathf.Clamp(this.currentStageNum, 0, this.selectMax);

                    if (this.setStageNumCoroutine != null)
                    {
                        // ステージ番号の反映
                        Debug.Log($"[Select]ステージ番号の反映1 :{this.currentStageNum}");
                        this.SetPartsStageNum();
                        this.StopCoroutine(this.setStageNumCoroutine);
                    }
                    this.setStageNumCoroutine = this.StartCoroutine(this.SetPartStageNumImpl());


                    count++;
                    if (count >= this.selectInputTypes[num].count && (num + 1) < this.selectInputTypes.Count)
                    {
                        // 次の入力速度へ
                        count = 0;
                        num++;
                    }
                }

                yield return null;
            }
        }

        private IEnumerator SetPartStageNumImpl()
        {
            // UpDown アニメーション再生終了まで待機
            yield return null;  // 1f待機
            yield return new WaitUntil(() => this.stageNumberParts[0].IsPlayingUpDownAnimation == false);
            this.setStageNumCoroutine = null;
        }

        /// <summary>
        /// 各パーツにステージ番号を設定する
        /// </summary>
        private void SetPartsStageNum()
        {
            for (int i = 0; i < this.stageNumberParts.Count; i++)
            {
                // -1以下か
                var stageNum = this.currentStageNum - this.selectingNum + i;
                var str = (stageNum >= 0 && stageNum <= this.selectMax) ? $"{stageNum + 1}" : "";

                this.stageNumberParts[i].SetStageNumStr(str);
                this.stageNumberParts[i].UpdateStageText();
            }
        }
    }
}
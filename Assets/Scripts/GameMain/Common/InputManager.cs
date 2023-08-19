using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VANITILE
{
    /// <summary>
    /// インプット操作
    /// </summary>
    public class InputManager : SingletonMono<InputManager>
    {
        /// <summary>
        /// 横操作
        /// </summary>
        public float Horizontal { get { return Input.GetAxis("Horizontal"); } }

        /// <summary>
        /// 縦操作
        /// </summary>
        public float Vertical { get { return Input.GetAxis("Vertical"); } }

        /// <summary>
        /// ジャンプ
        /// </summary>
        public bool Jump { get { return Input.GetKeyDown(KeyCode.Space); } }

        /// <summary>
        /// 決定
        /// </summary>
        public bool Decide { get { return Input.GetButtonDown("Decide"); } }

        /// <summary>
        /// 戻る
        /// </summary>
        public bool Back { get { return Input.GetButtonDown("Back"); } }

        /// <summary>
        /// 上下入力一回
        /// </summary>
        public Subject<int> VerticalOneSubject { get; } = new Subject<int>();

        /// <summary>
        /// 連続上下移動 購読用
        /// 一応 int にしてるが正直使いどころによっては bool でもいい気がする
        /// </summary>
        public Subject<int> ConsecutiveVerticalSubject { get; } = new Subject<int>();

        /// <summary>
        /// ゲーム実行前に呼び出す
        /// </summary>
        [RuntimeInitializeOnLoadMethod()]
        static void Init()
        {
            var obj = GameObject.Instantiate(new GameObject("InputManager"));
            obj.AddComponent<InputManager>();
            GameObject.DontDestroyOnLoad(obj);

            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }

        /// <summary>
        /// 上下入力一回
        /// </summary>
        /// <returns> CompositeDisposable </returns>
        public CompositeDisposable StartVerticalSubject()
        {
            CompositeDisposable disposables = new CompositeDisposable();

            // 上下入力の監視
            var inputObservable = Observable.EveryUpdate()
                .Select(_ => this.Vertical)
                .Select(input => Mathf.Floor(input));

            // 値が変わるまで開始 ニュートラルに戻ったら再度入力開始
            inputObservable.DistinctUntilChanged()
                .Where(input => input != .0f)
                .Subscribe(input =>
                {
                    this.ConsecutiveVerticalSubject.OnNext((int)input);
                    this.VerticalOneSubject.OnNext((int)input);
                }).AddTo(disposables);

            return disposables;
        }

        /// <summary>
        /// 連続上下入力 
        /// NOTE:徐々に早くする入力の場合、 take の入力を 3->6->Max にする。
        /// ◆必ず終了する時に Dispose する事◆
        /// </summary>
        /// <param name="interval">移動間隔</param>
        /// <param name="take">移動回数 MAX以上は流石に考慮しない</param>
        /// <returns> CompositeDisposable </returns>
        public CompositeDisposable StartConsecutiveVerticalSubject(float interval = 0.25f, int take = int.MaxValue)
        {
            CompositeDisposable disposables = new CompositeDisposable();

            // 上下入力の監視
            var inputObservable = Observable.EveryUpdate()
                .Select(_ => this.Vertical)
                .Select(input => Mathf.Floor(input));

            // 値が変わるまで開始
            inputObservable.DistinctUntilChanged()
                .Where(input => input != .0f)
                .Subscribe(input =>
                {
                    Observable.Timer(System.TimeSpan.FromSeconds(0f), System.TimeSpan.FromSeconds(interval))
                        .Take(take)
                        .DoOnCompleted(() =>
                        {
                            Debug.Log($"指定回数の移動終了 take:{take} StartConsecutiveVerticalSubject");
                            this.ConsecutiveVerticalSubject.OnCompleted();
                        })
                        .Subscribe(_ =>
                        {
                            this.ConsecutiveVerticalSubject.OnNext((int)input);
                        }).AddTo(disposables);
                }).AddTo(disposables);

            return disposables;
        }
    }
}
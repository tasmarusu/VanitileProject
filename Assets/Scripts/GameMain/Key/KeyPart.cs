namespace VANITILE
{
    using UniRx;
    using UniRx.Triggers;
    using UnityEngine;

    /// <summary>
    /// 各鍵
    /// </summary>
    public class KeyPart : CommonPart, IPart
    {
        /// <summary>
        /// スプライト
        /// </summary>
        [SerializeField] private SpriteRenderer sprite = null;

        /// <summary>
        /// コライダー
        /// </summary>
        [SerializeField] private Collider2D myCollider = null;

        /// <summary>
        /// 鍵取得情報
        /// </summary>
        public bool IsGet { get; private set; } = false;

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            // プレイヤーに接触時、鍵取得
            this.myCollider.OnTriggerEnter2DAsObservable()
                .Where(col => col.CompareTag("Player"))
                .Subscribe(_ =>
                {
                    this.GetKey();
                }).AddTo(this);
        }

        /// <summary>
        /// IDの設定
        /// </summary>
        public void SetId(int id)
        {
            this.Id = id;
        }

        /// <summary>
        /// 鍵取得
        /// </summary>
        private void GetKey()
        {
            StageDataModel.Instance.GetPlayerKey();
            this.sprite.enabled = false;
        }
    }
}
namespace VANITILE
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using static DefineData;

    public class BlockPart : CommonPart, IPart
    {
        /// <summary>
        /// Dissolveの消えるAnimationCurve
        /// </summary>
        [SerializeField, Header("Dissolveの消えるAnimationCurve")] private AnimationCurve curve;

        /// <summary>
        /// 消える Dissolve 速度
        /// </summary>
        [SerializeField, Header("消える Dissolve 速度")] private float disappearLerpSpeed = .5f;

        /// <summary>
        /// 接触時 Dissolve 速度
        /// </summary>
        [SerializeField, Header("接触時 Dissolve 速度")] private float hitPlayerDisolveSpeed = 2.0f;

        /// <summary>
        /// 接触時 Dissolve 値
        /// </summary>
        [SerializeField, Header("接触時 Dissolve 値"), Range(0.0f, 1.0f)] private float hitPlayerDisolveValue = .35f;

        /// <summary>
        /// 接触時 ブロックの下がる 値
        /// </summary>
        [SerializeField, Header("接触時 ブロックの下がる 値")] private float hitPlayerDownValue = .05f;

        /// <summary>
        /// SpriteRenderer
        /// </summary>
        [SerializeField] private SpriteRenderer spRenderer = null;

        /// <summary>
        /// Collider
        /// </summary>
        [SerializeField] private List<Collider2D> col2Ds = new List<Collider2D>();

        /// <summary>
        /// テキスト
        /// </summary>
        [SerializeField] private TextMeshProUGUI text;

        /// <summary>
        /// 角度種類
        /// </summary>
        public enum AngleType
        {
            /// <summary>
            /// 壁滑り中に消える
            /// </summary>
            Wait,

            /// <summary>
            /// 下
            /// </summary>
            Down,

            /// <summary>
            /// 右
            /// </summary>
            Right,

            /// <summary>
            /// 左
            /// </summary>
            Left,
        }

        /// <summary>
        /// プレイヤー接触したか
        /// </summary>
        public bool IsContactPlayer { get; private set; } = false;

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            ////this.text.text = $"{this.gameObject.name}";
        }

        /// <summary>
        /// 外部からのID設定
        /// </summary>
        /// <param name="id"></param>
        public void SetId(int id)
        {
            this.Id = id;
        }

        /// <summary>
        /// プレイヤーに接触
        /// </summary>
        /// <param name="type">地面か壁か</param>
        public void HitPlayer(CollisionType type)
        {
            this.IsContactPlayer = true;
            this.StartCoroutine(this.StartHitBlockDissolve());
            this.spRenderer.materials[0].SetColor("_Color", type == CollisionType.Ground ? Color.red : Color.green);
        }

        /// <summary>
        /// プレイヤーが離れる
        /// </summary>
        public void LeavePlayer(AngleType angleType = AngleType.Down)
        {
            if (this.IsContactPlayer == true)
            {
                // 判定の削除
                foreach (var col in this.col2Ds)
                {
                    GameObject.Destroy(col);
                }

                // 接触情報の通知
                StageDataModel.Instance.TouchBlockInPlayer();

                // エフェクト再生
                this.StartCoroutine(this.StartDestroyEffect(angleType));
            }
        }

        /// <summary>
        /// ブロックの Dissolve 値を増やす
        /// </summary>
        /// <returns>IEnumerator</returns>
        private IEnumerator StartHitBlockDissolve()
        {
            var timer = .0f;
            while (timer <= this.hitPlayerDisolveValue)
            {
                timer += Time.deltaTime * this.hitPlayerDisolveSpeed;
                var val = this.curve.Evaluate(timer);
                this.spRenderer.materials[0].SetFloat("_Threshold", val);
                this.spRenderer.materials[0].SetFloat("_DownValue", val * this.hitPlayerDownValue);
                yield return null;
            }

            this.spRenderer.materials[0].SetFloat("_Threshold", this.curve.Evaluate(this.hitPlayerDisolveValue));
        }

        /// <summary>
        /// 削除エフェクトの開始
        /// </summary>
        private IEnumerator StartDestroyEffect(AngleType angleType)
        {
            // 重力追加して指定方向に飛ばす
            this.AddForceRig(angleType);

            var timer = this.hitPlayerDisolveValue;
            var pos = this.spRenderer.transform.position;
            var scale = this.spRenderer.transform.localScale;
            while (timer <= 1.0f)
            {
                timer += Time.deltaTime / this.disappearLerpSpeed;
                var val = this.curve.Evaluate(timer);
                this.spRenderer.materials[0].SetFloat("_Threshold", val);
                this.spRenderer.materials[0].SetTextureOffset("_MainTex", new Vector2(.0f, val * 0.1f));
                this.spRenderer.transform.localScale = scale + new Vector3(val * 0.1f, val, .0f);
                yield return null;
            }

            // 上ジャンプSE
            SoundManager.Instance.PlaySe(DefineData.SeType.BreakBlock);

            // 時間経過後ブロック完全削除
            GameObject.Destroy(this.gameObject);
        }

        /// <summary>
        /// 飛ばす方向に傾けて重力を付ける
        /// </summary>
        private void AddForceRig(AngleType angleType)
        {
            var force = Vector2.zero;
            var euler = Vector3.zero;

            switch (angleType)
            {
                case AngleType.Wait:
                    break;

                case AngleType.Down:
                    this.gameObject.AddComponent<Rigidbody2D>();
                    break;

                case AngleType.Right:
                    force = new Vector2(100.0f, 80.0f);
                    this.gameObject.AddComponent<Rigidbody2D>().AddForce(force);

                    euler = this.transform.eulerAngles;
                    this.transform.eulerAngles = new Vector3(euler.x, euler.y, euler.z + 5.0f);

                    break;

                case AngleType.Left:
                    force = new Vector2(-100.0f, 80.0f);
                    this.gameObject.AddComponent<Rigidbody2D>().AddForce(force);

                    euler = this.transform.eulerAngles;
                    this.transform.eulerAngles = new Vector3(euler.x, euler.y, euler.z - 5.0f);

                    break;
            }
        }
    }
}
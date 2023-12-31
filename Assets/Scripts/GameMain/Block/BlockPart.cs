﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static DefineData;

namespace VANITILE
{
    public class BlockPart : CommonPart, IPart
    {
        /// <summary>
        /// Dissolveの消えるAnimationCurve
        /// </summary>
        [SerializeField] private AnimationCurve curve;

        /// <summary>
        /// Dissolveの消える速度
        /// </summary>
        [SerializeField] private float disappearLerpTime = .5f;

        /// <summary>
        /// プレイヤー接触したか
        /// </summary>
        public bool IsContactPlayer { get; private set; } = false;

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
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            //this.text.text = $"{this.gameObject.name}";
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
            this.spRenderer.materials[0].SetColor("_Color", type == CollisionType.Ground ? Color.red : Color.green);
        }

        /// <summary>
        /// プレイヤーが離れる
        /// </summary>
        public void LeavePlayer()
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
                this.StartCoroutine(this.StartDestroyEffect());
            }
        }

        /// <summary>
        /// 削除エフェクトの開始
        /// </summary>
        private IEnumerator StartDestroyEffect()
        {
            this.gameObject.AddComponent<Rigidbody2D>();

            var timer = .0f;
            var pos = this.spRenderer.transform.position;
            var scale = this.spRenderer.transform.localScale;
            while (timer <= 1.0f)
            {
                timer += Time.deltaTime / this.disappearLerpTime;
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
    }
}
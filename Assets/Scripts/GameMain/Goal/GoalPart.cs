﻿using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace VANITILE
{
    /// <summary>
    /// ゴール本体
    /// </summary>
    public class GoalPart : CommonPart, IPart
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
        /// 初期化
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// idの設定
        /// </summary>
        /// <param name="id"></param>
        public void SetId(int id)
        {
            this.Id = id;
        }

        /// <summary>
        /// プレイヤーとの接触判定
        /// </summary>
        public void StartCheckHitPlayer()
        {
            this.sprite.enabled = true;

            // ゴール可能状態でプレイヤーと接触するとゴール
            // 可能になったら購読開始とかしたい
            this.myCollider.OnTriggerStay2DAsObservable()
                .Where(col => col.CompareTag("Player"))
                .Where(x => StageDataModel.Instance.IsAbleGoal())
                .Select(col => col.transform.parent.GetComponent<PlayerController>())
                .Subscribe(pc =>
                {
                    Debug.Log($"[Goal]プレイヤーの一人がゴールした");
                    StageDataModel.Instance.GoalPlayer();
                    pc.Goal();
                }).AddTo(this);
        }
    }
}
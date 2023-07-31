using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace VANITILE
{
    /// <summary>
    /// プレイヤーの失敗位置
    /// </summary>
    public class PlayerMissPoint : MonoBehaviour
    {
        /// <summary>
        /// 当たり判定
        /// </summary>
        [SerializeField] private BoxCollider2D boxCollider2D = null;

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
            this.boxCollider2D.OnTriggerEnter2DAsObservable()
                .Where(collider => collider.CompareTag("Player"))
                .Select(collider => collider.transform.parent.GetComponent<PlayerController>())
                .Subscribe(player =>
                {
                    player.Miss();
                    StageDataModel.Instance.MissPlayer();
                }).AddTo(this);
        }
    }
}
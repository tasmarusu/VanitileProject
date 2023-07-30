using System.Collections.Generic;
using UnityEngine;
using static DefineData;

namespace VANITILE
{
    /// <summary>
    /// プレイヤーのコリジョン判定用クラス
    /// 壁と地面
    /// </summary>
    public class PlayerCollision : MonoBehaviour
    {
        /// <summary>
        /// 判定タイプの設定
        /// </summary>
        [SerializeField, Header("判定タイプの設定")] private CollisionType collistionType;

        /// <summary>
        /// 接触中のブロックリスト
        /// </summary>
        private List<GameObject> hitBlocks = new List<GameObject>();

        /// <summary>
        /// 接触中
        /// </summary>
        public bool IsTouch { get; private set; } = false;

        /// <summary>
        /// in
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var hitObj = collision.gameObject;
            if (hitObj.tag == "Block")
            {
                // ブロック側に接触した事を伝える
                BlockPart part;
                if (hitObj.transform.parent.TryGetComponent(out part))
                {
                    // ブロック多重ヒット回避
                    if (part.IsContactPlayer)
                    {
                        return;
                    }
#if UNITY_EDITOR
                    Debug.Log($"[Collision]接触ブロック名:{part.gameObject.name} :{collistionType} {Time.frameCount}");
#endif
                    // ヒットした事ブロックを保持
                    this.hitBlocks.Add(hitObj);
                    part.HitPlayer(this.collistionType);
                }
                else
                {
                    return;
                }

                // ブロックとの接触判定開始
                this.IsTouch = true;
            }
        }

        /// <summary>
        /// out
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerExit2D(Collider2D collision)
        {
            var hitObj = collision.gameObject;
            if (hitObj.tag == "Block")
            {
                // ブロック側に離れた事を伝える
                BlockPart part;
                if (hitObj.transform.parent.TryGetComponent<BlockPart>(out part))
                {
                    // ブロック多重ヒット判定回避
                    if (this.hitBlocks.Contains(hitObj) == false)
                    {
                        return;
                    }
#if UNITY_EDITOR
                    Debug.Log($"[Collision]退避ブロック名:{part.gameObject.name} :{collistionType} {Time.frameCount}");
#endif
                    // 接触ブロック解除
                    this.hitBlocks.Remove(hitObj);
                    part.LeavePlayer();
                }
                else
                {
                    return;
                }

                // ブロックとの接触判定終了
                if (this.hitBlocks.Count == 0)
                {
                    this.IsTouch = false;
                }
            }
        }
    }
}


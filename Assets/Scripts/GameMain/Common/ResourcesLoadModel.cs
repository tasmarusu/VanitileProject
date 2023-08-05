using UnityEngine;

namespace VANITILE
{
    /// <summary>
    /// Resources.Load の管理
    /// Addressables 使うか？
    /// </summary>
    public class ResourcesLoadModel : Singleton<ResourcesLoadModel>
    {
        /// <summary>
        /// BGM
        /// </summary>
        /// <param name="type">種類</param>
        /// <returns>AudioClip</returns>
        public AudioClip BgmClip(DefineData.BgmType type)
        {
            var path = $"Sounds/Bgm/{type.ToString()}";
            var obj = Resources.Load<AudioClip>(path);

#if UNITY_EDITOR
            if (obj == null)
            {
                Debug.LogError($"[Load]リソースのロードに失敗しました path:{path}");
                return null;
            }
#endif
            return obj;
        }

        /// <summary>
        /// SE
        /// </summary>
        /// <param name="type">種類</param>
        /// <returns>AudioClip</returns>
        public AudioClip SeClip(DefineData.SeType type)
        {
            var path = $"Sounds/Se/{type.ToString()}";
            var obj = Resources.Load<AudioClip>(path);

#if UNITY_EDITOR
            if (obj == null)
            {
                Debug.LogError($"[Load]リソースのロードに失敗しました path:{path}");
                return null;
            }
#endif
            return obj;
        }
    }
}
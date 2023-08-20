namespace VANITILE
{
    using UnityEngine;

    /// <summary>
    /// Resources.Load の管理
    /// Addressables 使うか？
    /// https://kiironomidori.hatenablog.com/entry/unity_save_json#SaveControllerTemplate%E3%82%92%E6%9B%B8%E3%81%8D%E6%8F%9B%E3%81%88%E3%81%9F%E3%82%AF%E3%83%A9%E3%82%B9
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
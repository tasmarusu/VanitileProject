namespace VANITILE
{
    using UnityEngine;

    /// <summary>
    /// Mono継承用シングルトンクラス
    /// 重複は調べていない
    /// </summary>
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// instance
        /// </summary>
        private static T instance;

        /// <summary>
        /// Instance
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));
                    if (instance == null)
                    {
                        Debug.LogError($"{typeof(T)}をアタッチしている Object が無いです");
                    }
                }

                return instance;
            }
        }
    }
}
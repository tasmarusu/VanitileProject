using UnityEngine;

/// <summary>
/// MonoBehaviourではないシングルトン
/// </summary>
/// <typeparam name="T">クラスの型</typeparam>
public class Singleton<T> where T : new()
{
    /// <summary>
    /// インスタンス
    /// </summary>
    private static T instance;

    /// <summary>
    /// インスタンス
    /// </summary>
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
                if (instance == null)
                {
                    Debug.LogError(typeof(T) + "is nothing");
                }
            }

            return instance;
        }
    }

    /// <summary>
    /// 解放処理
    /// </summary>
    public virtual void Release()
    {
        instance = default(T);
    }
}
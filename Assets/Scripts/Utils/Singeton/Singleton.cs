using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T s_instance;
    private static bool _isApplicationQuitting;

    public static T Instance
    {
        get
        {
            if (_isApplicationQuitting)
            {
                return null;
            }

            if (s_instance == null)
            {
                s_instance = FindObjectOfType<T>();
                if (s_instance == null)
                {
                    new GameObject(typeof(T).Name, typeof(T));
                }
            }

            return s_instance;
        }
    }

    protected virtual void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (s_instance != this)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (s_instance == this)
        {
            s_instance = null;
        }
    }

    protected virtual void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
    }
}

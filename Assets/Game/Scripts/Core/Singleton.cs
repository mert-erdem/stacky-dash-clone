using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance => _instance ??= FindObjectOfType<T>();

    public virtual void Awake()
    {
        if (_instance == null)
            _instance = this as T;
        else
            Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton <T>: MonoBehaviour where T: Component
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindAnyObjectByType<T>();
            }

            if(instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = typeof(T).Name;
                instance = obj.AddComponent<T>();
            }

            return instance;
        }
    }
    // Start is called before the first frame update
    public virtual void Awake()
    {
        if(instance == null)
        {
            Debug.Log($"{typeof(T)} set to Don't DestroyOnLoad Object");
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log($"{typeof(T)} object was destroyed");
            Destroy(gameObject);
        }
    }
}

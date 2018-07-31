using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));
                if (instance == null)
                {
                    GameObject gameObject = Instantiate(new GameObject());
                    instance = gameObject.AddComponent<T>();
                    gameObject.name = instance.GetType().ToString();
                    DontDestroyOnLoad(gameObject);
                    return instance;
                }
            }

            return instance;
        }
    }

    protected static void SetInstance(T value)
    {
        instance = value;
        DontDestroyOnLoad(value.gameObject);
    }
}
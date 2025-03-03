using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField]
    private bool _persistBetweenScenes = false;

    private static T _instance;
    private static readonly object _lock = new object();

    [NotNull]
    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if(_instance != null)
                    return _instance;
                var instances = FindObjectsOfType<T>();
                var count = instances.Length;
                if(count > 0)
                {
                    if(count == 1)
                        return _instance = instances[0];
                    Debug.LogWarning($"{nameof(Singleton<T>)} There should be one instance of {nameof(Singleton<T>)} of type {typeof(T)} in the scene but more were found");
                    for(int i = 1; i < count; i++)
                    {
                        Destroy(instances[i]);
                    }
                    return instances[0];
                }
                Debug.LogWarning($"{nameof(Singleton<T>)} of type {typeof(T)} was not found.");
                return _instance = new GameObject($"({nameof(Singleton<T>)}){typeof(T)}").AddComponent<T>();
            }
        }
    }

    private void Awake()
    {
        if(_persistBetweenScenes)
            DontDestroyOnLoad(gameObject);
        OnAwake();
    }

    protected virtual void OnAwake() { }
}
/*
Kamran Bigdely
https://gamedev.stackexchange.com/questions/116009/in-unity-how-do-i-correctly-implement-the-singleton-pattern
*/

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField]
    private bool _persistBetweenScenes = false;

    private static T _instance;

    [NotNull]
    public static T Instance
    {
        get
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
                    Destroy(instances[i].gameObject);
                }
                return _instance = instances[0];
            }
            Debug.LogWarning($"{nameof(Singleton<T>)} of type {typeof(T)} was not found.");
            return _instance = new GameObject($"({nameof(Singleton<T>)}){typeof(T)}").AddComponent<T>();
        }
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this as T;
        else if(_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if(_persistBetweenScenes)
            DontDestroyOnLoad(gameObject);
        OnAwake();
    }

    protected virtual void OnAwake() { }
}
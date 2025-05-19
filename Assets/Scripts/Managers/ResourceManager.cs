using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField]
    private int _startingResources = 100;

    private int _resource = 0;
    public int Resource => _resource;

    private void Start()
    {
        _resource = _startingResources;
    }

    public bool UseResources(int amount)
    {
        if(_resource - amount < 0 || amount <= 0)
            return false;
        _resource -= amount;
        return true;
    }
    public void AddResource(int amount)
    {
        if (amount <= 0)
            return;
        _resource += amount;
    }
}

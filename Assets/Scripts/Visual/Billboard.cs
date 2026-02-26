using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }
    void Update()
    {
        if (_camera == null) return;
        transform.forward = _camera.transform.forward;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Detector : MonoBehaviour
{
    public UnityEvent<Collider> OnTriggerIn;
    public UnityEvent<Collider> OnTriggerOut;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerIn?.Invoke(other);
    }
    private void OnTriggerExit(Collider other)
    {
        OnTriggerOut?.Invoke(other);
    }

    public void Rotate(Vector3 direction)
    {
        gameObject.transform.rotation = Quaternion.LookRotation(direction.normalized);
    }
}

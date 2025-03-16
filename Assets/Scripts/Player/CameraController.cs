using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    [Range(0.01f, 0.4f)]
    private float _zoomSpeed = 0.1f;

    [SerializeField]
    private CinemachineVirtualCamera _playerCamera;

    private CinemachineTrackedDolly _dolly;

    private void Awake()
    {
        if(_playerCamera == null)
        {
            Debug.LogError($"Player camera reference is missing on {gameObject.name}");
            return;
        }
        _dolly = _playerCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    public void OnCameraZoom(InputAction.CallbackContext context)
    {
        if (_dolly == null)
            return;

        if (context.performed)
        {
            float value = context.ReadValue<float>();
            value = _zoomSpeed * Mathf.Sign(value);
            _dolly.m_PathPosition = Mathf.Clamp01(_dolly.m_PathPosition + value);
        }
    }
}

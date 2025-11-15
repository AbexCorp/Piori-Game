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
        if (_isWatchtower)
            return;
        if (_dolly == null)
            return;

        if (context.performed)
        {
            float value = context.ReadValue<float>();
            value = _zoomSpeed * Mathf.Sign(value);
            _dolly.m_PathPosition = Mathf.Clamp01(_dolly.m_PathPosition + value);
        }
    }

    private bool _isWatchtower = false;
    private float _previous = 0;
    public void ToggleWatchtower()
    {
        if(_isWatchtower)
        {
            _isWatchtower = false;
            _dolly.m_PathPosition = _previous;
            return;
        }

        _isWatchtower = true;
        _previous = _dolly.m_PathPosition;
        _dolly.m_PathPosition = 2;
    }
}

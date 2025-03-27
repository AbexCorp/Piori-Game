using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [Header("Internal")]
    [SerializeField]
    private Rigidbody _rigidbody;


    [Header("Movement")]
    [SerializeField]
    private float _speed = 1;
    [SerializeField]
    private LayerMask _groundMask;

    void Awake()
    {
        if (_rigidbody == null)
            _rigidbody.GetComponent<Rigidbody>();
    }

    void Update()
    {
        Move();
    }


    #region >>> Movement <<<

    private Vector3 _movement;
    private Vector2 _mousePosition;

    public Vector2 MousePosition => _mousePosition;


    private void Move()
    {
        _rigidbody.velocity = _movement * _speed;
        UpdateGridPosition();
    }
    private void UpdateGridPosition()
    {
        RaycastHit hit;
        if(Physics.Raycast(origin:transform.position + Vector3.up * 0.1f, direction:Vector3.down, hitInfo:out hit, maxDistance:1f, layerMask: _groundMask.value))
        {
            hit.collider.gameObject.TryGetComponent<GridTile>(out GridTile tile);
            GridManager.Instance.ChangePlayerPosition(tile);
        }
        else
            GridManager.Instance.ChangePlayerPosition(null);
    }


    public void OnDirectMovement(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector3>();
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        if(Camera.main == null)
        {
            Debug.LogError("No camera in the scene");
            _mousePosition = Vector2.zero;
            return;
        }

        _mousePosition = context.ReadValue<Vector2>();
    }

    #endregion
}

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
    private bool _pointMovement;
    [SerializeField]
    private float _speed = 1;

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
        if (_pointMovement)
        {
            PointMove();
            return;
        }

        _rigidbody.velocity = _movement * _speed;
    }

    private void PointMove()
    {
        if (_rigidbody.position.DistanceTo2D(_movement) <= 0.05f)
            _rigidbody.velocity = Vector2.zero;
        else
            _rigidbody.velocity = _rigidbody.position.DirectionTo2D(_movement) * _speed;
    }


    public void OnDirectMovement(InputAction.CallbackContext context)
    {
        if (_pointMovement)
            return;

        _movement = context.ReadValue<Vector3>();
    }

    public void OnPointMovement(InputAction.CallbackContext context)
    {
        if(!_pointMovement)
            return;

        if(context.performed)
        {
            _movement.x = _mousePosition.x;
            _movement.y = 0;
            _movement.z = _mousePosition.y;
        }
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        if(Camera.main == null)
        {
            Debug.LogError("No camera in the scene");
            _mousePosition = Vector2.zero;
            return;
        }

        _mousePosition = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
    }

    #endregion
}

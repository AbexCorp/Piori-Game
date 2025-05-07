using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour, IHealth
{
    [Header("Internal")]
    [SerializeField]
    private Rigidbody _rigidbody;


    void Awake()
    {
        if (_rigidbody == null)
            _rigidbody.GetComponent<Rigidbody>();
        InitializePlayer();
    }
    private void InitializePlayer()
    {
        _healthCurrent = HealthMax;
    }

    void Update()
    {
        Move();
    }


    #region >>> Movement <<<

    [Header("Movement")]
    [SerializeField]
    private float _speed = 1;
    [SerializeField]
    private LayerMask _groundMask;


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


    #endregion


    #region >>> Health <<<

    [Header("Combat")]
    [SerializeField]
    private int _healthMax = 100;
    public int HealthMax => _healthMax;
    private int _healthCurrent = 0;
    public int HealthCurrent => _healthCurrent;
    public GameObject ParentGameObject => gameObject;


    public void GetDamaged(int damage)
    {
        if (damage <= 0)
            return;
        _healthCurrent -= damage;
        Debug.Log(_healthCurrent);
        if (_healthCurrent <= 0)
            Die();
    }
    private void Die()
    {
        Debug.Break();
    }

    #endregion


    #region >>> Mouse <<<

    [Header("Mouse Interaction")]
    [SerializeField]
    private LayerMask _mouseHitMask;

    private IMouseInteractable _hover = null;

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        if(Camera.main == null)
        {
            Debug.LogError("No camera in the scene");
            _mousePosition = Vector2.zero;
            return;
        }

        _mousePosition = context.ReadValue<Vector2>();
        MouseHover(context);
    }
    private void MouseHover(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(MousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask: _mouseHitMask))
        {
            if (hit.collider.gameObject.TryGetComponent<IMouseInteractable>(out var interacted))
            {
                if (_hover != null)
                    _hover.OnHoveExit(context);
                _hover = interacted;
                _hover.OnHoverEnter(context);
            }
            else
            {
                if (_hover != null)
                    _hover.OnHoveExit(context);
                _hover = null;
            }
        }
    }

    public void OnMouseClick(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(MousePosition);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask: _mouseHitMask).OrderBy(x => x.distance).ToArray();
        for(int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.TryGetComponent<IMouseInteractable>(out var interacted))
            {
                interacted.OnClick(context);
            }
        }
    }

    #endregion
}

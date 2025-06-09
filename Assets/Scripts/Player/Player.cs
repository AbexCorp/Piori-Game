using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
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
        GameManager.Instance.InterfaceManager.UpdateHealth(_healthCurrent, HealthMax);
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
        GameManager.Instance.InterfaceManager.UpdateHealth(_healthCurrent, HealthMax);
        if (_healthCurrent <= 0)
            Die();
    }
    private void Die()
    {
        _healthCurrent = 0;
        GameManager.Instance.ChangeGameState(GameState.Lose);
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
        ////
        //Ui Raycast
        ////
        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        if(results.Count > 0)
        {
            if(results.FirstOrDefault().gameObject.TryGetComponent<IMouseInteractable>(out var interracted))
            {
                if (_hover != null)
                    _hover.OnHoveExit(context);
                _hover = interracted;
                _hover.OnHoverEnter(context);
                return;
            }
            else
            {
                if (_hover != null)
                    _hover.OnHoveExit(context);
                _hover = null;
            }
        }




        ////
        //Gameobject Raycast
        ////
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
        ////
        //Ui Raycast
        ////
        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        if(results.Count > 0)
        {
            if(results.FirstOrDefault().gameObject.TryGetComponent<IMouseInteractable>(out var interracted))
            {
                interracted.OnClick(context);
                return;
            }
        }




        ////
        //Gameobject Raycast
        ////
        Ray ray = Camera.main.ScreenPointToRay(MousePosition);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask: _mouseHitMask).OrderBy(x => x.distance).ToArray();

        //Single clicks
        if (hits.Length <= 0)
            return;
        if(hits[0].collider.gameObject.TryGetComponent<IMouseInteractable>(out var interacted))
            interacted.OnClick(context);

        //Multiple clicks
        //for(int i = 0; i < hits.Length; i++)
        //{
        //    if (hits[i].collider.gameObject.TryGetComponent<IMouseInteractable>(out var interacted))
        //    {
        //        interacted.OnClick(context);
        //    }
        //}
    }

    #endregion


    #region >>> Attack <<<

    [Header("Attack")]
    [SerializeField]
    private ProjectileProfile _playerProjectile;
    [SerializeField]
    private int _damage = 20;
    [SerializeField]
    private float _attackCooldown = 1.5f;
    public float AttackCooldown => _attackCooldown;
    [SerializeField]
    private bool _attackIsOnCooldown = false;
    public bool AttackIsOnCooldown => _attackIsOnCooldown;
    [SerializeField]
    private float _remainingAttackCooldown = 0f;
    public float RemainingAttackCooldown => _remainingAttackCooldown;

    [SerializeField]
    private GameObject _attackCooldownUI;
    [SerializeField]
    private UnityEngine.UI.Image _attackCooldownUIFill;


    public void OnAttack(InputAction.CallbackContext context)
    {
        if (AttackIsOnCooldown)
            return;

        if (context.performed)
        {
            Ray ray = Camera.main.ScreenPointToRay(_mousePosition);
            if (Mathf.Abs(ray.direction.y) < 0.0001f) //paraller to world plane
                return;
            
            float t = -ray.origin.y / ray.direction.y;
            Vector3 hitpoint = ray.origin + t * ray.direction;

            Projectile projectile = GameManager.Instance.ProjectileManager.GetProjectile(_playerProjectile);
            projectile.InitializeProjectile(hitpoint, transform.position, _damage);
            StartCoroutine(AttackCooldownTimer());
        }
    }
    private IEnumerator AttackCooldownTimer()
    {
        int numberOfUpdates = 10;
        YieldInstruction yield = new WaitForSeconds(AttackCooldown / numberOfUpdates);

        _attackIsOnCooldown = true;
        _remainingAttackCooldown = AttackCooldown;
        _attackCooldownUI.SetActive(true);
        _attackCooldownUIFill.fillAmount = 1;
        for (int i = 0; i < numberOfUpdates; i++)
        {
            yield return yield;
            _remainingAttackCooldown -= AttackCooldown / numberOfUpdates;
            _remainingAttackCooldown = MathF.Round(_remainingAttackCooldown, 2);
            _attackCooldownUIFill.fillAmount = _remainingAttackCooldown / AttackCooldown;
        }
        _attackIsOnCooldown = false;
        _remainingAttackCooldown = 0f;
        _attackCooldownUI.SetActive(false);
        _attackCooldownUIFill.fillAmount = 0;
    }


    #endregion
}

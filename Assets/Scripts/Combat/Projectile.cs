using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour, ILoadable
{
    [HideInInspector]
    public bool IsUsed = false;

    protected Vector3 _spawnPosition;
    protected Vector3 _target;
    protected int _damage = 0;

    [Header("Prefab")]
    [SerializeField]
    protected string _uniqueID;
    public string UniqueID => _uniqueID;

    [Header("Internal")]
    [SerializeField]
    protected SphereCollider _collider;
    [SerializeField]
    protected Rigidbody _rigidBody;

    [Header("Projectile")]
    [SerializeField]
    protected float _speed = 2f;
    [SerializeField]
    protected SpriteRenderer _spriteRenderer;

    protected int _attackerID;
    protected string _attackerName;

    private void Awake()
    {
        _collider.isTrigger = true;
    }
    public void Load(ScriptableObject so)
    {
        if (so is not ProjectileProfile)
            return;
        ProjectileProfile pp = so as ProjectileProfile;

        _uniqueID = pp.UniqueID;
        gameObject.name = _uniqueID == null || _uniqueID == "" ? "Projectile (NoName)" : $"Projectile ({_uniqueID})";
        _speed = pp.Speed;

        _collisionMask = pp.CollisionMask;
        _rigidBody.includeLayers = _collisionMask;
        _hitMask = pp.HitMask;

        _lifetimeType = pp.LifetimeType;
        _lifetimeDistance = pp.LifetimeDistance;
        _lifetimeTime = pp.LifetimeTime;

        _useOmniDirection = pp.UseOmniDirection;
        _spriteRenderer.sprite = pp.Sprite;
    }

    public void InitializeProjectile(Vector3 target, Vector3 spawn, int attackerID, string attackerName, int? damage = null)
    {
        IsUsed = true;
        _attackerID = attackerID;
        _attackerName = attackerName;
        SetSpawn(spawn);
        SetTarget(target);
        SetDamage(damage);
        gameObject.SetActive(true);
        _rigidBody.velocity = _spawnPosition.DirectionTo2D(_target) * _speed;
        SetOmniDirection();

        switch (_lifetimeType)
        {
            case ProjectileLifetimeType.Time:
                StartCoroutine(TimeLifetime());
                break;

            case ProjectileLifetimeType.Distance:
                StartCoroutine(DistanceLifetime());
                break;

            case ProjectileLifetimeType.Target:
                StartCoroutine(TargetLifetime());
                break;
        }
    }
    protected void SetSpawn(Vector3 spawn)
    {
        transform.position = spawn + (Vector3.up * 0.5f);
        _spawnPosition = transform.position;
    }
    protected void SetTarget(Vector3 target)
    {
        _target = target; 
    }
    protected virtual void SetDamage(int? damage)
    {
        if(damage is not null)
        {
            _damage = (int)damage;
        }
    }


    #region >>> Collision <<<

    [Header("Collision")]
    [SerializeField]
    protected LayerMask _collisionMask;
    [SerializeField]
    protected LayerMask _hitMask;


    protected void OnTriggerEnter(Collider other)
    {
        if (IsInLayerMask(other.gameObject.layer, _hitMask))
        {
            ProjectileHit(other);
            BreakProjectile();
        }
        if (IsInLayerMask(other.gameObject.layer, _collisionMask))
            BreakProjectile();
    }
    protected virtual void ProjectileHit(Collider other)
    {
        if (other.gameObject.TryGetComponent<IHealth>(out IHealth target))
        {
            target.GetDamaged(_damage, _attackerID, _attackerName);
        }
    }

    private bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    #endregion


    #region >>> Lifetime <<<

    [Header("Lifetime")]
    [SerializeField]
    protected ProjectileLifetimeType _lifetimeType = ProjectileLifetimeType.Distance;
    public enum ProjectileLifetimeType
    {
        Time = 0,
        Distance = 1,
        Target = 2,
    }

    [SerializeField]
    protected float _lifetimeDistance = 4;
    [SerializeField]
    protected float _lifetimeTime = 2;


    protected virtual void BreakProjectile()
    {
        IsUsed = false;
        _rigidBody.velocity = Vector3.zero;
        StopAllCoroutines();
        OnProjectileBreak();
        gameObject.SetActive(false);
    }
    protected virtual void OnProjectileBreak() { }
    private IEnumerator TimeLifetime()
    {
        yield return new WaitForSeconds(_lifetimeTime);
        BreakProjectile();
    }
    private IEnumerator DistanceLifetime()
    {
        while (_spawnPosition.DistanceTo2D(transform.position) < _lifetimeDistance)
        {
            yield return new WaitForSeconds(0.2f);
        }
        BreakProjectile();
    }
    private IEnumerator TargetLifetime()
    {
        while (_spawnPosition.DistanceTo2D(transform.position) < _spawnPosition.DistanceTo2D(_target))
        {
            yield return new WaitForSeconds(0.2f);
        }
        BreakProjectile();
    }

    #endregion


    #region >>> OmniDirectional <<<

    [SerializeField]
    protected bool _useOmniDirection = false;
    [SerializeField]
    protected GameObject _omniDirectionSprite;

    protected void SetOmniDirection()
    {
        if( _useOmniDirection  == false)
        {
            _omniDirectionSprite.transform.rotation = Quaternion.Euler(_omniDirectionSprite.transform.eulerAngles.x, _omniDirectionSprite.transform.eulerAngles.y, 0);
            return;
        }
        Vector3 direction = Vector3Extensions.DirectionTo2D(_spawnPosition, _target);
        float angle = Vector3.Angle(Vector3.forward, direction);
        int sign = _spawnPosition.x > _target.x ? 1 : -1;
        _omniDirectionSprite.transform.rotation = Quaternion.Euler(_omniDirectionSprite.transform.eulerAngles.x, _omniDirectionSprite.transform.eulerAngles.y, sign * angle);
    }

    #endregion
}

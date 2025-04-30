using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class Projectile : MonoBehaviour
{
    [HideInInspector]
    public bool IsUsed = false;

    protected Vector3 _spawnPosition;
    protected Vector3 _target;
    protected int _damage = 0;

    [Header("Prefab")]
    [SerializeField]
    protected GameObject _prefab;
    [SerializeField]
    protected string _uniqueID;
    public string UniqueID => _uniqueID;

    [Header("Internal")]
    [SerializeField]
    protected SphereCollider _collider;
    [SerializeField]
    protected LayerMask _collisionMask;
    [SerializeField]
    protected LayerMask _hitMask;
    [SerializeField]
    protected Rigidbody _rigidBody;


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

    [Header("Projectile")]
    [SerializeField]
    protected float _speed = 2f;

    private void Awake()
    {
        _collider.isTrigger = true;
        _rigidBody.includeLayers = _collisionMask;
    }

    public void InitializeProjectile(Vector3 target, Vector3 spawn, int? damage = null)
    {
        IsUsed = true;
        SetSpawn(spawn);
        SetTarget(target);
        SetDamage(damage);
        gameObject.SetActive(true);
        _rigidBody.velocity = _spawnPosition.DirectionTo2D(_target) * _speed;

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

    protected void OnTriggerEnter(Collider other)
    {
        if (IsInLayerMask(other.gameObject.layer, _hitMask))
        {
            ProjectileHit(other);
            BreakProjectile();
        }
        if(IsInLayerMask(other.gameObject.layer, _collisionMask))
            BreakProjectile();
    }
    protected abstract void ProjectileHit(Collider other);

    private bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    #endregion


    #region >>> Lifetime <<<

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
}

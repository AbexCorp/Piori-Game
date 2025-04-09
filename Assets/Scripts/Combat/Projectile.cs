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
    protected bool _lifetimeInDistance = true;

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

    public void InitializeProjectile(Vector3 target, Vector3 spawn, Enemy enemy = null, Player player = null)
    {
        IsUsed = true;
        SetSpawn(spawn);
        SetTarget(target);
        SetDamage(enemy, player);
        gameObject.SetActive(true);
        _rigidBody.velocity = _spawnPosition.DirectionTo2D(_target) * _speed;

        if (_lifetimeInDistance)
            StartCoroutine(DistanceLifetime());
        else
            StartCoroutine(TimeLifetime());

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
    protected virtual void SetDamage(Enemy enemy, Player player)
    {
        if(enemy != null)
        {
            if(enemy is SimpleEnemy)
                _damage = (enemy as SimpleEnemy).RangedDamage;
        }
        if(player != null)
        {
            //Set damage from player here
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
        gameObject.SetActive(false);
    }
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

    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using UnityEngine;

public class Tower : Building
{
    [Header("Attack")]
    [SerializeField]
    protected bool _shoots = true; //or chain dog
    [SerializeField]
    protected TowerAttackType _attackType = TowerAttackType.Hitscan;
    public enum TowerAttackType
    {
        None = 0,
        Hitscan = 1,
        Projectile = 2,
        FakeProjectile = 3,
        Misc = 4
    }
    [SerializeField]
    protected ProjectileProfile _projectileProfile;

    [Space]
    [SerializeField]
    protected float _range = 4;
    [SerializeField]
    protected bool _hasCooldown = true;
    [SerializeField]
    protected float _attackCooldown = 0.3f;
    [SerializeField]
    protected int _damage = 20;

    private Enemy _target;
    private bool _isOnCooldown = false;


    void Update()
    {
        if (_shoots == false)
            return;
        AttackEnemy();
    }

    public override void Load(ScriptableObject so)
    {
        if (so is not TowerProfile)
            return;
        TowerProfile tp = so as TowerProfile;

        _uniqueID = gameObject.GetInstanceID();
        _uniqueName = tp.UniqueName;
        gameObject.name = $"Tower ({UniqueName})";
        _cost = tp.Cost;

        _healthMax = tp.HealthMax;
        _shoots = tp.Shoots;
        _attackType = tp.AttackType;
        _projectileProfile = tp.ProjectileProfile;

        _range = tp.Range;
        _hasCooldown = tp.HasCooldown;
        _attackCooldown = tp.AttackCooldown;
        _damage = tp.Damage;

        if(tp.Texture != null)
        {
            _renderer.material.SetTexture("_Texture", tp.Texture);
        }

        InitializeBuilding();
        AdjustDetectionRange(_range);
    }


    #region >>> Attack <<<

    private void AttackEnemy()
    {
        if (_isOnCooldown)
            return;

        GetTarget();
        if (_target == null)
            return;

        switch (_attackType)
        {
            case TowerAttackType.Hitscan:
                AttackHitscan();
                break;

            case TowerAttackType.Projectile:
                AttackProjectile();
                break;

            case TowerAttackType.FakeProjectile:
                AttackFakeProjectile();
                break;

            case TowerAttackType.None:
            default:
                break;
        }

        if(_hasCooldown)
            StartCoroutine(AttackCooldown());
    }
    private void AttackHitscan()
    {
        _target.GetDamaged(_damage);
    }
    private void AttackProjectile()
    {
        Projectile p = GameManager.Instance.ProjectileManager.GetProjectile(_projectileProfile);
        p.InitializeProjectile(_target.transform.position, transform.position, _damage);
    }
    private void AttackFakeProjectile()
    {
        Projectile p = GameManager.Instance.ProjectileManager.GetProjectile(_projectileProfile);
        p.InitializeProjectile(_target.transform.position, transform.position, 0);
        _target.GetDamaged(_damage);
    }
    private IEnumerator AttackCooldown()
    {
        _isOnCooldown = true;
        yield return new WaitForSeconds(_attackCooldown);
        _isOnCooldown = false;
    }

    #endregion


    #region >>> Find Target <<<

    protected int GetTargetValue(IHealth target)
    {
        return 1;
    }
    protected void GetTarget()
    {
        if(_targets.Count == 0)
        {
            _target = null;
            return;
        }

        List<IHealth> _targetsToRemove = new();
        foreach(var target in _targets.Keys.ToList())
        {
            if (target == null || target as UnityEngine.Object == null)
                _targetsToRemove.Add(target);
            else
                _targets[target] = GetTargetValue(target);
        }
        foreach(var t in _targetsToRemove)
        {
            _targets.Remove(t);
        }

        var tempIHealth = _targets.OrderByDescending(t => t.Value).FirstOrDefault().Key;
        if (tempIHealth == null)
            return;

        if(_targets.OrderByDescending(t => t.Value).FirstOrDefault().Key.ParentGameObject.TryGetComponent<Enemy>(out Enemy tempEnemy))
        {
            _target = tempEnemy;
            if (_targets[_target] < 0)
                _target = null;
        }
    }

    #endregion


    #region >>> TargetDetection <<<

    [Header("Target Detection")]
    [SerializeField]
    protected Detector _enemyDetector;
    [SerializeField]
    protected SphereCollider _detectorCollider;

    protected Dictionary<IHealth, int> _targets = new();

    protected void AdjustDetectionRange(float amount)
    {
        _detectorCollider.radius = amount;
    }

    public void OnTargetDetectEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<IHealth>(out IHealth target))
        {
            if (_targets.ContainsKey(target))
                return;
            _targets.Add(target, GetTargetValue(target));
        }
    }
    public void OnTargetDetectLeave(Collider other)
    {
        if(other.gameObject.TryGetComponent<IHealth>(out IHealth target))
        {
            if(_targets.ContainsKey(target))
                _targets.Remove(target);
        }
    }

    #endregion
}
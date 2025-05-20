using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
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



    private void Start()
    {
        StartCoroutine(CheckForEnemies());
    }
    public override void Load(ScriptableObject so)
    {
        if (so is not TowerProfile)
            return;
        TowerProfile tp = so as TowerProfile;

        gameObject.name = tp.UniqueID == null || tp.UniqueID == "" ? "Tower (NoName)" : $"Tower ({tp.UniqueID})";
        _cost = tp.Cost;

        _healthMax = tp.HealthMax;
        _shoots = tp.Shoots;
        _attackType = tp.AttackType;
        _projectileProfile = tp.ProjectileProfile;

        _range = tp.Range;
        _hasCooldown = tp.HasCooldown;
        _attackCooldown = tp.AttackCooldown;
        _damage = tp.Damage;

        InitializeBuilding();
    }


    #region >>> Find Enemy <<<

    private IEnumerator CheckForEnemies()
    {
        yield return null;
        while (true)
        {
            yield return new WaitForSeconds(0.25f);
            if(_isOnCooldown)
                continue;
            if(_target == null)
                FindEnemy();
            if(_target != null)
                AttackEnemy();
        }
    }
    private void FindEnemy()
    {
        foreach(var enemy in GameManager.Instance.EnemyManager.SpawnedEnemies)
        {
            if(enemy == null)
                continue;
            ConfirmEnemy(enemy);
            if(enemy != null)
                break;
        }
    }
    private void ConfirmEnemy(Enemy enemy)
    {
        if(enemy.transform.position.DistanceTo2D(transform.position) <= _range)
            _target = enemy;
        else
            _target = null;
    }

    #endregion


    #region >>> Attack <<<

    private void AttackEnemy()
    {
        ConfirmEnemy(_target);
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
}
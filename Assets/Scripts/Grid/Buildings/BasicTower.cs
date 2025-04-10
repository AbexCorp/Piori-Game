using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTower : Building
{
    [SerializeField]
    protected bool _shoots = true; //or chain dog
    [SerializeField]
    protected bool _shootingHitscan = false; // or projectile // or fake projectile // change this to enum
    [SerializeField]
    protected Projectile _projectilePrefab;

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
    private void AttackEnemy()
    {
        ConfirmEnemy(_target);
        if (_target == null)
            return;

        if (_shootingHitscan)
            AttackHitscan();
        else
            AttackProjectile();

        if(_hasCooldown)
            StartCoroutine(AttackCooldown());
    }
    private void AttackHitscan()
    {
        _target.GetDamaged(_damage);
    }
    private void AttackProjectile() { }
    private void AttackFakeProjectile() { }
    private IEnumerator AttackCooldown()
    {
        _isOnCooldown = true;
        yield return new WaitForSeconds(_attackCooldown);
        _isOnCooldown = false;
    }
}
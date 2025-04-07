using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemy : Enemy
{
    protected float _distanceToPlayer;
    protected override void Update()
    {
        UpdateDistanceToPlayer();
        if ( !(_stopsMovementAfterAttack || _attackIsOnCooldown) )
            Move();
        Attack();
    }
    protected void UpdateDistanceToPlayer()
    {
        _distanceToPlayer = transform.position.DistanceTo2D(GameManager.Instance.Player.gameObject.transform.position);
    }


    #region >>> Combat <<<

    [Header("Combat")]
    [SerializeField]
    protected bool _stopsMovementAfterAttack = true;

    protected bool _attackIsOnCooldown = false;

    protected void Attack()
    {
        if (_attackIsOnCooldown)
            return;
        if (_canAttackMelee)
            MeleeAttack();
        else if (_canAttackRanged)
            RangedAttack();
    }
    protected IEnumerator AttackCooldown(float time)
    {
        _attackIsOnCooldown = true;
        _rigidbody.velocity = Vector3.zero;
        yield return new WaitForSeconds(time);
        _attackIsOnCooldown = false;
    }

    #region >>> Melee <<<

    [Header("Melee")]
    [SerializeField]
    protected bool _usesMelee = false;
    protected bool _canAttackMelee => _usesMelee && _distanceToPlayer <= _meleeRange;

    [SerializeField]
    [Range(0f, 10f)]
    protected float _meleeAttackCooldown = 2f;

    [SerializeField]
    [Range(1, 300)]
    protected int _meleeDamage = 10;

    [SerializeField]
    [Range(0.1f, 3f)]
    protected float _meleeRange = 0.5f;

    protected virtual void MeleeAttack()
    {
        GameManager.Instance.Player.GetDamaged(_meleeDamage);
        StartCoroutine(AttackCooldown(_meleeAttackCooldown));
    }

    #endregion

    #region >>> Ranged <<<

    [Header("Ranged")]
    [SerializeField]
    protected bool _usesRanged = false;
    protected bool _canAttackRanged => _usesRanged && _distanceToPlayer <= _rangedAttackCooldown;

    [SerializeField]
    [Range(0f, 10f)]
    protected float _rangedAttackCooldown = 2f;

    [SerializeField]
    [Range(1, 300)]
    protected int _rangedDamage = 10;
    public int RangedDamage => _rangedDamage;

    [SerializeField]
    [Range(1f, 10f)]
    protected float _rangedRange = 3f;

    protected virtual void RangedAttack()
    {
        GameManager.Instance.Player.GetDamaged(_rangedDamage);
        StartCoroutine(AttackCooldown(_rangedAttackCooldown));
    }

    #endregion

    #endregion
}

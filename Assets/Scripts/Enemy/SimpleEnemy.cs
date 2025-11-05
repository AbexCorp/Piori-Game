using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleEnemy : Enemy
{
    protected override void Update()
    {
        if ( !(_stopsMovementAfterAttack && _attackIsOnCooldown) )
            Move();
        Attack();
        UpdateDetectors();
    }

    public override void Load(ScriptableObject so)
    {
        if (so is not SimpleEnemyProfile)
            return;
        SimpleEnemyProfile sp = so as SimpleEnemyProfile;

        _cost = sp.Cost;
        _tier = sp.Tier;

        _uniqueID = gameObject.GetInstanceID();
        _uniqueName = sp.UniqueName;
        gameObject.name = UniqueName == null || UniqueName == "" ? "Enemy (NoName)" : $"Enemy ({UniqueName})";


        _speed = sp.Speed;
        _groundMask = sp.GroundMask;
        _pathfindingType = sp.PathfindingType;

        _healthMax = sp.HealthMax;

        _stopsMovementAfterAttack = sp.StopsMovementAfterAttack;
        _attacksBuildings = sp.AttacksBuildings;
        _prioritizesPlayer = sp.PrioritizesPlayer;

        _usesMelee = sp.UsesMelee;
        _meleeAttackCooldown = sp.MeleeAttackCooldown;
        _meleeDamage = sp.MeleeDamage;
        _meleeRange = sp.MeleeRange;

        _projectileProfile = sp.ProjectileProfile;
        _usesRanged = sp.UsesRanged;
        _rangedAttackCooldown = sp.RangedAttackCooldown;
        _rangedDamage = sp.RangedDamage;
        _rangedRange = sp.RangedRange;


        if(sp.Texture != null)
        {
            _renderer.material.SetTexture("_Texture", sp.Texture);
        }

        InitializeEnemy();
    }
    

    #region >>> Combat <<<

    [Header("Combat")]
    [SerializeField]
    protected bool _stopsMovementAfterAttack = true;
    [Header("Building Attack")]
    [SerializeField]
    protected bool _attacksBuildings = false;
    [SerializeField]
    protected bool _prioritizesPlayer = true;

    protected bool _attackIsOnCooldown = false;
    protected Dictionary<IHealth, int> _targets = new();
    private IHealth _target;
    protected float _distanceToTarget;

    protected void Attack()
    {
        if (_attackIsOnCooldown)
            return;

        GetTarget();
        if (_target == null)
            return;

        UpdateDistanceToTarget();
        if (_distanceToTarget <= _meleeRange && _usesMelee)
            MeleeAttack();
        else if (_distanceToTarget <= _rangedRange && _usesRanged)
            RangedAttack();
    }
    protected IEnumerator AttackCooldown(float time)
    {
        _attackIsOnCooldown = true;
        _rigidbody.velocity = Vector3.zero;
        yield return new WaitForSeconds(time);
        _attackIsOnCooldown = false;
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
        _target = _targets.OrderByDescending(t => t.Value).FirstOrDefault().Key;
        if (_target == null)
            return;
        if (_targets[_target] < 0)
            _target = null;
    }
    //There are some issues here
    //Need to test this a lot
    protected int GetTargetValue(IHealth target)
    {
        int value = -10000;
        float distance = target.ParentGameObject.transform.position.DistanceTo2D(transform.position);

        //Is in range
        if ((_usesMelee && distance <= _meleeRange) || (_usesRanged && distance <= _rangedRange))
            value = 0;

        //Melee/Ranged
        if ((_usesMelee && distance <= _meleeRange))
            value += 200;
        if (_usesRanged && distance <= _rangedRange)
            value += 100;

        //Is player
        bool isPlayer = target is Player;
        if (isPlayer && _prioritizesPlayer)
            value += 1000;
        else if(isPlayer && !_prioritizesPlayer)
            value = 10;

        //Is building
        bool isBuilding = target is Building;
        if (isBuilding && _attacksBuildings && !_prioritizesPlayer)
            value += 500;
        else if (isBuilding && !_attacksBuildings)
            value = -10000;

        return value;
    }
    protected void UpdateDistanceToTarget()
    {
        _distanceToTarget = _target != null ? _target.ParentGameObject.transform.position.DistanceTo2D(transform.position) : -1;
    }

    #region >>> Melee <<<

    [Header("Melee")]
    [SerializeField]
    protected bool _usesMelee = false;

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
        if (_target == null)
            return;
        _target.GetDamaged(_meleeDamage);
        StartCoroutine(AttackCooldown(_meleeAttackCooldown));
    }

    #endregion

    #region >>> Ranged <<<

    [Header("Ranged")]
    [SerializeField]
    protected ProjectileProfile _projectileProfile;

    [SerializeField]
    protected bool _usesRanged = false;

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
        if (_target == null || _projectileProfile == null)
            return;
        Projectile projectile = GameManager.Instance.ProjectileManager.GetProjectile(_projectileProfile);
        projectile.InitializeProjectile(_target.ParentGameObject.transform.position, transform.position, _rangedDamage);
        StartCoroutine(AttackCooldown(_rangedAttackCooldown));
    }

    #endregion

    #region >>> Target Detection <<<

    [Header("Target Detection")]
    [SerializeField]
    protected Detector _playerDetector;
    [SerializeField]
    protected Detector _buildingDetector;

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
    protected void UpdateDetectors()
    {
        if(_rigidbody.velocity == Vector3.zero || _playerDetector == null)
            return;
        _playerDetector.Rotate(_rigidbody.velocity.normalized);
        if(_attacksBuildings && _buildingDetector != null)
            _buildingDetector.Rotate(_rigidbody.velocity.normalized);
    }

    #endregion

    #endregion
}

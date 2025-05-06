using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SimpleEnemyProfile", menuName = "SimpleEnemy Profile")]
public class SimpleEnemyProfile : EnemyProfile
{
    [Header("Combat")]
    [SerializeField]
    public bool StopsMovementAfterAttack = true;
    [Header("Building Attack")]
    [SerializeField]
    public bool AttacksBuildings = false;
    [SerializeField]
    public bool PrioritizesPlayer = true;



    [Header("Melee")]
    [SerializeField]
    public bool UsesMelee = false;

    [SerializeField]
    [Range(0f, 10f)]
    public float MeleeAttackCooldown = 2f;

    [SerializeField]
    [Range(1, 300)]
    public int MeleeDamage = 10;

    [SerializeField]
    [Range(0.1f, 3f)]
    public float MeleeRange = 0.5f;

    [Header("Ranged")]
    [SerializeField]
    public ProjectileProfile ProjectileProfile;

    [SerializeField]
    public bool UsesRanged = false;

    [SerializeField]
    [Range(0f, 10f)]
    public float RangedAttackCooldown = 2f;

    [SerializeField]
    [Range(1, 300)]
    public int RangedDamage = 10;

    [SerializeField]
    [Range(1f, 10f)]
    public float RangedRange = 3f;
}

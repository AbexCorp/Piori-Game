using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerProfile", menuName = "Tower Profile")]
public class TowerProfile : ScriptableObject
{
    public string UniqueID;

    [Header("Health")]
    public int HealthMax = 50;

    [Header("Combat")]
    public bool Shoots = true;
    public Tower.TowerAttackType AttackType = Tower.TowerAttackType.Hitscan;
    public ProjectileProfile ProjectileProfile;

    public float Range = 4;
    public bool HasCooldown = true;
    public float AttackCooldown = 0.3f;
    public int Damage = 20;
}

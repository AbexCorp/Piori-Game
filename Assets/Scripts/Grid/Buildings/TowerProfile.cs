using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerProfile", menuName = "Buildings/Tower Profile")]
public class TowerProfile : BuildingProfile
{
    [Header("Combat")]
    public bool Shoots = true;
    public Tower.TowerAttackType AttackType = Tower.TowerAttackType.Hitscan;
    public ProjectileProfile ProjectileProfile;

    public float Range = 4;
    public bool HasCooldown = true;
    public float AttackCooldown = 0.3f;
    public int Damage = 20;

    public override bool CheckIfCanBuild(GridTile tile, out string reason)
    {
        if(base.CheckIfCanBuild(tile, out string r) == false)
        {
            reason = r;
            return false;
        }
        reason = "";
        return true;
    }
}

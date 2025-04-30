using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerProjectile : Projectile
{
    protected override void ProjectileHit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            other.gameObject.GetComponent<Enemy>().GetDamaged(_damage);
    }
}

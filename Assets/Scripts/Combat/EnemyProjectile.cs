using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    protected override void ProjectileHit(Collider other)
    {
        if(other.gameObject.TryGetComponent<IHealth>(out IHealth target))
        {
            target.GetDamaged(_damage);
        }
    }
}

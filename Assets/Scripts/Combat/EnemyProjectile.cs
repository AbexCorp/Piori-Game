using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    protected override void ProjectileHit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            GameManager.Instance.Player.GetDamaged(_damage);
    }
}

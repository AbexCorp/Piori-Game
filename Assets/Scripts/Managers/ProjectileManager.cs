using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    private List<Projectile> _projectiles = new();

    public Projectile GetProjectile(Projectile projectilePrefab)
    {
        Projectile projectile = null;
        foreach(var p in _projectiles)
        {
            if( p.IsUsed == false && p.UniqueID == projectilePrefab.UniqueID)
            {
                return p;
            }
        }

        projectile = Instantiate(projectilePrefab);
        _projectiles.Add(projectile);
        return projectile;
    }
}

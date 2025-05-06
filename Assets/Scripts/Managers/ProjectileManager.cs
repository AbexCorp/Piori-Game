using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField]
    private Projectile _projectilePrefab;

    private List<Projectile> _projectiles = new();

    public Projectile GetProjectile(ProjectileProfile projectileProfile)
    {
        Projectile projectile = null;
        foreach(var p in _projectiles)
        {
            if( p.IsUsed == false && p.UniqueID == projectileProfile.UniqueID)
            {
                return p;
            }
        }

        projectile = Instantiate(_projectilePrefab);
        projectile.Load(projectileProfile);
        _projectiles.Add(projectile);
        return projectile;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileProfile", menuName = "Projectile Profile")]
public class ProjectileProfile : ScriptableObject
{
    [Header("Prefab")]
    public string UniqueID;

    [Header("Projectile")]
    public float Speed = 2f;

    [Header("Collision")]
    public LayerMask CollisionMask;
    [SerializeField]
    public LayerMask HitMask;

    [Header("Lifetime")]
    [SerializeField]
    public Projectile.ProjectileLifetimeType LifetimeType = Projectile.ProjectileLifetimeType.Distance;

    [SerializeField]
    public float LifetimeDistance = 4;
    [SerializeField]
    public float LifetimeTime = 2;
}

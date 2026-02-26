using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProfile : ScriptableObject
{
    public string UniqueName;

    [Header("Spawning")]
    public int Tier = 1;
    public int Cost = 50;

    [Header("Movement")]
    [SerializeField]
    public float Speed = 1;
    [SerializeField]
    public LayerMask GroundMask;
    [SerializeField]
    public Pathfinding.PathfindingType PathfindingType = Pathfinding.PathfindingType.Walkable;

    [Header("Health")]
    [SerializeField]
    public int HealthMax = 50;

    [Header("Visual")]
    [SerializeField]
    public Texture Texture = null;
}

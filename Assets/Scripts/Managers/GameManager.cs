using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private Player _player;
    public Player Player => _player;

    [SerializeField]
    private GridManager _gridManager;
    public GridManager GridManager => _gridManager;

    [SerializeField]
    private ProjectileManager _projectileManager;
    public ProjectileManager ProjectileManager => _projectileManager;

    [SerializeField]
    private EnemyManager _enemyManager;
    public EnemyManager EnemyManager => _enemyManager;

    [SerializeField]
    private Level _level;
    public Level Level => _level;

    protected override void OnAwake()
    {
        if (_player == null)
            Debug.LogWarning("Player refference is not assigned");
        if (_gridManager == null)
            Debug.LogWarning("Grid Manager refference is not assigned");
        if(_projectileManager == null)
            Debug.LogWarning("Projectile Manager refference is not assigned");
        if(_enemyManager == null)
            Debug.LogWarning("Enemy Manager refference is not assigned");
    }
}

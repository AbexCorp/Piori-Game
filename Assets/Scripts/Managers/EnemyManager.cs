using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    //Do here object pooling like projectiles
    private List<Enemy> _spawnedEnemies = new();
    public List<Enemy> SpawnedEnemies => new List<Enemy>(_spawnedEnemies);

    public void OnEnemySpawn(Enemy spawnedEnemy)
    {
        _spawnedEnemies.Add(spawnedEnemy);
    }
    public void OnEnemyDeath(Enemy deadEnemy)
    {
        _spawnedEnemies.Remove(deadEnemy);
    }
}

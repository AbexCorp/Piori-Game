using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private Enemy _enemyPrefab;

    public enum SpawnStyle
    {
        Everywhere = 0,
        Top = 1,
        Right = 2,
        Down = 3,
        Left = 4
    }

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



    public void SpawnWave(Level.Wave wave)
    {
        List<EnemyProfile> enemies = BuyEnemies(wave);
        foreach(var enemyToSpawn in enemies)
        {
            GridTile tile = GameManager.Instance.GridManager.Grid.GetRandomBorderTileWalkable();
            Enemy enemy = Instantiate(_enemyPrefab, tile.gameObject.transform.position, Quaternion.identity);
            enemy.Load(enemyToSpawn);
            OnEnemySpawn(enemy);
        }
    }
    public List<EnemyProfile> BuyEnemies(Level.Wave wave, int checkAmount = 10)
    {
        List<KeyValuePair<int, List<EnemyProfile>>> spawnList = new();

        for(int i = 0; i < checkAmount; i++)
        {
            int cost = 0;
            List<EnemyProfile> spawns = new();
            var enemies = wave.Template.PossibleEnemies;
            while(cost < wave.Budget)
            {
                int counter = 4;
                enemies = enemies.OrderBy(x => UnityEngine.Random.value).ToList();
                var enemy = enemies.FirstOrDefault();
                if (enemy.Cost + cost < wave.Budget * 1.1f)
                {
                    spawns.Add(enemy);
                    cost += enemy.Cost;
                }
                else
                {
                    counter--;
                    if (counter < 0)
                        break;
                }
            }
            spawnList.Add(new KeyValuePair<int, List<EnemyProfile>>(cost, spawns));
        }

        return spawnList.OrderByDescending(x => x.Key).FirstOrDefault().Value;
    }
}

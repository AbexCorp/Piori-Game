using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveTemplate", menuName = "Spawn/Wave Template")]
public class WaveTemplate : ScriptableObject
{
    [SerializeField]
    private List<EnemyProfile> _possibleEnemies = new();
    public List<EnemyProfile> PossibleEnemies => _possibleEnemies;
}

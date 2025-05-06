using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Spawn/Level")]
public class Level : ScriptableObject
{
    [Serializable]
    public struct Wave
    {
        [SerializeField]
        public WaveTemplate Template;
        [SerializeField]
        public int Budget;
        [SerializeField]
        public EnemyManager.SpawnStyle SpawnStyle;
    }

    [SerializeField]
    private List<Wave> _waves = new();
    public List<Wave> Waves => _waves;
}

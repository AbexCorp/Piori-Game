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

    protected override void OnAwake()
    {
        if (_player == null)
            Debug.LogWarning("Player refference is not assigned");
        if (_gridManager == null)
            Debug.LogWarning("Grid Manager refference is not assigned");
    }
}

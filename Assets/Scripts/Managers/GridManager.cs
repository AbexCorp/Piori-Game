using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GridManager : Singleton<GridManager>
{
    [SerializeField]
    private GameObject _gridContainer; //Stores spawned tiles in hierarchy

    private Grid _grid;
    public Grid Grid => _grid;


    protected override void OnAwake()
    {
        InitializeGrid();
    }
    private void Update()
    {
        if(SpawnEnemy) //debug
            CreateEnemy(); //debug
    }
    private void InitializeGrid()
    {
        if (_gridContainer == null)
            _gridContainer = gameObject;

        _grid = new();
        _grid.SetTilePrefab(_tilePrefab); //debug
        _grid.InitializeGrid(_gridContainer);
    }


    #region >>> Player <<<

    private GridTile _playerPosition = null;
    public GridTile PlayerPosition => _playerPosition;
    public UnityEvent OnPlayerPositionChanged;

    public void ChangePlayerPosition(GridTile tile)
    {
        if (tile == _playerPosition)
            return;
        _playerPosition = tile;
        OnPlayerPositionChanged?.Invoke();
    }

    #endregion


    #region >>> Effects <<<

    public void ClearAllGridEffects()
    {
        for(int x = 0; x < Grid.Width; x++)
        {
            for(int y = 0; y < Grid.Height; y++)
            {
                Grid[x, y].DisableEffect();
            }
        }
    }

    #endregion


    #region >>> Debug <<<

    [SerializeField]
    private GridTile _tilePrefab;

    public Building TowerPrefab;
    public TowerProfile TowerProfile;
    public bool IsBuilding = false;

    public Enemy EnemyPrefab;
    public SimpleEnemyProfile SimpleEnemyProfile;
    public bool SpawnEnemy = false;
    private void CreateEnemy()
    {
        if (SpawnEnemy == false)
            return;
        GridTile t = Grid[Grid.Width-1, Grid.Height-1];
        var enemy = Instantiate(EnemyPrefab, t.transform.position, Quaternion.identity);
        enemy.Load(SimpleEnemyProfile);
        GameManager.Instance.EnemyManager.OnEnemySpawn(enemy);
        SpawnEnemy = false;
    }

    #endregion
}

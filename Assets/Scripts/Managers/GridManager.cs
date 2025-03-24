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


    #region >>> Debug <<<

    [SerializeField]
    private GridTile _tilePrefab;

    public Building BuildingPrefab;
    public bool IsBuilding = false;

    #endregion
}

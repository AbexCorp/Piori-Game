using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    #region >>> Debug <<<

    [SerializeField]
    private GridTile _tilePrefab;

    public Building BuildingPrefab;
    public bool IsBuilding = false;

    #endregion
}

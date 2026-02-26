using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GridManager : Singleton<GridManager>
{
    [Header("Grid Manager")]
    [SerializeField]
    private GridTile _tilePrefab;
    public GridTile TilePrefab => _tilePrefab;

    [SerializeField]
    private GameObject _gridContainer; //Stores spawned tiles in hierarchy

    private Grid _grid;
    public Grid Grid => _grid;

    [Header("Map Border")]
    [SerializeField]
    private BoxCollider _mapBorderLeft;
    [SerializeField]
    private BoxCollider _mapBorderRight;
    [SerializeField]
    private BoxCollider _mapBorderBottom;
    [SerializeField]
    private BoxCollider _mapBorderTop;


    protected override void OnAwake()
    {
        InitializeGrid();
        CreateGridBorder();
    }

    private void InitializeGrid()
    {
        if (_gridContainer == null)
            _gridContainer = gameObject;

        _grid = new();
        _grid.InitializeGrid(_gridContainer, GridWidth, GridHeight);
    }
    private void CreateGridBorder()
    {
        _mapBorderLeft.size = new Vector3(1, 3, Grid.Height);
        _mapBorderLeft.center = new Vector3(-1, 1, (Grid.Height/2)-0.5f);

        _mapBorderRight.size = new Vector3(1, 3, Grid.Height);
        _mapBorderRight.center = new Vector3(Grid.Width, 1, (Grid.Height/2)-0.5f);

        _mapBorderBottom.size = new Vector3(Grid.Width+2, 3, 1);
        _mapBorderBottom.center = new Vector3((Grid.Width/2)-0.5f, 1, -1);

        _mapBorderTop.size = new Vector3(Grid.Width+2, 3, 1);
        _mapBorderTop.center = new Vector3((Grid.Width/2)-0.5f, 1, Grid.Height);
    }


    #region >>> Player <<<

    [Header("Events")]
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

    [Header("Debug")]

    public int GridWidth = 10;
    public int GridHeight = 10;

    public string[] MapBlocades;
    public bool GetTileBlocade(int x, int y)
    {
        int mapHeight = MapBlocades?.Length ?? 0;

        if (y < 0 || y >= mapHeight)
            return true;

        int rowIndex = mapHeight - 1 - y;

        if (rowIndex < 0 || rowIndex >= MapBlocades.Length)
            return true;

        string row = MapBlocades[rowIndex];

        if (string.IsNullOrEmpty(row) || x < 0 || x >= row.Length)
            return true;

        char tile = row[x];

        if (tile == '0')
            return false;

        if (tile == '1')
            return true;

        if (tile == '2') //temp resource tile
        {
            return false;
        }

        return true;
    }

    #endregion
}

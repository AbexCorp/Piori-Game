using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int _width = 10;
    private int _height = 10;
    public int Width => _width;
    public int Height => _height;

    private GridTile[] _tiles;

    public void InitializeGrid(GameObject container)
    {
        _tiles = new GridTile[Width * Height];

        for(int y = 0; y < Height; y++)
        {
            for(int x = 0; x < Width; x++)
            {
                GridTile tile = GameObject.Instantiate(GetTilePrefab(x, y), container.transform);
                tile.Initialize(x, y, this);
                tile.gameObject.transform.localPosition = new Vector2(x, y);
                _tiles[CoordinateToIndex(x, y)] = tile;
            }
        }
    }


    #region >>> Tile Retrieval <<<

    public GridTile GetTile(int x, int y)
    { 
        return _tiles[CoordinateToIndex(x, y)];
    }
    public GridTile GetTile(Vector2Int v)
    { 
        return _tiles[CoordinateToIndex(v.x, v.y)];
    }
    public GridTile this[int x, int y]
    { 
        get {  return _tiles[CoordinateToIndex(x, y)]; } 
    }
    public GridTile this[Vector2Int v]
    { 
        get { return _tiles[CoordinateToIndex(v.x, v.y)]; }
    }

    private int CoordinateToIndex(int x, int y)
    {
        return y * Width + x;
    }

    #endregion


    #region >>> Debug <<<

    [SerializeField]
    private GridTile _tilePrefab;
    private GridTile GetTilePrefab(int x, int y)
    {
        return _tilePrefab;
    }
    public void SetTilePrefab(GridTile prefab)
    {
        _tilePrefab = prefab;
    }

    #endregion
}

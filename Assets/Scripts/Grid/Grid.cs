using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid
{
    private int _width = 10;
    private int _height = 10;
    public int Width => _width;
    public int Height => _height;

    private GridTile[] _tiles;
    private List<GridTile> _borderTiles = new();

    public void InitializeGrid(GameObject container, int width, int height)
    {
        _width = width;
        _height = height;
        _tiles = new GridTile[Width * Height];

        for(int y = 0; y < Height; y++)
        {
            for(int x = 0; x < Width; x++)
            {
                //GridTile tile = GameObject.Instantiate(GridManager.Instance.TilePrefab, container.transform);
                //tile.Load(GetTileProfile(int x, int y));
                GridTile tile = GameObject.Instantiate(GridManager.Instance.TilePrefab, container.transform);
                tile.Initialize(x, y, this);
                tile.gameObject.transform.localPosition = new Vector3(x, 0, y);
                _tiles[CoordinateToIndex(x, y)] = tile;
            }
        }

        foreach(var t in _tiles)
        {
            t.NavigationNode.ConnectNeighboringTiles();
        }

        FindBorderTiles();
    }
    private void FindBorderTiles()
    {
        foreach(var t in _tiles)
        {
            if(t.X == 0 || t.X == Width - 1 || t.Y == 0 || t.Y == Height - 1)
                _borderTiles.Add(t);
        }
    }
    private GridTile GetTileProfile(int x, int y)
    {
        return GridManager.Instance.TilePrefab;
    }


    #region >>> Tile Retrieval <<<

    public GridTile GetTile(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
            return null;
        return _tiles[CoordinateToIndex(x, y)];
    }
    public GridTile GetTile(Vector2Int v)
    {
        if (v.x < 0 || v.y < 0 || v.x >= Width || v.y >= Height)
            return null;
        return _tiles[CoordinateToIndex(v.x, v.y)];
    }
    public GridTile this[int x, int y]
    {
        get
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return null;
            return _tiles[CoordinateToIndex(x, y)];
        } 
    }
    public GridTile this[Vector2Int v]
    {
        get
        {
            if (v.x < 0 || v.y < 0 || v.x >= Width || v.y >= Height)
                return null;
            return _tiles[CoordinateToIndex(v.x, v.y)];
        }
    }

    public GridTile GetRandomBorderTile()
    {
        return _borderTiles[UnityEngine.Random.Range((int)0, (int)_borderTiles.Count)];
    }
    public GridTile GetRandomBorderTileWalkable()
    {
        return _borderTiles.Where(x => x.IsOccupied == false).OrderBy( x => UnityEngine.Random.value).FirstOrDefault();
    }



    private int CoordinateToIndex(int x, int y)
    {
        return y * Width + x;
    }

    #endregion
}

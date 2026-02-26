using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationNode
{
    private GridTile _tile;
    public GridTile Tile => _tile;

    private List<NavigationNode> _neighbors;
    public List<NavigationNode> Neighbors => _neighbors;


    public NavigationNode(GridTile tile)
    {
        _tile = tile;
    }

    public NavigationNode Connection { get; set; }

    private int _g;
    public int StepsFromStart { get { return _g; } set { _g = value; } }
    public int G { get { return _g; } set { _g = value; } }

    private int _h;
    public int StepsToEnd { get { return _h; } set { _h = value; } }
    public int H { get { return _h; } set { _h = value; } }

    private int _f;
    public int Score { get { return _f; } set { _f = value; } }
    public int F { get { return _f; } set { _f = value; } }


    public void ConnectNeighboringTiles()
    {
        _neighbors = new();
        List<Vector2Int> directions = new() { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left};
        foreach (var direction in directions)
        {
            GridTile t = GridManager.Instance.Grid.GetTile(direction + Tile.GridPosition);
            if (t != null)
                Neighbors.Add(t.NavigationNode);
        }
    }
    public static int GetDistanceTo(NavigationNode from, NavigationNode to)
    {
        return Mathf.Abs(from.Tile.X - to.Tile.X) + Mathf.Abs(from.Tile.Y - to.Tile.Y);
    }
}

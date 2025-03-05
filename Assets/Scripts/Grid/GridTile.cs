using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(BoxCollider2D))]
public class GridTile : MonoBehaviour
{
    private Grid _grid;
    private int _x;
    private int _y;

    public Grid Grid => _grid;
    public int X => _x;
    public int Y => _y;

    public bool IsOccupied => IsOccupiedByBuilding; //Add checks for characters on tile;

    public void Initialize(int x, int y, Grid grid)
    {
        _grid = grid;
        _x = x;
        _y = y;
        gameObject.name = $"Tile ({x},{y})";
    }

    #region >>> Building <<<

    private Building _building;
    public Building Building => _building;
    public bool IsOccupiedByBuilding => _building != null;

    public bool Build(Building building)
    {
        if(IsOccupied) 
            return false;

        _building = building;
        _building.GetBuilt(this);
        return true;
    }

    public bool DestroyBuilding()
    {
        if(!IsOccupiedByBuilding)
            return false;

        _building.GetDestroyed();
        _building = null;
        return true;
    }

    public void ClearAssignedBuilding()
    {
        _building = null;
    }

    #endregion


    #region >>> Debugs <<<

    protected void OnMouseDown()
    {
        if (GameManager.Instance.GridManager.IsBuilding == false)
            return;

        Build(Instantiate(GameManager.Instance.GridManager.BuildingPrefab));
    }

    #endregion
}

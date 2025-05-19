using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridTile : MonoBehaviour, IMouseInteractable
{
    private Grid _grid;
    private int _x;
    private int _y;

    public Grid Grid => _grid;
    public int X => _x;
    public int Y => _y;
    public Vector2Int GridPosition => new Vector2Int(X, Y);
    public Vector3 WorldPosition3D => transform.position;
    public Vector2 WorldPosition2D => new Vector2(transform.position.x, transform.position.z);


    public bool IsOccupied => IsOccupiedByBuilding; //Add checks for characters on tile;
    [SerializeField]
    private bool _isWalkable = true;
    public bool IsWalkable => _isWalkable;

    public void Initialize(int x, int y, Grid grid)
    {
        _grid = grid;
        _x = x;
        _y = y;
        gameObject.name = $"Tile ({x},{y})";

        _navigationNode = new(this);
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


    #region >>> Navigation <<<

    private NavigationNode _navigationNode;
    public NavigationNode NavigationNode => _navigationNode;

    #endregion


    #region >>> Effect <<<

    [SerializeField]
    private Renderer _renderer;
    
    public void SetEffectCoor(Color color)
    {
        _renderer.material.SetColor("_EffectColor", color);
        EnableEffect();
    }
    public void EnableEffect()
    {
        _renderer.material.SetInt("_ShowEffect", 1);
    }
    public void DisableEffect()
    {
        _renderer.material.SetInt("_ShowEffect", 0);
    }

    #endregion


    #region >>> IMouseInteractable <<<

    public void OnHoverEnter(InputAction.CallbackContext context)
    {
        if (context.performed)
            EnableEffect();
    }

    public void OnHoveExit(InputAction.CallbackContext context)
    {
        if (context.performed)
            DisableEffect();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (GameManager.Instance.GridManager.IsBuilding == false)
                return;
            if (_building != null)
                return;

            if (!GameManager.Instance.GridManager.TowerProfile.CheckIfCanBuild(this))
                return;
            Building b = GameManager.Instance.GridManager.TowerPrefab;
            b.Load(GameManager.Instance.GridManager.TowerProfile);
            Build(Instantiate(b));
        }
    }

    #endregion
}

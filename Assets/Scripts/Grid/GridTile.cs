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


    public bool IsOccupied => IsOccupiedByBuilding || !IsWalkable; //Add checks for characters on tile;
    [SerializeField]
    private bool _isWalkable = true;
    public bool IsWalkable => _isWalkable;

    public void Initialize(int x, int y, Grid grid)
    {
        _grid = grid;
        _x = x;
        _y = y;
        gameObject.name = $"Tile ({x},{y})";

        //////////////DEBUG
            _isWalkable = !GameManager.Instance.GridManager.GetTileBlocade(x, y);
            if (!IsWalkable)
            {
                EnableOccupy();
                TurnOnCollider();
            }
        //////////////DEBUG

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
        GameManager.Instance.BuildingManager.SelectedProfile.AdjustCost();
        GameManager.Instance.BuildingManager.StopBuilding();
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

    public bool SellBuilding()
    {
        if (!IsOccupied)
            return false;

        int cashback = Mathf.Clamp((int)System.MathF.Round(_building.Cost * GameManager.Instance.Player.BuildingSellingReturn), 0, int.MaxValue);
        GameManager.Instance.AnalyticsManager.OnBuildingSold(_building, this, cashback);
        GameManager.Instance.ResourceManager.AddResource(cashback);
        _building.GetDestroyed();
        _building = null;
        GameManager.Instance.BuildingManager.StopSelling();
        return true;
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
    }
    public void EnableEffect()
    {
        _renderer.material.SetInt("_ShowEffect", 1);
    }
    public void DisableEffect()
    {
        _renderer.material.SetInt("_ShowEffect", 0);
    }

    public void EnableOccupy()
    {
        _renderer.material.SetInt("_IsOccupied", 1);
    }
    public void DisableOccupy()
    {
        _renderer.material.SetInt("_IsOccupied", 0);
    }

    #endregion


    #region >>> IMouseInteractable <<<

    public void OnHoverEnter(InputAction.CallbackContext context)
    {
        if (context.performed)
            EnableEffect();
    }

    public void OnHoverExit(InputAction.CallbackContext context)
    {
        if (context.performed)
            DisableEffect();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (GameManager.Instance.BuildingManager.IsBuilding == true)
            {
                if (!GameManager.Instance.BuildingManager.SelectedProfile.CheckIfCanBuild(this, out string reason))
                {
                    GameManager.Instance.InterfaceManager.DisplayTextMessage(reason);
                    return;
                }
                Building b = Instantiate(GameManager.Instance.BuildingManager.BuildingPrefab, gameObject.transform.position, Quaternion.identity);
                b.Load(GameManager.Instance.BuildingManager.SelectedProfile);
                Build(b);
            }
            else if (GameManager.Instance.BuildingManager.IsSelling == true)
            {
                if (_building == null)
                {
                    GameManager.Instance.InterfaceManager.DisplayTextMessage("Nothing to sell");
                    return;
                }
                if(WorldPosition3D.DistanceTo2D(GameManager.Instance.Player.transform.position) > GameManager.Instance.Player.MaxBuildDistance)
                {
                    GameManager.Instance.InterfaceManager.DisplayTextMessage("To far from you");
                    return;
                }

                SellBuilding();
            }
        }
    }

    #endregion


    #region Debug

    [SerializeField]
    private GameObject _colliderObject;
    private void TurnOnCollider()
    {
        _colliderObject.SetActive(true);
    }
    private void TurnOffCollider()
    {
        _colliderObject.SetActive(false);
    }

    #endregion
}

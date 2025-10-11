using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingProfile : ScriptableObject
{
    public string UniqueID;

    [Header("Health")]
    public int HealthMax = 50;
    [Header("Resources")]
    public int BaseCost = 50;
    [Range(0.0f, 1.0f)]
    public float CostAdjustmentPercentage = 0;

    [Header("Visual")]
    public Texture Texture = null;

    public virtual bool CheckIfCanBuild(GridTile tile, out string reason)
    {
        if(tile.IsOccupiedByBuilding) //Enpty from buildings
        {
            reason = "Already occupied by another building";
            return false;
        }
        if (tile.IsWalkable == false) //Enpty from environment
        {
            reason = "You can't build in this spot";
            return false;
        }
        if (Cost > GameManager.Instance.ResourceManager.Resource) //Have resource to build
        {
            reason = "Not enough resources";
            return false;
        }
        
        //Empty from player and enemy
        if(Physics.BoxCast(
            center: new Vector3(tile.WorldPosition3D.x, tile.WorldPosition3D.y + 3, tile.WorldPosition3D.z),
            halfExtents: new Vector3(0.41f, 0.5f, 0.41f),
            direction: Vector3.down,
            orientation: tile.gameObject.transform.rotation,
            layerMask: LayerMask.GetMask("Player", "Enemy"),
            maxDistance: 4f))
        {
            reason = "This area is occupied right now";
            return false;
        }

        //To far from player
        if(tile.WorldPosition3D.DistanceTo2D(GameManager.Instance.Player.transform.position) > GameManager.Instance.Player.MaxBuildDistance)
        {
            reason = "To far from you";
            return false;
        }

        //Here add other conditions ex: tile is of type X, or tile is next to tile with Y
        reason = "";
        return true;
    }


    #region Cost Adjustment

    protected int CostAdjustment = 0;
    public int Cost => BaseCost + CostAdjustment;
    public void AdjustCost()
    {
        CostAdjustment = (int)((((CostAdjustmentPercentage + 1f) * Cost)) - BaseCost);
    }

    #endregion
}

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
    [Header("Building Requirements")]
    public AdditionalBuildingRequirement SpecialBuildingRequirement = AdditionalBuildingRequirement.None;
    [Header("Other")]
    [Range(0.0f, 1.0f)]
    public float CostAdjustmentPercentage = 0;
    
    

    [Header("Visual")]
    public Texture Texture = null;
    public Sprite UIIcon = null;


    #region >>> Building Rquirements <<<

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

        //Additional Requirements
        if(CheckAdditionalBuildingRequirement(SpecialBuildingRequirement, tile) == false)
        {
            reason = BuildingRequirementToReason(SpecialBuildingRequirement);
            return false;
        }


        //Here add other conditions ex: tile is of type X, or tile is next to tile with Y
        reason = "";
        return true;
    }
    private bool CheckAdditionalBuildingRequirement(AdditionalBuildingRequirement v, GridTile tile)
    {
        if (v == AdditionalBuildingRequirement.None)
            return true;

        switch (v)
        {
            case AdditionalBuildingRequirement.NextToTerrain:
                return CheckNextToTerrain(tile);

            case AdditionalBuildingRequirement.OnResourceTileDEBUG:
                return CheckOnResourceTile(tile);

            default:
                return false;
        }
    }
    #region Requirements

    private bool CheckNextToTerrain(GridTile tile)
    {
        int rows = GameManager.Instance.GridManager.MapBlocades.Length;
        int cols = GameManager.Instance.GridManager.MapBlocades[0].Length;

        var directions = new (int, int)[] { (-1, 0), (1, 0), (0, -1), (0, 1) };

        foreach (var dir in directions)
        {
            int newY = tile.Y + dir.Item1;
            int newX = tile.X + dir.Item2;

            if (newY >= 0 && newY < rows && newX >= 0 && newX < cols)
            {
                if (GameManager.Instance.GridManager.MapBlocades[rows - 1 - newY][newX] == '1')
                {
                    return true;
                }
            }
        }
        return false;
    }
    private bool CheckOnResourceTile(GridTile tile)
    {
        return GameManager.Instance.GridManager.MapBlocades[GameManager.Instance.GridManager.MapBlocades.Length - 1 - tile.Y][tile.X] == '2'; //Y is flipped here
    }

    #endregion

    public enum AdditionalBuildingRequirement
    {
        None = 0,
        NextToTerrain = 1,
        OnResourceTileDEBUG = 999
    }
    private string BuildingRequirementToReason(AdditionalBuildingRequirement v)
    {
        switch (v)
        {
            default:
                return "No text for this fail";

            case AdditionalBuildingRequirement.None:
                return "0 0 0";

            case AdditionalBuildingRequirement.NextToTerrain:
                return "Must be built next to the terrain";

            case AdditionalBuildingRequirement.OnResourceTileDEBUG:
                return "Must be built on a resource tile";
        }
    }

    #endregion


    #region Cost Adjustment

    protected int CostAdjustment = 0;
    public int Cost => BaseCost + CostAdjustment;
    public void AdjustCost()
    {
        CostAdjustment = (int)((((CostAdjustmentPercentage + 1f) * Cost)) - BaseCost);
    }

    #endregion
}

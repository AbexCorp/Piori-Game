using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingProfile : ScriptableObject
{
    public string UniqueID;

    [Header("Health")]
    public int HealthMax = 50;
    [Header("Resources")]
    public int Cost = 50;

    public virtual bool CheckIfCanBuild(GridTile tile)
    {
        if(Cost > GameManager.Instance.ResourceManager.Resource || tile.IsOccupied)
            return false;
        //Here add different conditions ex: tile is of type X, or tile is next to tile with Y
        return true;
    }
}

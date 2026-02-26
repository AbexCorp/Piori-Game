using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceBuildingProfile", menuName = "Buildings/Resource Building Profile")]
public class ResourceBuildingProfile : BuildingProfile
{
    public ResourceBuilding.ResourceProductionType ProductionType = ResourceBuilding.ResourceProductionType.Factory;
    public int ProductionAmount = 1;
    [Range(0.1f, 10f)]
    public float ProductionCooldown = 2.5f;

    public override bool CheckIfCanBuild(GridTile tile, out string reason)
    {
        if(base.CheckIfCanBuild(tile, out string r) == false)
        {
            reason = r;
            return false;
        }
        reason = "";
        return true;
    }
}

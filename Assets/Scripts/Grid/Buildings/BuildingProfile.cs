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
        if(Cost > GameManager.Instance.ResourceManager.Resource || tile.IsOccupied) //Enpty from buildings
            return false;
        
        //Empty from player and enemy
        if(Physics.BoxCast(
            center: new Vector3(tile.WorldPosition3D.x, tile.WorldPosition3D.y + 3, tile.WorldPosition3D.z),
            halfExtents: new Vector3(0.41f, 0.5f, 0.41f),
            direction: Vector3.down,
            orientation: tile.gameObject.transform.rotation,
            layerMask: LayerMask.GetMask("Player", "Enemy"),
            maxDistance: 4f))
        {
            return false;
        }

        //Here add other conditions ex: tile is of type X, or tile is next to tile with Y
        return true;
    }
}

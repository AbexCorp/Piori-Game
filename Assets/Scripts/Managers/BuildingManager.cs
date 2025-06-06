using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    private Building _towerPrefab;
    [SerializeField]
    private Building _resourceBuildingPrefab;


    private bool _buildingIsAllowed = false;
    public bool BuildingIsAllowed => _buildingIsAllowed;

    private bool _isBuilding;
    public bool IsBuilding => _isBuilding;


    private BuildingProfile _selectedProfile = null;
    public BuildingProfile SelectedProfile => _selectedProfile;
    public void SelectProfile(BuildingProfile p)
    {
        _selectedProfile = p;
        ChoosePrefab();
    }
    private Building _buildingPrefab;
    public Building BuildingPrefab => _buildingPrefab;
    private void ChoosePrefab()
    {
        if (_selectedProfile == null)
        {
            _buildingPrefab = null;
            return;
        }

        if (_selectedProfile is TowerProfile)
            _buildingPrefab = _towerPrefab;
        if (_selectedProfile is ResourceBuildingProfile)
            _buildingPrefab = _resourceBuildingPrefab;
    }
    


    public void AllowBuilding()
    {
        _buildingIsAllowed = true;
    }
    public void DisallowBuilding()
    {
        StopBuilding();
        SelectProfile(null);
        _buildingIsAllowed = false;
    }
    public void Build()
    {
        if (BuildingIsAllowed == false)
            return;
        if (_selectedProfile == null)
        {
            ChoosePrefab();
            return;
        }
        ChoosePrefab();
        _isBuilding = true;
    }
    public void StopBuilding()
    {
        _isBuilding = false;
        SelectProfile(null);
    }
}

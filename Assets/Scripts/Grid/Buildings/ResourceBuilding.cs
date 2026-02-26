using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBuilding : Building
{
    [Header("Resource Generation")]
    [SerializeField]
    protected ResourceProductionType _productionType = ResourceProductionType.Factory;
    public enum ResourceProductionType
    {
        Factory = 0, //Generate a small amount of resource
        Mine = 1, //Generate a medium amount of resource, can only be built in resource spots
        Farm = 2, //Expensive and generates very little, can build fields on adjecent tiles if
        Misc = 3
    }
    [SerializeField]
    protected int _productionAmount = 1;
    [SerializeField]
    [Range(0.1f, 10f)]
    protected float _productionCooldown = 2.5f;


    private void Start()
    {
        StartCoroutine(ProduceResources());
    }

    public override void Load(ScriptableObject so)
    {
        if (so is not ResourceBuildingProfile)
            return;
        ResourceBuildingProfile rbp = so as ResourceBuildingProfile;

        _uniqueID = GameManager.Instance.GetUniqueID();
        _uniqueName = rbp.UniqueName;
        gameObject.name = $"ResourceBuilding ({UniqueName})";
        _cost = rbp.Cost;

        _healthMax = rbp.HealthMax;

        _productionType = rbp.ProductionType;
        _productionAmount = rbp.ProductionAmount;
        _productionCooldown = rbp.ProductionCooldown;

        if (rbp.Texture != null)
        {
            _renderer.material.SetTexture("_Texture", rbp.Texture);
        }

        InitializeBuilding();
    }

    protected IEnumerator ProduceResources()
    {
        while (true)
        {
            if(GameManager.Instance.CurrentGameState != GameState.BeforeFirstWave)
                GameManager.Instance.ResourceManager.AddResource(_productionAmount);
            yield return new WaitForSeconds(_productionCooldown);
        }
    }
}

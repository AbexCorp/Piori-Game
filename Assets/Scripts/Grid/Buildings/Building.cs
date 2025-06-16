using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Building : MonoBehaviour, IHealth, ILoadable
{
    [Header("Internal")]
    [SerializeField]
    protected Rigidbody _rigidbody;

    [Header("Resources")]
    [SerializeField]
    protected int _cost = 50;
    public int Cost => _cost;

    private void Awake()
    {
        if(_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody>();
            Debug.LogWarning($"{nameof(Rigidbody)} is not assigned on {nameof(Building)} of {gameObject.name}");
        }
        _rigidbody.isKinematic = true;
        InitializeBuilding();
    }
    protected virtual void InitializeBuilding()
    {
        _healthCurrent = HealthMax;
    }
    public abstract void Load(ScriptableObject so);


    #region >>> Building <<<

    protected GridTile _occupiedTile;

    public virtual void GetBuilt(GridTile tile)
    {
        _occupiedTile = tile;
        _rigidbody.position = tile.gameObject.transform.position;
        GameManager.Instance.ResourceManager.UseResources(_cost);
    }

    public virtual void GetDestroyed()
    {
        if(_occupiedTile != null)
        {
            _occupiedTile.ClearAssignedBuilding();
            _occupiedTile = null;
        }
        Destroy(gameObject);
    }

    #endregion


    #region >>> Health <<<

    [Header("Health")]
    [SerializeField]
    protected int _healthMax = 50;
    public int HealthMax => _healthMax;
    protected int _healthCurrent = 0;
    public int HealthCurrent => _healthCurrent;
    public GameObject ParentGameObject => gameObject;

    [SerializeField]
    private GameObject _healthBarInterface;
    [SerializeField]
    private UnityEngine.UI.Image _healthBar;

    public void GetDamaged(int damage)
    {
        if (damage < 0)
            return;
        _healthCurrent -= damage;
        if(!_healthBarInterface.activeInHierarchy)
            _healthBarInterface.SetActive(true);
        _healthBar.fillAmount = Mathf.Clamp((_healthCurrent / (float)_healthMax), 0, 1);
        Die();
    }
    protected void Die()
    {
        if (_healthCurrent > 0)
            return;
        GetDestroyed();
    }

    #region Repair

    private bool _isBeeingRepaired = false;
    public bool IsBeeingRepaired => _isBeeingRepaired;

    public void Repair(int amount, float time)
    {
        if (IsBeeingRepaired)
            return;
        StartCoroutine(Repairs(amount, time));
    }
    private IEnumerator Repairs(int amount, float time)
    {
        int updatesAmount = 20;
        YieldInstruction yield = new WaitForSeconds(time / updatesAmount);

        _isBeeingRepaired = true;
        _healthBarInterface.SetActive(true);

        for(int i = 0; i < updatesAmount; i++)
        {
            yield return yield;
            _healthCurrent = HealthCurrent + (int)System.MathF.Ceiling(amount / (float)updatesAmount);
            _healthBar.fillAmount = Mathf.Clamp(HealthCurrent / (float)HealthMax, 0, 1);

            if(_healthCurrent >= HealthMax)
            {
                _healthCurrent = HealthMax;
                _healthBarInterface.SetActive(false);
                break;
            }
        }
        _isBeeingRepaired = false;
    }

    #endregion

    #endregion
}

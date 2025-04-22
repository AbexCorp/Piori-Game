using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Building : MonoBehaviour, IHealth
{
    [Header("Internal")]
    [SerializeField]
    protected Rigidbody _rigidbody;

    private void Awake()
    {
        if(_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody>();
            Debug.LogWarning($"{nameof(Rigidbody)} is not assigned on {nameof(Building)} of {gameObject.name}");
        }
        _rigidbody.isKinematic = true;
        InitializeTower();
    }
    protected virtual void InitializeTower()
    {
        _healthCurrent = HealthMax;
    }


    #region >>> Building <<<

    protected GridTile _occupiedTile;

    public virtual void GetBuilt(GridTile tile)
    {
        _occupiedTile = tile;
        _rigidbody.position = tile.gameObject.transform.position;
    }

    public virtual void GetDestroyed()
    {
        _occupiedTile.ClearAssignedBuilding();
        _occupiedTile = null;
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

    public void GetDamaged(int damage)
    {
        if (damage < 0)
            return;
        _healthCurrent -= damage;
        Die();
    }
    protected void Die()
    {
        if (_healthCurrent >= 0)
            return;

        if(_occupiedTile != null)
        {
            _occupiedTile.ClearAssignedBuilding();
            _occupiedTile = null;
        }
        Destroy(gameObject);
    }

    #endregion
}

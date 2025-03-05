using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Building : MonoBehaviour
{
    [Header("Internal")]
    [SerializeField]
    protected Rigidbody2D _rigidbody;

    private void Awake()
    {
        if(_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            Debug.LogWarning($"{nameof(Rigidbody2D)} is not assigned on {nameof(Building)} of {gameObject.name}");
        }
        _rigidbody.isKinematic = true;
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
}

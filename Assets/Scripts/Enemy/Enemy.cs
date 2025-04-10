using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Enemy : MonoBehaviour
{
    [SerializeField]
    protected Rigidbody _rigidbody;


    protected virtual void Start()
    {
        UpdateGridPosition();
        GridManager.Instance.OnPlayerPositionChanged.AddListener(OnPlayerMoved);
        DebugThis();
    }
    protected virtual void Update()
    {
        Move();
    }


    #region >>> Movement <<<

    [SerializeField]
    protected float Speed = 1;
    [SerializeField]
    protected LayerMask _groundMask;

    protected Vector3? _movementTarget = null;
    protected List<NavigationNode> _path = new();
    protected GridTile _gridPosition = null;


    protected void Move()
    {
        if(_movementTarget == null)
        {
            FindNextMovePoint();
            _rigidbody.velocity = Vector3.zero;
            return;
        }

        if (gameObject.transform.position.DistanceTo2D(_movementTarget.Value) < 0.05f)
        {
            _movementTarget = null;
            return;
        }
        _rigidbody.velocity = gameObject.transform.position.DirectionTo2D(_movementTarget.Value) * Speed;
    }
    protected virtual void FindNextMovePoint()
    {
        if(_path == null || _path.Count == 0)
        {
            _movementTarget = null;
            FindPath();
            return;
        }

        _movementTarget = _path.Last().Tile.WorldPosition3D;
        _path.Remove(_path.Last());
    }
    protected virtual void OnPlayerMoved()
    {
        FindPath();
        FindNextMovePoint();
    }
    protected void UpdateGridPosition()
    {
        RaycastHit hit;
        if(Physics.Raycast(origin:transform.position + Vector3.up * 0.1f, direction:Vector3.down, hitInfo:out hit, maxDistance:1f, layerMask: _groundMask.value))
        {
            if(hit.collider.gameObject.TryGetComponent<GridTile>(out GridTile tile))
            {
                _gridPosition = tile;
            }
        }
        else
            _gridPosition = null;
    }

    //Pathfinding
    protected virtual void FindPath()
    {
        UpdateGridPosition();
        FindPathToPlayer();
    }
    protected void FindPathToPlayer()
    {
        _path = Pathfinding.FindPath(_gridPosition?.NavigationNode, GridManager.Instance.PlayerPosition?.NavigationNode);
    }

    #endregion


    #region >>> Life <<<

    [SerializeField]
    protected int _health = 50;
    protected int _currentHealth;

    public virtual void GetDamaged(int damage)
    {
        _currentHealth -= damage;
        Debug.Log($"{gameObject.name} damaged for {damage}, hp = {_currentHealth}");
        Die();
    }
    public virtual void Die()
    {
        if(_currentHealth <= 0)
        {
            Debug.Log($"{gameObject.name} died");
            GameManager.Instance.EnemyManager.OnEnemyDeath(this);
            Destroy(gameObject);
        }
    }

    #endregion


    private void DebugThis()
    {
        _currentHealth = _health;
    }
}

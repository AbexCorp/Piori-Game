using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Enemy : MonoBehaviour, IHealth, ILoadable
{
    protected string _uniqueID;
    public string UniqueID => _uniqueID;

    [SerializeField]
    protected Rigidbody _rigidbody;


    protected virtual void Start()
    {
        UpdateGridPosition();
        GridManager.Instance.OnPlayerPositionChanged.AddListener(OnPlayerMoved);
        InitializeEnemy();
    }
    protected virtual void InitializeEnemy()
    {
        _healthCurrent = HealthMax;
    }
    protected virtual void Update()
    {
        Move();
    }
    public abstract void Load(ScriptableObject so);


    #region >>> Movement <<<

    [SerializeField]
    protected float _speed = 1;
    [SerializeField]
    protected LayerMask _groundMask;
    [SerializeField]
    protected Pathfinding.PathfindingType _pathfindingType = Pathfinding.PathfindingType.Walkable;

    protected Vector3? _movementTarget = null;
    protected List<NavigationNode> _path = new();
    protected GridTile _gridPosition = null;


    protected void Move()
    {
        if (_movementTarget == null)
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
        _rigidbody.velocity = gameObject.transform.position.DirectionTo2D(_movementTarget.Value) * _speed;
    }
    protected virtual void FindNextMovePoint()
    {
        if (_path == null || _path.Count == 0)
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
        if (Physics.Raycast(origin: transform.position + Vector3.up * 0.1f, direction: Vector3.down, hitInfo: out hit, maxDistance: 1f, layerMask: _groundMask.value))
        {
            if (hit.collider.gameObject.TryGetComponent<GridTile>(out GridTile tile))
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
        _path = Pathfinding.FindPath(_gridPosition?.NavigationNode, GridManager.Instance.PlayerPosition?.NavigationNode, _pathfindingType);
    }

    #endregion


    #region >>> Health <<<

    [Header("Health")]
    [SerializeField]
    protected int _healthMax = 50;
    public int HealthMax => _healthMax;
    private int _healthCurrent;
    public int HealthCurrent => _healthCurrent;
    public GameObject ParentGameObject => gameObject;

    [SerializeField]
    private GameObject _healthBarInterface;
    [SerializeField]
    private UnityEngine.UI.Image _healthBar;

    public virtual void GetDamaged(int damage)
    {
        _healthCurrent -= damage;
        if(_healthBarInterface != null && _healthBar != null)
        {
            if(!_healthBarInterface.activeInHierarchy)
                _healthBarInterface.SetActive(true);
            _healthBar.fillAmount = Mathf.Clamp((_healthCurrent / (float)_healthMax), 0, 1);
        }
        Die();
    }
    protected virtual void Die()
    {
        if (_healthCurrent <= 0)
        {
            GameManager.Instance.EnemyManager.OnEnemyDeath(this);
            Destroy(gameObject);
        }
    }

    #endregion


    #region >>> Spawning <<<

    [Header("Spawning")]
    public int _tier = 1;
    public int Tier => _tier;
    public int _cost = 50;
    public int Cost => _cost;

    #endregion
}

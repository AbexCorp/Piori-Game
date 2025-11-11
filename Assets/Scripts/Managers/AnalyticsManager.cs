using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Services.Analytics;

public class AnalyticsManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnGameStateChanged += PlayerMovement;
        GameManager.Instance.OnGameStateChanged += TallyResources;
        GameManager.Instance.OnGameStateChanged += OnGameEnd;
    }


    #region >>> Shortcuts <<<

    private GameState CurrentGameState => GameManager.Instance.CurrentGameState;
    private int CurrentWave => GameManager.Instance.EnemyManager.CurrentWave;
    private int GameTime => GameManager.Instance.GameTime;

    #endregion


    #region >>> Building <<<

    public void OnBuildingBuilt(Building building, GridTile tile, int cost)
    {
        //Debug.Log($"Built {building.name}, {tile.GridPosition}, {cost}, {CurrentGameState}, {CurrentWave}, {GameTime}"); ////////////////////////
    }
    public void OnBuildingKilled(Building building, GridTile tile)
    {
        //Debug.Log($"Killed {building.name}, {tile.GridPosition}, {CurrentGameState}, {CurrentWave}, {GameTime}"); ////////////////////////////
    }
    public void OnBuildingSold(Building building, GridTile tile, int cashback)
    {
        //Debug.Log($"Sold {building.name}, {tile.GridPosition}, {cashback}, {CurrentGameState}, {CurrentWave}, {GameTime}"); ////////////////////////////
    }
    public void OnBuildingDestroyed(Building building, GridTile tile) //Also probably some reason here
    {
        //Debug.Log($"Deleted {building.name}, {tile.GridPosition}, {CurrentGameState}, {CurrentWave}, {GameTime}"); /////////////////////////////
    }

    #endregion


    #region >>> Player <<<

    #region Movement
    private Vector3 _playerPosition;
    private float _moveDistance = 0;
    private Coroutine _movementCalculator;
    private void PlayerMovement(GameState current, GameState old)
    {
        switch (current)
        {

            case GameState.NewWave:
            case GameState.WaveBreak:
            case GameState.Win:
            case GameState.Lose:
                ResetPlayerMovementCounter(current, old);
                return;

            case GameState.BeforeFirstWave:
                StartPlayerMovement();
                return;

            case GameState.GamePreparePhase:
                _playerPosition = GameManager.Instance.Player.transform.position;
                return;
        }
    }
    private void StartPlayerMovement()
    {
        Vector3 newPosition = GameManager.Instance.Player.transform.position;
        if (_movementCalculator == null)
        {
            _playerPosition = newPosition;
            _moveDistance = 0;
            _movementCalculator = StartCoroutine(CalculatePlayerMovement());
        }
    }
    private void ResetPlayerMovementCounter(GameState current, GameState old)
    {
        Vector3 newPosition = GameManager.Instance.Player.transform.position;
        if (_movementCalculator == null)
        {
            _moveDistance += _playerPosition.DistanceTo3D(newPosition);
            //Debug.Log($"move distance {_moveDistance}, {old} {CurrentWave}"); ///////////////////////////////
            _playerPosition = newPosition;
            _moveDistance = 0;
            _movementCalculator = StartCoroutine(CalculatePlayerMovement());
            return;
        }
        StopCoroutine(_movementCalculator);
        _moveDistance += _playerPosition.DistanceTo3D(newPosition);
        //Debug.Log($"move distance {_moveDistance}, {old} {CurrentWave}"); ///////////////////////////////
        _playerPosition = newPosition;
        _moveDistance = 0;
        if(current != GameState.Win && current != GameState.Lose)
            _movementCalculator = StartCoroutine(CalculatePlayerMovement());
    }
    private IEnumerator CalculatePlayerMovement()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            Vector3 newPosition = GameManager.Instance.Player.transform.position;
            _moveDistance += _playerPosition.DistanceTo3D(newPosition);
            _playerPosition = newPosition;
        }
    }
    #endregion

    #region Health
    public void OnPlayerLoseHealth(int damage)
    {
        //Debug.Log($"Player loose health {damage}, {CurrentGameState} {CurrentWave} {GameTime}"); ////////////////////
    }
    #endregion

    #region Secondary Mechanics
    public void OnPlayerShoot()
    {
        //Debug.Log($"Shot: {CurrentGameState}, {CurrentWave}, {GameTime}"); ////////////////////////////////////
    }
    public void OnPlayerRepair(Building building)
    {
        //Debug.Log($"Repair: {building.name}, {CurrentGameState} {CurrentWave} {GameTime}"); /////////////////////////
    }
    #endregion

    #endregion


    #region >>> Resources <<<

    private int _resourcesGained = 0;
    private int _resourcesUsed = 0;
    public void OnGainResource(int amount)
    {
        _resourcesGained += amount;
    }
    public void OnUseResource(int amount)
    {
        _resourcesUsed += amount;
    }
    private void TallyResources(GameState current, GameState old)
    {
        int waveAdjustment = 0;
        switch (current)
        {
            case GameState.Win:
            case GameState.Lose:
                break;
            case GameState.NewWave:
                waveAdjustment -= 1;
                break;
            default:
                return;
        }

        //Debug.Log($"Resources: {_resourcesGained}, {_resourcesUsed}, {CurrentWave + waveAdjustment}"); //////////////////////////
        _resourcesGained = 0;
        _resourcesUsed = 0;
    }

    #endregion


    #region >>> Enemies <<<

    private class EnemyData
    {
        public EnemyData(int id, string name, int wave, int lifeStart)
        {
            ID = id;
            Name = name;
            Wave = wave;
            LifeStart = lifeStart;
        }
        public int ID;
        public string Name;
        public int Wave;
        public int LifeStart;
        public int LifeEnd = -1;
    }

    private Dictionary<int, EnemyData> _enemies = new();
    Dictionary<int, int> _waveCounter = new();
    public void OnEnemyCreate(int id, string name)
    {
        _enemies.Add(id ,new EnemyData(id, name, CurrentWave, GameTime));

        if(_waveCounter.ContainsKey(CurrentWave) == false)
            _waveCounter.Add(CurrentWave, 1);
        else
            _waveCounter[CurrentWave]++;
    }
    public void OnEnemyDeath(int id)
    {
        if (_enemies.ContainsKey(id))
        {
            _enemies[id].LifeEnd = GameTime;
        }

        int wave = _enemies[id].Wave;
        _waveCounter[wave]--;
        if (_waveCounter[wave] <= 0)
        {
            //Debug.Log($"Beat Wave {wave}");
        }
    }

    #endregion


    #region >>> CombatLog <<<

    private List<DamageEvent> _damageEvents = new();
    public struct DamageEvent
    {
        public DamageEvent(int attackerID, string attackerName, int targetID, string targetName, int damage, bool gotKilled)
        {
            AttackerID = attackerID;
            AttackerName = attackerName;
            TargetID = targetID;
            TargetName = targetName;
            Damage = damage;
            GotKilled = gotKilled;
        }
        public int AttackerID;
        public string AttackerName;
        public int TargetID;
        public string TargetName;
        public int Damage;
        public bool GotKilled;
    }
    public void NewCombatEvent(int attackerID, string attackerName, int targetID, string targetName, int damage, bool gotKilled)
    {
        _damageEvents.Add(new DamageEvent(attackerID, attackerName, targetID, targetName, damage, gotKilled));
    }

    #endregion


    #region >>> Game <<<

    public void OnGameEnd(GameState current, GameState old)
    {
        if(current == GameState.Win)
        {

        }
        if (current == GameState.Lose)
        {

        }
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //if (Analytics.enabled == false)
        //    return;

        GameManager.Instance.OnGameStateChanged += PlayerMovement;
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
}

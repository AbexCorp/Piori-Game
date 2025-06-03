using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private Player _player;
    public Player Player => _player;

    [SerializeField]
    private GridManager _gridManager;
    public GridManager GridManager => _gridManager;

    [SerializeField]
    private ProjectileManager _projectileManager;
    public ProjectileManager ProjectileManager => _projectileManager;

    [SerializeField]
    private EnemyManager _enemyManager;
    public EnemyManager EnemyManager => _enemyManager;

    [SerializeField]
    private ResourceManager _resourceManager;
    public ResourceManager ResourceManager => _resourceManager;

    [SerializeField]
    private InterfaceManager _interfaceManager;
    public InterfaceManager InterfaceManager => _interfaceManager;

    [SerializeField]
    private BuildingManager _buildingManager;
    public BuildingManager BuildingManager => _buildingManager;



    protected override void OnAwake()
    {
        if (_player == null)
            Debug.LogWarning("Player refference is not assigned");
        if (_gridManager == null)
            Debug.LogWarning("Grid Manager refference is not assigned");
        if(_projectileManager == null)
            Debug.LogWarning("Projectile Manager refference is not assigned");
        if(_enemyManager == null)
            Debug.LogWarning("Enemy Manager refference is not assigned");
        if(_resourceManager == null)
            Debug.LogWarning("Resource Manager refference is not assigned");
        if(_interfaceManager == null)
            Debug.LogWarning("Interface Manager refference is not assigned");
        if(_buildingManager == null)
            Debug.LogWarning("Building Manager refference is not assigned");
    }
    private void Start()
    {
        ChangeGameState(GameState.GamePreparePhase);
        ChangeGameState(GameState.BeforeFirstWave);
    }


    #region >>> Game State <<<

    public event Action<GameState> OnGameStateChanged;
    private GameState _currentGameState = GameState.None;
    public GameState CurrentGameState => _currentGameState;

    public void ChangeGameState(GameState newState)
    {
        _currentGameState = newState;
        switch (newState)
        {
            case GameState.None:
                break;
            case GameState.GamePreparePhase:
                break;

            case GameState.BeforeFirstWave:
                BuildingManager.AllowBuilding();
                StartCoroutine(WaveBreakTimer(FirstWaveDelay));
                break;

            case GameState.Wave:
                break;
            case GameState.WaveBreak:
                StartCoroutine(WaveBreakTimer(WaveBreakTime));
                break;

            case GameState.Win:
                break;
            case GameState.Lose:
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }

    #endregion


    #region >>> Level <<<

    [SerializeField]
    private Level _level;
    public Level Level => _level;

    [SerializeField]
    private float _firstWaveDelay = 15f;
    public float FirstWaveDelay => _firstWaveDelay;
    [SerializeField]
    private float _waveBreakTime = 10f;
    public float WaveBreakTime => _waveBreakTime;

    protected IEnumerator WaveBreakTimer(float time)
    {
        yield return new WaitForSeconds(time);
        ChangeGameState(GameState.Wave);
    }

    #endregion
}

public enum GameState
{
    None = 0,
    GamePreparePhase = 1,
    BeforeFirstWave = 2,
    Wave = 3,
    WaveBreak = 4,
    Win = 5,
    Lose = 6
}

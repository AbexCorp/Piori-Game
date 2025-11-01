using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [SerializeField]
    private AnalyticsManager _analyticsManager;
    public AnalyticsManager AnalyticsManager => _analyticsManager;



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

    /// <summary>
    /// New, Old
    /// </summary>
    public event Action<GameState, GameState> OnGameStateChanged;
    private GameState _currentGameState = GameState.None;
    public GameState CurrentGameState => _currentGameState;

    public void ChangeGameState(GameState newState)
    {
        GameState old = _currentGameState;
        _currentGameState = newState;
        switch (newState)
        {
            case GameState.None:
                break;
            case GameState.GamePreparePhase:
                break;

            case GameState.BeforeFirstWave:
                BuildingManager.AllowBuilding();
                StartCoroutine(BeforeFirstWaveTimer(FirstWaveDelay));
                StartCoroutine(GameTimer());
                break;

            case GameState.NewWave:
                break;
            case GameState.Wave:
                StartCoroutine(WaveTimer(WaveTime));
                break;
            case GameState.WaveBreak:
                StartCoroutine(WaveBreakTimer(WaveBreakTime));
                break;

            case GameState.Win:
                SceneManager.LoadScene("WinGame");
                break;
            case GameState.Lose:
                SceneManager.LoadScene("LoseGame");
                break;
        }

        OnGameStateChanged?.Invoke(newState, old);
    }

    #endregion


    #region >>> Game Timer <<<

    private int _gameTime = 0;
    public int GameTime => _gameTime;
    public event Action OnGameTimerAdvance;
    private IEnumerator GameTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            _gameTime += 1;
            OnGameTimerAdvance?.Invoke();
        }
    }

    #endregion


    #region >>> Level <<<

    [SerializeField]
    private Level _level;
    public Level Level => _level;


    [SerializeField]
    private int _firstWaveDelay = 15;
    public int FirstWaveDelay => _firstWaveDelay;

    [SerializeField]
    private int _waveTime = 10;
    public int WaveTime => _waveTime;

    [SerializeField]
    private int _waveBreakTime = 10;
    public int WaveBreakTime => _waveBreakTime;

    private bool GameIsOver => CurrentGameState == GameState.Win || CurrentGameState == GameState.Lose;

    private IEnumerator BeforeFirstWaveTimer(int time)
    {
        YieldInstruction yield = new WaitForSeconds(1);
        UpdateTimerColor(new Color(117/255f, 175/255f, 183/255f));

        for(int i = 0; i < time; i++)
        {
            if(GameIsOver)
                break;
            UpdateTimerValue(i, FirstWaveDelay);
            yield return yield;
        }
        if(!GameIsOver)
            ChangeGameState(GameState.NewWave);
    }
    private IEnumerator WaveTimer(int time)
    {
        YieldInstruction yield = new WaitForSeconds(1);
        UpdateTimerColor(new Color(231 / 255f, 69 / 255f, 69 / 255f));

        for (int i = 0; i < time; i++)
        {
            if(GameIsOver)
                break;
            UpdateTimerValue(i, WaveTime);
            yield return yield;
        }

        if (Level.Waves.Count <= EnemyManager.CurrentWave)
            InterfaceManager.StopGameTimer();
        if (!GameIsOver && Level.Waves.Count > EnemyManager.CurrentWave)
            ChangeGameState(GameState.WaveBreak);
    }
    private IEnumerator WaveBreakTimer(int time)
    {
        YieldInstruction yield = new WaitForSeconds(1);
        UpdateTimerColor(new Color(60/255f, 160/255f, 60/255f));

        for(int i = 0; i < time; i++)
        {
            if(GameIsOver)
                break;
            UpdateTimerValue(i, WaveBreakTime);
            yield return yield;
        }
        if(!GameIsOver)
            ChangeGameState(GameState.NewWave);
    }
    private void UpdateTimerColor(Color color)
    {
        InterfaceManager.ChangeGameTimerColor(color);
    }
    private void UpdateTimerValue(int time, int maxTime)
    {
        InterfaceManager.ChangeGameTimerValue($"{maxTime - time}", (maxTime - time)/(float)maxTime);
    }

    #endregion
}

public enum GameState
{
    None = 0,
    GamePreparePhase = 1,
    BeforeFirstWave = 2,
    NewWave = 3,
    Wave = 4,
    WaveBreak = 5,
    Win = 6,
    Lose = 7
}

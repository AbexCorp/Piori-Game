using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Services.Analytics;
using Event = Unity.Services.Analytics.Event;
using System.Text;
using System.Linq;

public class AnalyticsManager : MonoBehaviour
{
    private GameInfoEvent _gameInfoEvent;

    void Start()
    {
        GameManager.Instance.OnGameStateChanged += PlayerMovement;
        GameManager.Instance.OnGameStateChanged += OnWaveEnd;
        GameManager.Instance.OnGameStateChanged += OnGameEnd;

        _gameInfoEvent = new GameInfoEvent();
    }


    #region >>> Shortcuts <<<

    private GameState CurrentGameState => GameManager.Instance.CurrentGameState;
    private int CurrentWave => GameManager.Instance.EnemyManager.CurrentWave;
    private int GameTime => GameManager.Instance.GameTime;

    #endregion


    #region >>> Building <<<

    private List<GridTile> _builtTiles = new();

    public void OnBuildingBuilt(Building building, GridTile tile, int cost)
    {
        //Debug.Log($"Built {building.name}, {tile.GridPosition}, {cost}, {CurrentGameState}, {CurrentWave}, {GameTime}"); ////////////////////////
        if(_builtTiles.Contains(tile) == false)
        {
            _builtTiles.Add(tile);
        }
        CountBuilding(building);
        _buildingsBuilt++;
    }
    public void OnBuildingKilled(Building building, GridTile tile)
    {
        //Debug.Log($"Killed {building.name}, {tile.GridPosition}, {CurrentGameState}, {CurrentWave}, {GameTime}"); ////////////////////////////
        _buildingsLost++;
    }
    public void OnBuildingSold(Building building, GridTile tile, int cashback)
    {
        //Debug.Log($"Sold {building.name}, {tile.GridPosition}, {cashback}, {CurrentGameState}, {CurrentWave}, {GameTime}"); ////////////////////////////
        _buildingsSold++;
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
        _playerHPLost += damage;
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

        //Debug.Log($"Resources: {_resourcesGained}, {_resourcesUsed}, {CurrentWave + waveAdjustment} {current}"); //////////////////////////
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
            //Debug.Log($"Beat Wave {wave}"); ////////////////////////////
            _waveTimes[wave - 1] = GameTime;
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
    public void ClearCombatLog()
    {
        _damageEvents.Clear();
    }

    public int CheckDamageFor(string name)
    {
        var damageEvents = _damageEvents.Where(x => x.AttackerName == name).ToList();
        int damage = 0;
        foreach (var damageEvent in damageEvents)
            damage += damageEvent.Damage;

        return damage;
    }
    public int CheckKillsFor(string name)
    {
        var damageEvents = _damageEvents.Where(x => x.AttackerName == name && x.GotKilled == true).ToList();
        int kills = 0;
        foreach (var damageEvent in damageEvents)
            kills++;

        return kills;
    }

    #endregion


    #region >>> Game <<<

    private int[] _waveTimes = new int[] {-1, -1, -1, -1, -1};
    private bool _victory = false;

    private int _buildingsBuilt = 0;
    private int _buildingsLost = 0;
    private int _buildingsSold = 0;

    private int _towersBuilt = 0;
    private int _resourcesBuilt = 0;
    private int _wallsBuilt = 0;
    private int _cannonsBuilt = 0;
    private int _gunsBuilt = 0;
    private int _arrowsBuilt = 0;
    private int _lumberyardsBuilt = 0;

    private int _playerHPLost = 0;
    private float _playerMovement = 0; //this is broken
    private float _playerMovementTotal = 0;
    private int _playerDamage = 0;
    private int _playerKills = 0;

    int _cannonDamage = 0;
    int _gunDamage = 0;
    int _arrowDamage = 0;
    int _cannonKills = 0;
    int _gunKills = 0;
    int _arrowKills = 0;

    int _gruntDamage = 0;
    int _archerDamage = 0;
    int _toughDamage = 0;
    int _bossDamage = 0;
    int _gruntKills = 0;
    int _archerKills = 0;
    int _toughKills = 0;
    int _bossKills = 0;

    public void OnWaveEnd(GameState current, GameState old)
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

        WaveEndEvent waveEnd = new(CurrentWave + waveAdjustment);
        WaveEndCount();
        waveEnd.SetResources(_resourcesGained, _resourcesUsed);
        waveEnd.SetBuildings(_buildingsBuilt, _buildingsLost, _buildingsSold, _towersBuilt, _resourcesBuilt, _wallsBuilt, _cannonsBuilt, _gunsBuilt, _arrowsBuilt, _lumberyardsBuilt);
        waveEnd.SetPlayer(_playerHPLost, _playerMovement, _playerDamage, _playerKills);
        waveEnd.SetTowerCombat(_cannonDamage, _gunDamage, _arrowDamage, _cannonKills, _gunKills, _arrowKills);
        waveEnd.SetEnemyCombat(_gruntDamage, _archerDamage, _toughDamage, _bossDamage, _gruntKills, _archerKills, _toughKills, _bossKills);
        AnalyticsService.Instance.RecordEvent(waveEnd);
        WaveEndTally(current, old);
    }
    public void OnGameEnd(GameState current, GameState old)
    {
        if (current != GameState.Win && current != GameState.Lose)
            return;

        if(current == GameState.Win)
        {
            _victory = true;
            _gameInfoEvent.SetVictory(_victory);
        }
        if (current == GameState.Lose)
        {
            _victory = false;
            _gameInfoEvent.SetVictory(_victory, CurrentWave);
        }

        GridBuildingTally();
        _gameInfoEvent.SetWaveTime(_waveTimes[0], _waveTimes[1], _waveTimes[2], _waveTimes[3], _waveTimes[4]);
        AnalyticsService.Instance.RecordEvent(_gameInfoEvent);
    }
    private void GridBuildingTally()
    {
        StringBuilder sb = new StringBuilder();
        for(int i = 0; i < _builtTiles.Count; i++)
        {
            sb.Append($"{_builtTiles[i].X}-{_builtTiles[i].Y}");
            if(i < _builtTiles.Count - 1)
                sb.Append(',');
        }
        //Debug.Log(sb.ToString());///////////////////
        GridBuildingEvent g = new GridBuildingEvent(sb.ToString(), _victory);
        AnalyticsService.Instance.RecordEvent(g);
    }
    private void WaveEndCount()
    {
        CountPlayer();
        CountTowerCombat();
        CountEnemiesCombat();
    }
    private void WaveEndTally(GameState current, GameState old)
    {
        TallyResources(current, old);
        TallyBuildings();
        TallyPlayer();
        TallyTowerCombat();
        TallyEnemiesCombat();
        ClearCombatLog();
    }
    private void CountBuilding(Building building)
    {
        switch (building.UniqueName)
        {
            case "Wall":
                _wallsBuilt++;
                break;

            case "Cannon Tower":
                _towersBuilt++;
                _cannonsBuilt++;
                break;

            case "Gun Tower":
                _towersBuilt++;
                _gunsBuilt++;
                break;

            case "Arrow Tower":
                _towersBuilt++;
                _arrowsBuilt++;
                break;

            case "Lumberyard":
                _resourcesBuilt++;
                _lumberyardsBuilt++;
                break;
        }
    }
    private void TallyBuildings()
    {
        _buildingsBuilt = 0;
        _buildingsLost = 0;
        _buildingsSold = 0;
        _towersBuilt = 0;
        _resourcesBuilt = 0;
        _wallsBuilt = 0;
        _cannonsBuilt = 0;
        _gunsBuilt = 0;
        _arrowsBuilt = 0;
        _lumberyardsBuilt = 0;
    }
    private void CountPlayer()
    {
        _playerMovementTotal += _playerMovement;
        _playerMovement = _moveDistance - _playerMovementTotal;
        Debug.Log($"{_playerMovementTotal} {_playerMovement} {_moveDistance}");
        _playerDamage = CheckDamageFor("Player");
        _playerKills = CheckKillsFor("Player");
    }
    private void TallyPlayer()
    {
        _playerHPLost = 0;
        _playerDamage = 0;
        _playerKills = 0;
    }
    private void CountTowerCombat()
    {
        _cannonDamage = CheckDamageFor("Cannon Tower");
        _gunDamage = CheckDamageFor("Gun Tower");
        _arrowDamage = CheckDamageFor("Arrow Tower");

        _cannonKills = CheckKillsFor("Cannon Tower");
        _gunKills = CheckKillsFor("Gun Tower");
        _arrowKills = CheckKillsFor("Arrow Tower");
    }
    private void TallyTowerCombat()
    {
        _cannonDamage = 0;
        _gunDamage = 0;
        _arrowDamage = 0;

        _cannonKills = 0;
        _gunKills = 0;
        _arrowKills = 0;
    }
    private void CountEnemiesCombat()
    {
        _gruntDamage = CheckDamageFor("Grunt");
        _archerDamage = CheckDamageFor("Archer");
        _toughDamage = CheckDamageFor("Tough");
        _bossDamage = CheckDamageFor("Boss");

        _gruntKills = CheckKillsFor("Grunt");
        _archerKills = CheckKillsFor("Archer");
        _toughKills = CheckKillsFor("Tough");
        _bossKills = CheckKillsFor("Boss");
    }
    private void TallyEnemiesCombat()
    {
        _gruntDamage = 0;
        _archerDamage = 0;
        _toughDamage = 0;
        _bossDamage = 0;

        _gruntKills = 0;
        _archerKills = 0;
        _toughKills = 0;
        _bossKills = 0;
    }

    #endregion
}

internal class GameInfoEvent : Event
{
    public GameInfoEvent() : base("GameInfoEvent") { }

    public void SetVictory(bool won, int waveLost = -1)
    {
        SetParameter("Victory", won);
        SetParameter("WaveLost", waveLost);
    }

    public void SetWaveTime(int one, int two, int three, int four, int five)
    {
        SetParameter("Wave1Time", one);
        SetParameter("Wave2Time", two);
        SetParameter("Wave3Time", three);
        SetParameter("Wave4Time", four);
        SetParameter("Wave5Time", five);
    }

    //Victory - bool
    //WaveLost - int

    //Wave1Time - int
    //Wave2Time - int
    //Wave3Time - int
    //Wave4Time - int
    //Wave5Time - int
}

internal class WaveEndEvent : Event
{
    public WaveEndEvent(int wave) : base("WaveEndEvent")
    {
        SetParameter("Wave", wave);
    }

    public void SetResources(int gained, int lost)
    {
        SetParameter("ResourcesGained", gained);
        SetParameter("ResourcesSpent", lost);
    }
    public void SetBuildings(int BuildingsBuilt, int BuildingsLost, int BuildingsSold, int TowersBuilt, int ResourcesBuilt, int WallsBuilt, int CannonsBuilt,
        int GunsBuilt, int ArrowsBuilt, int LumberyardsBuilt)
    {
        SetParameter("BuildingsBuilt", BuildingsBuilt);
        SetParameter("BuildingsLost", BuildingsLost);
        SetParameter("BuildingsSold", BuildingsSold);

        SetParameter("TowersBuilt", TowersBuilt);
        SetParameter("ResourcesBuilt", ResourcesBuilt);
        SetParameter("WallsBuilt", WallsBuilt);
        SetParameter("CannonsBuilt", CannonsBuilt);
        SetParameter("GunsBuilt", GunsBuilt);
        SetParameter("ArrowsBuilt", ArrowsBuilt);
        SetParameter("LumberyardsBuilt", LumberyardsBuilt);
    }
    public void SetPlayer(int PlayerHPLost, float PlayerMovement, int PlayerDamage, int PlayerKills)
    {
        SetParameter("PlayerHPLost", PlayerHPLost);
        SetParameter("PlayerMovement", PlayerMovement);
        SetParameter("PlayerDamage", PlayerDamage);
        SetParameter("PlayerKills", PlayerKills);
    }
    public void SetTowerCombat(int CannonDamage, int GunDamage, int ArrowDamage, int CannonKills, int GunKills, int ArrowKills)
    {
        SetParameter("CannonDamage", CannonDamage);
        SetParameter("GunDamage", GunDamage);
        SetParameter("ArrowDamage", ArrowDamage);

        SetParameter("CannonKills", CannonKills);
        SetParameter("GunKills", GunKills);
        SetParameter("ArrowKills", ArrowKills);
    }
    public void SetEnemyCombat(int GruntDamage, int ArcherDamage, int ToughDamage, int BossDamage, int GruntKills, int ArcherKills, int ToughKills, int BossKills)
    {
        SetParameter("GruntDamage", GruntDamage);
        SetParameter("ArcherDamage", ArcherDamage);
        SetParameter("ToughDamage", ToughDamage);
        SetParameter("BossDamage", BossDamage);

        SetParameter("GruntKills", GruntKills);
        SetParameter("ArcherKills", ArcherKills);
        SetParameter("ToughKills", ToughKills);
        SetParameter("BossKills", BossKills);
    }

    //Wave - int

    //ResourcesGained - int
    //ResourcesSpent - int

    //BuildingsBuilt - int
    //BuildingsLost - int
    //BuildingsSold - int

    //TowersBuilt - int
    //ResourcesBuilt - int
    //WallsBuilt - int
    //CannonsBuilt - int
    //GunsBuilt - int
    //ArrowsBuilt - int
    //LumberyardsBuilt - int

    //PlayerHPLost - int
    //PlayerMovement - float
    //PlayerDamage - int
    //PlayerKills - int

    //CannonDamage - int
    //GunDamage - int
    //ArrowDamage - int

    //CannonKills - int
    //GunKills - int
    //ArrowKills - int

    //GruntDamage - int
    //ArcherDamage - int
    //ToughDamage - int
    //BossDamage - int

    //GruntKills - int
    //ArcherKills - int
    //ToughKills - int
    //BossKills - int
}

internal class GridBuildingEvent : Event
{
    public GridBuildingEvent(string tiles, bool victory) : base("GridBuildingEvent")
    {
        SetParameter("TileList", tiles);
        SetParameter("Victory", victory);
    }

    //Victory - bool
    //TileList - string
}
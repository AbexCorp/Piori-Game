using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class InterfaceManager : MonoBehaviour
{
    private void Start()
    {
        GenerateBuildingButtons();
    }
    private void Update()
    {
        DebugUI();
        MoveTooltip();
    }


    #region >>> Building <<<

    [Header("Building")]
    [SerializeField]
    private GameObject _buildingButtonsFrame;
    [SerializeField]
    private BuildingButton _buildingButtonPrefab;
    [SerializeField]
    private CustomButton _sellButton;
    [SerializeField]
    private CustomButton _cancelBuildingButton;

    private Dictionary<int, BuildingButton> _buildingButtons = new();

    private void GenerateBuildingButtons()
    {
        float position = 0;
        float buttonWidth = _buildingButtonPrefab.RectTransform.rect.width;
        float margin = 20;

        //Regular build buttons
        TowerProfile[] towers = Resources.LoadAll<TowerProfile>("Buildings/Towers");
        for(int i = 0; i < towers.Length; i++)
        {
            BuildingButton b = Instantiate(_buildingButtonPrefab, _buildingButtonsFrame.transform);
            b.SetProfile(towers[i]);
            b.RectTransform.anchoredPosition = new Vector2(position, 0);
            position += (buttonWidth + margin);
            if(_buildingButtons.Count < 9)
                _buildingButtons.Add(_buildingButtons.Count + 1, b);
        }

        ResourceBuildingProfile[] resourceBuildings = Resources.LoadAll<ResourceBuildingProfile>("Buildings/ResourceBuildings");
        for(int i = 0; i < resourceBuildings.Length; i++)
        {
            BuildingButton b = Instantiate(_buildingButtonPrefab, _buildingButtonsFrame.transform);
            b.SetProfile(resourceBuildings[i]);
            b.RectTransform.anchoredPosition = new Vector2(position, 0);
            position += (buttonWidth + margin);
            if (_buildingButtons.Count < 9)
                _buildingButtons.Add(_buildingButtons.Count + 1, b);
        }

        //Sell button
        position += margin;
        _sellButton.RectTransform.anchoredPosition = new Vector2 (position, 0);
        position += _sellButton.RectTransform.rect.width + margin;

        //Cancel button
        _cancelBuildingButton.RectTransform.anchoredPosition = new Vector2 (position, 0);
        position += _cancelBuildingButton.RectTransform.rect.width + margin;
        BuildingCancelButtonSetActive(false);
    }

    public void OnSellButton()
    {
        GameManager.Instance.BuildingManager.Sell();
    }
    public void OnCancelButton()
    {
        GameManager.Instance.BuildingManager.StopBuilding();
        GameManager.Instance.BuildingManager.StopSelling();
    }
    public void BuildingCancelButtonSetActive(bool value)
    {
        _cancelBuildingButton.gameObject.SetActive(value);
    }

    public void QuickBuild(int value)
    {
        if (value < 0 || value > _buildingButtons.Count)
            return;

        if(value != 0)
        {
            if (_buildingButtons.ContainsKey(value))
            {
                _buildingButtons[value].Activate();
                return;
            }
            return;
        }

        if(GameManager.Instance.BuildingManager.IsBuilding || GameManager.Instance.BuildingManager.IsSelling)
        {
            _cancelBuildingButton.Activate();
            return;
        }
        else if(GameManager.Instance.BuildingManager.IsBuilding == false && GameManager.Instance.BuildingManager.IsSelling == false)
        {
            _sellButton.Activate();
            return;
        }
    }

    #endregion


    #region >>> GameTimer <<<

    [Header("Timer")]
    [SerializeField]
    private UnityEngine.UI.Image _gameTimer;
    [SerializeField]
    private TMP_Text _gameTimerText;

    public void ChangeGameTimerColor(Color color)
    {
        _gameTimer.color = color;
    }
    public void ChangeGameTimerValue(string text, float fill)
    {
        _gameTimerText.text = text;
        _gameTimer.fillAmount = Mathf.Clamp(fill, 0, 1);
    }

    #endregion


    #region >>> Text Box <<<

    [Header("Text Box")]
    [SerializeField]
    private UnityEngine.UI.Image _textBox;
    [SerializeField]
    private TMP_Text _textBoxText;
    [SerializeField]
    [Range(0.1f, 6f)]
    private float _textShowingAmount = 2.5f;
    private List<string> _list = new List<string>();
    private bool _showingText = false;

    public void DisplayTextMessage(string text)
    {
        _list.Add(text);
        if(_showingText == false)
            StartCoroutine(TextBoxDisplay());
    }
    private IEnumerator TextBoxDisplay()
    {
        YieldInstruction yield = new WaitForSeconds(_textShowingAmount);
        if(_list.Count == 0)
            yield return null;

        _showingText = true;
        _textBox.gameObject.SetActive(true);
        while (_list.Count > 0)
        {
            _textBoxText.text = _list.FirstOrDefault();
            yield return yield;
            _list.RemoveAt(0);
        }
        _textBox.gameObject.SetActive(false);
        _showingText = false;
    }

    #endregion


    #region >>> Tooltip <<<

    [Header("Floating Tooltip")]
    [SerializeField]
    private RectTransform _floatingTooltip;
    [SerializeField]
    private TMP_Text _floatingTooltipText;

    private bool _floatingTooltipIsEnabled = false;
    public bool FloatingTooltipIsEnabled => _floatingTooltipIsEnabled;
    private int distance = 10;

    public void TooltipSetActive(bool value)
    {
        _floatingTooltip.gameObject.SetActive(value);
        _floatingTooltipIsEnabled = value;
    }
    public void ChangeTooltipText(string text)
    {
        _floatingTooltipText.text = text;
    }
    private void MoveTooltip()
    {
        if (_floatingTooltipIsEnabled == false)
            return;

        float xOffset = 0;
        float yOffset = 0;
        if(GameManager.Instance.Player.MousePosition.x + _floatingTooltip.sizeDelta.x + distance > Screen.width)
            xOffset -= (distance + distance + _floatingTooltip.sizeDelta.x);
        if (GameManager.Instance.Player.MousePosition.y - _floatingTooltip.sizeDelta.y - distance < 0)
            yOffset += (distance + distance + _floatingTooltip.sizeDelta.y);

        _floatingTooltip.anchoredPosition = new Vector2(GameManager.Instance.Player.MousePosition.x + distance + xOffset, GameManager.Instance.Player.MousePosition.y - _floatingTooltip.sizeDelta.y - distance + yOffset);
    }

    #endregion


    #region >>> PauseMenu <<<

    [Header("Pause Menu")]
    [SerializeField]
    private GameObject _pauseMenu;

    private bool _pauseMenuEnabled = false;
    public bool PauseMenuEnabled => _pauseMenuEnabled;

    public void PauseMenu()
    {
        if (_pauseMenuEnabled == false)
        {
            _pauseMenuEnabled = true;
            Time.timeScale = 0f;
            _pauseMenu.SetActive(true);
        }
        else
        {
            _pauseMenuEnabled = false;
            Time.timeScale = 1f;
            _pauseMenu.SetActive(false);
        }
    }

    #endregion


    #region Debug

    [Header("Debug")]
    [SerializeField]
    private TMP_Text _debugUI;


    private StringBuilder sb = new();
    private int _health = -1;
    private int _healthMax = -1;
    private int _resource = -1;

    private void DebugUI()
    {
        sb.Clear();
        sb.AppendLine($"Health: {_health}/{_healthMax}");
        sb.AppendLine($"Resource: {_resource}");
        sb.AppendLine($"Game State: {GameManager.Instance.CurrentGameState}");
        sb.AppendLine($"Wave: {GameManager.Instance.EnemyManager.CurrentWave}");
        if (GameManager.Instance.BuildingManager.IsSelling)
            sb.AppendLine($"Building: Selling");
        else
            sb.AppendLine($"Building: {(GameManager.Instance.BuildingManager.SelectedProfile == null ? "Nothing" : GameManager.Instance.BuildingManager.SelectedProfile.UniqueID)}");

        _debugUI.text = sb.ToString();
    }
    public void UpdateHealth(int amount, int max)
    {
        _health = amount;
        _healthMax = max;
    }
    public void UpdateResource(int amount)
    {
        _resource = amount;
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class InterfaceManager : Menu
{
    protected override void Start()
    {
        base.Start();
        GenerateBuildingButtons();
    }
    private void Update()
    {
        //DebugUI();
        MoveTooltip();
        MoveBuildStatusTooltip();
    }


    #region >>> Building <<<

    [Header("Building")]
    [SerializeField]
    private GameObject _buildingButtonsFrame;
    [SerializeField]
    private Animator _buildingButtonsFrameAnimator;
    [SerializeField]
    private GameObject _buildingIconsFrame;
    [SerializeField]
    private BuildingButton _buildingButtonPrefab;
    [SerializeField]
    private BuildingIcon _buildingIconPrefab;
    [SerializeField]
    private CustomButton _sellButton;
    [SerializeField]
    private Sprite _sellButtonIcon;
    [SerializeField]
    private CustomButton _cancelBuildingButton;
    [SerializeField]
    private Sprite _cancelBuildingButtonIcon;

    private Dictionary<int, BuildingButton> _buildingButtons = new();
    private bool _buildButtonsVisible = false;

    private void GenerateBuildingButtons()
    {
        float iconPosition = -10;
        float buttonPosition = 0;
        float buttonHeight = _buildingButtonPrefab.RectTransform.rect.height;
        float iconHeight = _buildingIconPrefab.RectTransform.rect.height;
        float margin = 20;
        float buttonFrameHeightAdjustment = 10; //Should reat button fram top margin

        //Regular build buttons
        TowerProfile[] towers = Resources.LoadAll<TowerProfile>("Buildings/Towers");
        towers = towers.OrderBy(x => x.BaseCost).ToArray();
        for(int i = 0; i < towers.Length; i++)
        {
            towers[i] = Instantiate(towers[i]);
        }

        for(int i = 0; i < towers.Length; i++)
        {
            BuildingButton b = Instantiate(_buildingButtonPrefab, _buildingButtonsFrame.transform);
            BuildingIcon ic = Instantiate(_buildingIconPrefab, _buildingIconsFrame.transform);
            b.SetProfile(towers[i]);

            ic.RectTransform.anchoredPosition = new Vector2(0, iconPosition);
            buttonPosition = iconPosition - ((iconHeight - buttonHeight) / 2) + buttonFrameHeightAdjustment;
            iconPosition -= iconHeight;
            b.RectTransform.anchoredPosition = new Vector2(0, buttonPosition);

            if(_buildingButtons.Count < 9)
            {
                _buildingButtons.Add(_buildingButtons.Count + 1, b);
                ic.Set(_buildingButtons.Count.ToString(), b.Icon);
            }
            else
                ic.Set("", b.Icon);
        }

        iconPosition -= margin;
        ResourceBuildingProfile[] resourceBuildings = Resources.LoadAll<ResourceBuildingProfile>("Buildings/ResourceBuildings");
        for (int i = 0; i < resourceBuildings.Length; i++)
        {
            resourceBuildings[i] = Instantiate(resourceBuildings[i]);
        }

        for (int i = 0; i < resourceBuildings.Length; i++)
        {
            BuildingButton b = Instantiate(_buildingButtonPrefab, _buildingButtonsFrame.transform);
            BuildingIcon ic = Instantiate(_buildingIconPrefab, _buildingIconsFrame.transform);
            b.SetProfile(resourceBuildings[i]);

            ic.RectTransform.anchoredPosition = new Vector2(0, iconPosition);
            buttonPosition = iconPosition - ((iconHeight - buttonHeight) / 2) + buttonFrameHeightAdjustment;
            iconPosition -= iconHeight;
            b.RectTransform.anchoredPosition = new Vector2(0, buttonPosition);

            if (_buildingButtons.Count < 9)
            {
                _buildingButtons.Add(_buildingButtons.Count + 1, b);
                ic.Set(_buildingButtons.Count.ToString(), b.Icon);
            }
            else
                ic.Set("", b.Icon);
        }


        //Sell button
        iconPosition -= margin;
        BuildingIcon ico = Instantiate(_buildingIconPrefab, _buildingIconsFrame.transform);
        ico.RectTransform.anchoredPosition = new Vector2(0, iconPosition);
        ico.Set("`", _sellButtonIcon);
        buttonPosition = iconPosition - ((iconHeight - buttonHeight) / 2) + buttonFrameHeightAdjustment;

        _sellButton.RectTransform.anchoredPosition = new Vector2(0, buttonPosition);
        iconPosition -= iconHeight;


        //Cancel button
        iconPosition -= margin;
        ico = Instantiate(_buildingIconPrefab, _buildingIconsFrame.transform);
        ico.RectTransform.anchoredPosition = new Vector2(0, iconPosition);
        ico.Set("`", _cancelBuildingButtonIcon);
        buttonPosition = iconPosition - ((iconHeight - buttonHeight) / 2) + buttonFrameHeightAdjustment;

        _cancelBuildingButton.RectTransform.anchoredPosition = new Vector2(0, buttonPosition);
        BuildingCancelButtonSetActive(false);

        SetBuildButtonsTo(false);
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

    public void SetBuildButtons()
    {
        _buildButtonsVisible = !_buildButtonsVisible;
        SetBuildButtonsTo(_buildButtonsVisible);
    }
    public void SetBuildButtonsTo(bool value)
    {
        switch (value)
        {
            case true:
                _buildingButtonsFrameAnimator.Play("BuildMenuShow");
                return;

            case false:
                _buildingButtonsFrameAnimator.Play("BuildMenuHide");
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
    public void StopGameTimer()
    {
        ChangeGameTimerColor(Color.gray);
        ChangeGameTimerValue("", 1);
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

    #region Building Status Tooltip

    [Space]
    [SerializeField]
    private RectTransform _buildingStatusTooltip;
    [SerializeField]
    private TMP_Text _buildingStatusText;

    private bool _buildtStatusTooltipIsActive = false;

    public void BuildStatusTooltipSetActive(bool value)
    {
        _buildtStatusTooltipIsActive = value;
        _buildingStatusTooltip.gameObject.SetActive(value);
    }
    public void MoveBuildStatusTooltip()
    {
        if(_buildtStatusTooltipIsActive)
            _buildingStatusTooltip.anchoredPosition = new Vector2(GameManager.Instance.Player.MousePosition.x + 10, GameManager.Instance.Player.MousePosition.y);
    }
    public void BuildStatusTooltipUpdateText()
    {
        if (GameManager.Instance.BuildingManager.IsSelling)
            _buildingStatusText.text = $"Building: Selling";
        else
            _buildingStatusText.text = ($"Building: {(GameManager.Instance.BuildingManager.SelectedProfile == null ? "Nothing" : GameManager.Instance.BuildingManager.SelectedProfile.UniqueName)}");
    }

    #endregion

    #endregion


    #region >>> Counters <<<

    [Header("Counters")]

    #region Player Health

    [Space]
    [SerializeField]
    private UnityEngine.UI.Image _playerHealthFill;
    [SerializeField]
    private TMP_Text _playerHealthText;

    public void UpdatePlayerHealth()
    {
        _playerHealthText.text = $"{GameManager.Instance.Player.HealthCurrent} / {GameManager.Instance.Player.HealthMax}";
        _playerHealthFill.fillAmount = GameManager.Instance.Player.HealthCurrent / (float)GameManager.Instance.Player.HealthMax;
    }

    #endregion

    #region Resource Counter

    [Space]
    [SerializeField]
    private TMP_Text _resourceCounter;

    public void UpdateResourceCounter(int amount)
    {
        _resourceCounter.text = $"{amount}";
    }

    #endregion

    #region Wave Counter

    [Space]
    [SerializeField]
    private TMP_Text _waveCounter;

    public void WaveCounterUpdate()
    {
        _waveCounter.text = $"Wave: {GameManager.Instance.EnemyManager.CurrentWave}";
    }

    #endregion

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
    public void BackToMenu()
    {
        Time.timeScale = 1f;
        LoadScene("MainMenu");
    }

    #endregion


    #region Debug

    [Header("Debug")]
    [SerializeField]
    private TMP_Text _debugUI;


    private StringBuilder sb = new();

    private void DebugUI()
    {
        sb.Clear();

        //WriteDebugUiHere

        _debugUI.text = sb.ToString();
    }

    #endregion
}

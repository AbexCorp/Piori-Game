using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    private void Start()
    {
        GenerateBuildingButtons();
    }



    [Header("Building")]
    [SerializeField]
    private GameObject _buildingButtonsFrame;
    [SerializeField]
    private BuildingButton _buildingButtonPrefab;
    private void GenerateBuildingButtons()
    {
        float position = 0;
        float buttonWidth = _buildingButtonPrefab.RectTransform.rect.width;
        float margin = 20;


        TowerProfile[] towers = Resources.LoadAll<TowerProfile>("Buildings/Towers");
        for(int i = 0; i < towers.Length; i++)
        {
            BuildingButton b = Instantiate(_buildingButtonPrefab, _buildingButtonsFrame.transform);
            b.SetProfile(towers[i]);
            b.RectTransform.anchoredPosition = new Vector2(position, 0);
            position += (buttonWidth + margin);
        }

        ResourceBuildingProfile[] resourceBuildings = Resources.LoadAll<ResourceBuildingProfile>("Buildings/ResourceBuildings");
        for(int i = 0; i < resourceBuildings.Length; i++)
        {
            BuildingButton b = Instantiate(_buildingButtonPrefab, _buildingButtonsFrame.transform);
            b.SetProfile(resourceBuildings[i]);
            b.RectTransform.anchoredPosition = new Vector2(position, 0);
            position += (buttonWidth + margin);
        }
    }

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



    #region Debug

    [Header("Debug")]
    [SerializeField]
    private TMP_Text _debugUI;


    private StringBuilder sb = new();
    private int _health = -1;
    private int _healthMax = -1;
    private int _resource = -1;

    private void Update()
    {
        sb.Clear();
        sb.AppendLine($"Health: {_health}/{_healthMax}");
        sb.AppendLine($"Resource: {_resource}");
        sb.AppendLine($"Game State: {GameManager.Instance.CurrentGameState}");
        sb.AppendLine($"Wave: {GameManager.Instance.EnemyManager.CurrentWave}");
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

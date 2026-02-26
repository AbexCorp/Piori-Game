using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : Menu
{
    public InputActionAsset inputactions;

    new private void Start()
    {
        base.Start();
        CheckConsent();
    }
    public void OnPlay()
    {
        LoadScene("WorldMap");
    }
    
    public void OnQuit()
    {
        #if UNITY_EDITOR
        Debug.Break();
        #endif
        Application.Quit();
    }
    [SerializeField]
    private GameObject _howToPlay;
    private bool _isShowingInstructions = false;
    public void OnHowToPLay()
    {
        _isShowingInstructions = !_isShowingInstructions;
        _howToPlay.SetActive(_isShowingInstructions);
    }
    [SerializeField]
    private GameObject _analyticsMenu;
    [SerializeField]
    private TMP_Text _consentNow;
    public void SetConsent(bool consent)
    {
        _analyticsMenu.SetActive(false);
        Services.Instance.SetConsent(consent);
    }
    private void SetConsentUI(string value)
    {
        _consentNow.text = $"Now: {value}";
    }
    private void CheckConsent()
    {
        if (File.Exists("consent.txt"))
        {
            using (StreamReader sr = new StreamReader("consent.txt"))
            {
                switch ((char)sr.Read())
                {
                    case '1':
                        SetConsentUI("agreeing");
                        return;

                    case '0':
                        SetConsentUI("disagreeing");
                        return;

                    default:
                        SetConsentUI("unspecified");
                        return;
                }
            }
        }
        else
        {
            SetConsentUI("unspecified");
            return;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : Menu
{
    public InputActionAsset inputactions;

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
}

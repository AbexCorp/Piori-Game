using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class WorldMap : Menu
{
    public void OnReturn()
    {
        LoadScene("MainMenu");
    }
    public void OnMission()
    {
        LoadScene("GameScene");
    }
}

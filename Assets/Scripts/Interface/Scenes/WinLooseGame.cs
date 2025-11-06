using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class WinLooseGame : Menu
{
    public void PlayAgain()
    {
        LoadScene("GameScene");
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void MainMenu()
    {
        LoadScene("MainMenu");
    }
}

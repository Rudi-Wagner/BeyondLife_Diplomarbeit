using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void MoveToLevels()
    {
        SceneManager.LoadScene("Levels");
    }

    public void MoveToSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void MoveToExit()
    {
        Application.Quit();
    }
}

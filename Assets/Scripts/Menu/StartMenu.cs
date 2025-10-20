using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Load the main game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MineSite");
    }

    public void QuitGame()
    {
        // Quit the application
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PauseMenu : NetworkBehaviour
{
    public void ResumeGame()
    {
        gameObject.SetActive(false);
        PlayerNetwork.isPaused = false;
    }

    public void QuitToMainMenu()
    {
        // If running as host or client, shut down networking
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
        }

        // Load the main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        PlayerNetwork.isPaused = false;
        Time.timeScale = 1f; // Ensure time is unpaused when returning to main menu
    }
}

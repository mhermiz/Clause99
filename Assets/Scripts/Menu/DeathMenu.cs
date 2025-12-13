using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class DeathMenu : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string gameSceneName = "MineSite";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    // Called by Play Again button
    public void PlayAgain()
    {
        // Shut down networking cleanly
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

        SceneManager.LoadScene(gameSceneName);
    }

    // Called by Exit button
    public void ExitToMainMenu()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }
}
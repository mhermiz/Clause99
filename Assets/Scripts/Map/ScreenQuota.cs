using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ScreenQuota : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    private PlayerStats localPlayer;

    private void Start()
    {
        StartCoroutine(FindlocalPlayerWhenReady());
        if (localPlayer != null)
        {
            
        }
    }

    private IEnumerator FindlocalPlayerWhenReady()
    {
        while (localPlayer == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null)
            {
                localPlayer = p.GetComponent<PlayerStats>();
                Debug.Log("Stats found localPlayer!");
                // Subscribe to changes in the NetworkVariable
                localPlayer.playerScore.OnValueChanged += OnScoreChanged;
                UpdateUI(localPlayer.playerScore.Value);
                yield break;
            }

            yield return new WaitForSeconds(0.2f); // try again
        }
    }

    private void OnScoreChanged(int oldValue, int newValue)
    {
        UpdateUI(newValue);
    }

    private void UpdateUI(int score)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    private GameObject NetworkUI;

    private void Awake()
    {
        NetworkUI = GameObject.Find("NetworkUI");

        serverBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
            NetworkUI.SetActive(false);
        });

        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            NetworkUI.SetActive(false);
        });

        clientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            NetworkUI.SetActive(false);
        });
    }
}

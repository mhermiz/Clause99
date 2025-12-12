using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public enum MineralInteractionType
{
    Collect,   // pick up minerals
    Deposit    // deposit minerals
}

public class MineralInteraction : NetworkBehaviour, IInteractable
{
    private TMP_Text mineralWeigh;

    [Header("Set whether this object is a collection node or deposit station")]
    public MineralInteractionType interactionType = MineralInteractionType.Collect;

    public bool ShouldDespawnAfterInteract => interactionType == MineralInteractionType.Collect;
    public bool RequiresHold => interactionType == MineralInteractionType.Deposit;

    private void Start()
    {
        GameObject textObject = GameObject.Find("MineralWeigh");
        mineralWeigh = textObject.GetComponent<TMP_Text>();
    }

    public void Interact(GameObject player)
    {
        var playerStats = player.GetComponent<PlayerStats>();
        if (playerStats == null) return;

        // Client sends request to the server
        if (!IsServer)
        {
            InteractServerRpc(playerStats.GetComponent<NetworkObject>().OwnerClientId);
        }
        else
        {
            // Host can handle directly
            HandleInteraction(playerStats);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractServerRpc(ulong clientId)
    {
        var player = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId).gameObject;
        var stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            HandleInteraction(stats);
        }
    }

    private void HandleInteraction(PlayerStats playerStats)
    {
        switch (interactionType)
        {
            case MineralInteractionType.Collect:
                if (playerStats.CanCarryMore())
                {
                    playerStats.AddMineral();
                    Debug.Log($"Collected! Player now has {playerStats.mineralsCarried.Value}");
                }
                else
                {
                    Debug.Log("Mineral capacity full!");
                }
                break;

            case MineralInteractionType.Deposit:
                int deposited = playerStats.DepositAllMinerals();
                Debug.Log($"Deposited {deposited} minerals!");
                break;
        }

        // Update the UI for the local player
        if (NetworkManager.Singleton.LocalClientId == playerStats.OwnerClientId && mineralWeigh != null)
        {
            mineralWeigh.text = $"{playerStats.mineralsCarried.Value}/{playerStats.mineralCapacity}";
        }
    }
}

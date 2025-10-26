using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MineralInteraction : NetworkBehaviour, IInteractable
{
    public void Interact(GameObject player)
    {
        var playerNetObj = player.GetComponent<NetworkObject>();
        if (playerNetObj == null) return;

        // Client sends request to the server
        if (!IsServer)
        {
            InteractServerRpc(playerNetObj.OwnerClientId);
        }
        else
        {
            // Host can handle directly
            MineralPickupClientRpc(playerNetObj.OwnerClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractServerRpc(ulong clientId)
    {
        Debug.Log($"[ServerRpc] Received mineral interaction from client {clientId}");
        MineralPickupClientRpc(clientId);
    }

    [ClientRpc]
    private void MineralPickupClientRpc(ulong clientId)
    {
        // Only the targeted client should act
        if (NetworkManager.Singleton.LocalClientId != clientId)
            return;

        Debug.Log($"[ClientRpc] Mineral picked up by client {clientId}");

        var player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        if (player == null)
        {
            Debug.LogWarning("Local player object not found for mineral pickup.");
            return;
        }

        // Here you can add logic to update player's inventory or stats
        Debug.Log("Mineral added to player's inventory.");
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DoorInteraction : NetworkBehaviour, IInteractable
{
    [SerializeField] private GameObject teleportPoint;

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
            TeleportPlayerClientRpc(playerNetObj.OwnerClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractServerRpc(ulong clientId)
    {
        Debug.Log($"[ServerRpc] Received door interaction from client {clientId}");
        TeleportPlayerClientRpc(clientId);
    }

    [ClientRpc]
    private void TeleportPlayerClientRpc(ulong clientId)
    {
        // Only the targeted client should act
        if (NetworkManager.Singleton.LocalClientId != clientId)
            return;

        Debug.Log($"[ClientRpc] Teleporting client {clientId}");

        var player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        if (player == null)
        {
            Debug.LogWarning("Local player object not found for teleport.");
            return;
        }

        var teleportPos = teleportPoint.transform;
        player.transform.SetPositionAndRotation(teleportPos.position, teleportPos.rotation);
    }
}

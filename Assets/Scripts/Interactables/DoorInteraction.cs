using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DoorInteraction : NetworkBehaviour, IInteractable
{
    public GameObject teleportPoint;
    private Transform teleportPos;

    public void Interact(GameObject player)
    {
        if (IsServer)
        {
            teleportPos = teleportPoint.transform;
            player.transform.position = teleportPos.position;
            player.transform.rotation = teleportPos.rotation;
        } else
        {
            InteractServerRpc(player.GetComponent<NetworkObject>().OwnerClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractServerRpc(ulong ownerClientId)
    {
        Debug.Log($"[ServerRpc] Client {ownerClientId} requested door interaction");
        
        var playerObject = NetworkManager.Singleton.ConnectedClients[ownerClientId].PlayerObject.gameObject;
        if (playerObject != null)
        {
            teleportPos = teleportPoint.transform;
            playerObject.transform.position = teleportPos.position;
            playerObject.transform.rotation = teleportPos.rotation;
        }
    }
}

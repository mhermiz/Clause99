using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ToolInteraction : NetworkBehaviour, IInteractable
{
    [SerializeField] private ItemObjects toolItem;
    public bool RequiresHold => false;

    public void Interact(GameObject player)
    {
        var playerObj = player.GetComponent<NetworkObject>();
        if (playerObj == null)
        {
            ;
            return;
        }

        // Client sends request to the server
        if (!IsServer)
        {
            PickupToolServerRpc(playerObj.OwnerClientId);
        }
        else
        {
            // Host can handle directly
            ToolPickupClientRpc(playerObj.OwnerClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PickupToolServerRpc(ulong clientId)
    {
        Debug.Log($"[ServerRpc] Received tool interaction from client {clientId}");
        ToolPickupClientRpc(clientId);
    }

    [ClientRpc]
    private void ToolPickupClientRpc(ulong clientId)
    {
        // Only the targeted client should act
        if (NetworkManager.Singleton.LocalClientId != clientId)
            return;

        Debug.Log($"[ClientRpc] Tool picked up by client {clientId}");

        var player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        if (player == null)
        {
            Debug.LogWarning("Local player object not found for tool pickup.");
            return;
        }

        var inventory = player.GetComponent<PlayerInventory>();
        if (inventory == null)
        {
            Debug.LogWarning("PlayerInventory component not found on local player.");
            return;
        }

        // Add the tool to the player's inventory
        inventory.AddItem(toolItem);
        Debug.Log($"Tool {toolItem} added to inventory of client {clientId}");
    }

    public void AssignItem(ItemObjects item)
    {
        toolItem = item;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    private NetworkVariable<float> health = new NetworkVariable<float>(100f);
    public static NetworkVariable<float> stamina = new NetworkVariable<float>(100f);

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            health.Value = 100f;
            stamina.Value = 100f;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage)
    {
        if (!IsServer) return;
        health.Value -= damage;
        Debug.Log($"{OwnerClientId} took {damage} damage. Health now: {health.Value}");
    }
}

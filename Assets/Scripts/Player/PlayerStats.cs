using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    private NetworkVariable<float> health = new NetworkVariable<float>(100f);
    private NetworkVariable<float> stamina = new NetworkVariable<float>(100f);

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

    [ServerRpc(RequireOwnership = false)]
    public void ChangeStaminaServerRpc(float amount)
    {
        stamina.Value = Mathf.Clamp(stamina.Value + amount, 0f, 100f);
    }

    public bool TryConsumeStamina(float amount)
    {
        if (stamina.Value >= amount)
        {
            ChangeStaminaServerRpc(-amount);
            Debug.Log($"{OwnerClientId} stamina reduced. Stamina now: {stamina.Value}");
            return true;
        }
        return false;
    }

    public void RegenerateStamina(float amount)
    {
        if (stamina.Value < 100f)
        {
            ChangeStaminaServerRpc(amount);
            Debug.Log($"{OwnerClientId} stamina gained. Stamina now: {stamina.Value}");
        }
    }
}

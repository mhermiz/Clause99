using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private StaminaBar staminaBar;
    private NetworkVariable<float> health = new NetworkVariable<float>(100f);
    private NetworkVariable<float> stamina = new NetworkVariable<float>(100f);
    private const float maxHealth = 100f;
    private const float maxStamina = 100f;

    public NetworkVariable<int> mineralsCarried = new NetworkVariable<int>(0);
    public int mineralCapacity = 32;
    public bool CanCarryMore() => mineralsCarried.Value < mineralCapacity;

    public NetworkVariable<int> playerScore = new NetworkVariable<int>(0);
    private DamageFlash damageFlash;
    
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            health.Value = 100f;
            stamina.Value = 100f;
        }

        if (IsLocalPlayer)
        {
            // Find the health bar UI
            if (healthBar == null)
            {
                var uiRoot = GameObject.Find("PlayerUI");
                if (uiRoot != null)
                {
                    var hb = uiRoot.transform.Find("HealthBar");
                    if (hb != null)
                        healthBar = hb.GetComponent<HealthBar>();
                }
            }

            // Find the StaminaBar UI
            if (staminaBar == null)
            {
                var uiRoot = GameObject.Find("PlayerUI");
                if (uiRoot != null)
                {
                    var sb = uiRoot.transform.Find("StaminaBar");
                    if (sb != null)
                        staminaBar = sb.GetComponent<StaminaBar>();
                }
            }

            damageFlash = GameObject.Find("DamageOverlay").GetComponent<DamageFlash>();

            // Subscribe to health change events
            health.OnValueChanged += OnHealthChanged;
            stamina.OnValueChanged += OnStaminaChanged;

            // Initialize UI
            healthBar.SetHealth(health.Value, maxHealth);
            staminaBar.SetStamina(stamina.Value, maxStamina);
        }
    }

    private void OnHealthChanged(float oldValue, float newValue)
    {
        healthBar.SetHealth(newValue, maxHealth);
    }

    private void OnStaminaChanged(float oldValue, float newValue)
    {
        staminaBar.SetStamina(newValue, maxStamina);
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage)
    {
        if (!IsServer) return;
        health.Value -= damage;
        Debug.Log($"{OwnerClientId} took {damage} damage. Health now: {health.Value}");
        FlashDamageClientRpc(OwnerClientId);
    }

    [ClientRpc]
    private void FlashDamageClientRpc(ulong clientId)
    {
        if (IsLocalPlayer && damageFlash)
        {
            damageFlash.Flash();
        }
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

    public void AddMineral(int amount = 1)
    {
        mineralsCarried.Value = Mathf.Min(mineralsCarried.Value + amount, mineralCapacity);
    }

    public int DepositAllMinerals()
    {
        int deposited = mineralsCarried.Value;
        mineralsCarried.Value = 0;
        playerScore.Value += deposited;
        return deposited;
    }
}

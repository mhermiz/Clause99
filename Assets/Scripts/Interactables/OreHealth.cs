using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class OreHealth : NetworkBehaviour
{
    public float maxHealth = 50f;
    private float currentHealth;
    public GameObject coalPrefab;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        Debug.Log($"Ore took {amount} damage.");
        currentHealth -= amount;

        if (currentHealth <= 0)
            DestroyOre();
    }

    private void DestroyOre()
    {
        float spawnHeight = 1f;
        Vector3 spawnPos1 = transform.position + new Vector3(0, spawnHeight, 0);
        Vector3 spawnPos2 = transform.position + new Vector3(0, spawnHeight + 0.1f, 0);

        // Only the server spawns networked objects
        if (IsServer)
        {
            var coal1 = Instantiate(coalPrefab, spawnPos1, Quaternion.identity);
            var coal2 = Instantiate(coalPrefab, spawnPos2, Quaternion.identity);
            var netObj = coal1.GetComponent<NetworkObject>();
            var netObj2 = coal2.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                netObj.Spawn();
                netObj2.Spawn();
            }
            else
            {
                Debug.LogWarning("Coal prefab is missing NetworkObject component!");
            }
        }

        Destroy(gameObject); // Destroy the ore locally (server will sync)
    }
}
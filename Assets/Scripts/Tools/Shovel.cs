using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shovel : MonoBehaviour
{
    public float damage = 25f;

    private bool canDamage = false;

    // Called by Hotbar when the shovel is equipped or unequipped
    public void SetActiveWeapon(bool active)
    {
        canDamage = active;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canDamage) return;

        if (other.TryGetComponent<EnemyHealth>(out EnemyHealth enemy))
        {
            enemy.TakeDamage(damage);
        }
    }
}

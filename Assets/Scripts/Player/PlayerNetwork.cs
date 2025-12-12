using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    private GameObject pauseMenuUI;
    public static bool isPaused = false;
    private Rigidbody rb;
    private CapsuleCollider col;

    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundLayer;
    bool isGrounded;

    private float lastAttackTime = 0f;
    public ItemObjects equippedItem;
    [SerializeField] private LayerMask playerLayer; // Optional: Layer mask to filter punch hits
    [SerializeField] private LayerMask enemyLayer; // Layer mask to filter enemy hits
    [SerializeField] private LayerMask interactableLayer; // Layer mask to filter interactable hits

    private PlayerStats stats;
    private float staminaDelay = 0f;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            pauseMenuUI = GameObject.Find("PauseMenu");
            pauseMenuUI.SetActive(false);
            stats = GetComponent<PlayerStats>();
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {

        if (!IsOwner)
        {
            return; // Only the owner can control the character
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuUI.activeSelf)
            {
                pauseMenuUI.SetActive(false);
                isPaused = false;

            }
            else
            {
                pauseMenuUI.SetActive(true);
                isPaused = true;
            }
        }

        if (isPaused)
        {
            return; // Do not process movement when paused
        }

        // Jump input
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Camera-relative movement
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = right.y = 0;

        Vector3 movement = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");

        // Sprinting and stamina management
        float movespeed;
        bool isMoving = Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0;

        if (Input.GetKey(KeyCode.LeftShift) && isMoving && stats.TryConsumeStamina(20f * Time.deltaTime))
        {
            movespeed = 8f; // Sprint speed
            staminaDelay = 2f; // Reset delay
        }
        else
        {
            movespeed = 5f; // Normal speed
            if (staminaDelay > 0f)
            {
                staminaDelay -= Time.deltaTime;
            }
            else
            {
                stats.RegenerateStamina(10f * Time.deltaTime);
            }
        }
        
        // Player Movement
        rb.MovePosition(rb.position + movement.normalized * movespeed * Time.deltaTime);

        // Combat - Punch
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
    }

    private void Attack()
    {
        // No item = punching players only
        if (equippedItem == null)
        {
            Debug.Log("Punch!");
            TryDealDamageToPlayer(2f, 10f); 
            return;
        }

        // Only allow enemy attacks for a shovel
        if (equippedItem.toolType == ToolType.Shovel)
        {
            Debug.Log("Shovel Attack!");
            if (Time.time < lastAttackTime + equippedItem.cooldown)
            return;
            lastAttackTime = Time.time;
            TryDealDamageToEnemy(2f, 10f);
            return;
        }

        // Only allow pickaxe attacks
        if (equippedItem.toolType == ToolType.Pickaxe)
        {
            Debug.Log("Pickaxe Attack!");
            if (Time.time < lastAttackTime + equippedItem.cooldown)
            return;
            lastAttackTime = Time.time;
            TryDealDamageToOres(2f, 10f);
            return;
        }
    }

    private void TryDealDamageToPlayer(float range, float damage)
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, range))
        {
            if (hit.collider.TryGetComponent<PlayerStats>(out var stats))
            {
                stats.TakeDamageServerRpc(damage);
            }
        }
    }

    private void TryDealDamageToEnemy(float range, float damage)
    {   
        Debug.Log("Trying to deal damage to enemy");
        if (Physics.Raycast (
            Camera.main.transform.position,
            Camera.main.transform.forward,
            out RaycastHit hit,
            range,
            enemyLayer
        ))
        {
            if (hit.collider.TryGetComponent<EnemyHealth>(out var enemy))
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    private void TryDealDamageToOres(float range, float damage)
    {   
        Debug.Log("Trying to deal damage to ore");
        if (Physics.Raycast (
            Camera.main.transform.position,
            Camera.main.transform.forward,
            out RaycastHit hit,
            range,
            interactableLayer
        ))
        {
            if (hit.collider.TryGetComponent<OreHealth>(out var mineral))
            {
                mineral.TakeDamage(damage);
            }
        }
    }
}

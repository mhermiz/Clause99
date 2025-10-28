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

    private float punchRange = 2f;
    private float punchDamage = 10f;
    [SerializeField] private LayerMask playerLayer; // Optional: Layer mask to filter punch hits

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

        Vector3 movement = forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal");

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
        transform.Translate(movement.normalized * movespeed * Time.deltaTime, Space.World);

        // Combat - Punch
        if (Input.GetMouseButtonDown(0))
        {
            Punch();
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
    }

    private void Punch()
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, punchRange, playerLayer))
        {
            Debug.Log($"[Punch] Hit player: {hit.collider.gameObject.name}");

            var hitPlayerStats = hit.collider.gameObject.GetComponent<PlayerStats>();
            if (hitPlayerStats != null)
            {
                hitPlayerStats.TakeDamageServerRpc(punchDamage);
            }
        }
        else
        {
            Debug.Log("[Punch] Missed");
        }
    }
}

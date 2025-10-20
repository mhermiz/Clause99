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
    [SerializeField] private Transform spawnedObjectprefab;
    private Rigidbody rb;
    private CapsuleCollider col;

    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundLayer;
    bool isGrounded;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            pauseMenuUI = GameObject.Find("PauseMenu");
            pauseMenuUI.SetActive(false);
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

        if (Input.GetKeyDown(KeyCode.T))
        {
            Transform spawnedObjectTransform = Instantiate(spawnedObjectprefab);
            spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
            //randomNumber.Value = Random.Range(0, 100);//
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

        float movespeed = 5f;
        transform.Translate(movement.normalized * movespeed * Time.deltaTime, Space.World);
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
    }
}

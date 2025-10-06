using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerBody; // the parent capsule/body to rotate
    private Camera playerCamera;

    [Header("Settings")]
    public float mouseSensitivity = 100f;

    // Networked Y rotation
    private NetworkVariable<float> networkYaw = new NetworkVariable<float>(
        0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private float xRotation = 0f; // local camera pitch

    private void Awake()
    {
        playerCamera = GetComponent<Camera>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            playerCamera.enabled = false;
            GetComponent<AudioListener>().enabled = false;
        }
    }

    private void Update()
    {
        if (IsOwner)
        {
            HandleCameraInput();
        }
        else
        {
            ApplyNetworkRotation();
        }
    }

    private void HandleCameraInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate camera vertically (local only)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate player horizontally (yaw)
        playerBody.Rotate(Vector3.up * mouseX);

        // Update networked Y rotation
        networkYaw.Value = playerBody.eulerAngles.y;
    }

    private void ApplyNetworkRotation()
    {
        // Smoothly apply networked rotation for non-owners
        playerBody.rotation = Quaternion.Euler(0f, networkYaw.Value, 0f);
    }

    // Optional: allow dynamic sensitivity changes
    public void SetMouseSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
    }
}

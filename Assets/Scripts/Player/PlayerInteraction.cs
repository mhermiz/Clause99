using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteraction : NetworkBehaviour
{
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private float holdTime = 2f;
    [SerializeField] private GameObject promptText;
    private float holdTimer = 0f;
    private IInteractable currentInteractable;

    private void Start()
    {
        // Try to find the Interact UI text that exists in the PlayerUI canvas
        if (promptText == null)
        {
            var uiRoot = GameObject.Find("PlayerUI");
            if (uiRoot != null)
            {
                promptText = uiRoot.transform.Find("Interact")?.gameObject;
            }
        }

        if (promptText != null)
        {
            promptText.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return; // Only the owner can interact
        }

        // Raycast from camera
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, interactionRange))
        {
            var interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                currentInteractable = interactable;
                promptText.SetActive(true);

                if (Input.GetKey(KeyCode.E))
                {
                    if (currentInteractable.RequiresHold)
                    {
                        holdTimer += Time.deltaTime;
                        if (holdTimer >= holdTime)
                        {
                            currentInteractable.Interact(gameObject);
                            holdTimer = 0f; // Reset timer after interaction
                        }
                    }
                    else
                    {
                        currentInteractable.Interact(gameObject);

                        if (currentInteractable.ShouldDespawnAfterInteract && hit.collider.TryGetComponent(out NetworkObject netObj))
                        {
                            DespawnObjectServerRpc(netObj.NetworkObjectId);
                        }
                    }
                }
                else
                {
                    holdTimer = 0f; // Reset timer if key is released
                }
            }
            else
            {
                promptText.SetActive(false);
                currentInteractable = null;
                holdTimer = 0f; // Reset timer if not looking at an interactable
            }
        }
        else
        {
            promptText.SetActive(false);
            currentInteractable = null;
            holdTimer = 0f; // Reset timer if nothing is hit
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnObjectServerRpc(ulong networkObjectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject netObj))
        {
            netObj.Despawn();
        }
    }
}
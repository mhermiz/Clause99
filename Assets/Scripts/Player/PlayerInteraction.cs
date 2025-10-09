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
                Debug.Log("Looking at interactable");

                if (Input.GetKey(KeyCode.E))
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
}

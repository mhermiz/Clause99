using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterController : NetworkBehaviour
{
    [SerializeField] private Transform spawnedObjectprefab;

    private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
            randomNumber.OnValueChanged += (int previousValue, int newValue) =>
            {
                Debug.Log(OwnerClientId + " - " + randomNumber.Value);
            };
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

        if (Input.GetKeyDown(KeyCode.Y))
        {
            DestroyImmediate(spawnedObjectprefab.gameObject, true);    
        }

        // Camera-relative movement
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = right.y = 0;

        Vector3 movement = forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal");

        float movespeed = 5f;
        transform.Translate(movement.normalized * movespeed * Time.deltaTime, Space.World);
    }
}

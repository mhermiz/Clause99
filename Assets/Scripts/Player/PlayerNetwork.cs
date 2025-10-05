using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
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
            Destroy(spawnedObjectprefab.gameObject);
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        float movespeed = 5f;
        transform.Translate(movement * Time.deltaTime * movespeed);
    }
}

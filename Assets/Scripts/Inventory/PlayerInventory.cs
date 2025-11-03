using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    private NetworkList<int> itemIDs; // holds item IDs
    private int selectedItemIndex = 0;

    [SerializeField] private Transform dropPoint;
    [SerializeField] private GameObject itemPrefab;


    private void Awake()
    {
        itemIDs = new NetworkList<int>();
        itemIDs.OnListChanged += OnInventoryChanged;
    }

    private void OnInventoryChanged(NetworkListEvent<int> change)
    {
        // You can refresh UI or visuals here
        Debug.Log($"Inventory changed: now has {itemIDs.Count} items");
    }
    
    public override void OnNetworkSpawn()
    {
        itemIDs.OnListChanged += OnInventoryChanged;
        
        if (!IsOwner) return;
        if (itemIDs.Count > 0)
            SelectItem(0);
    }

    private void Update()
    {
        if (!IsOwner) return;

        HandleHotbarInput();
        HandleDropInput();
    }

    private void HandleHotbarInput()
    {
        // Press 1, 2, 3, etc. to select an item
        for (int i = 0; i < itemIDs.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectItem(i);
                break;
            }
        }
    }

    private void HandleDropInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropSelectedItem();
        }
    }

    private void SelectItem(int index)
    {
        if (index < 0 || index >= itemIDs.Count) return;

        selectedItemIndex = index;
        ItemObjects selectedItem = ItemDatabase.GetItemByID(itemIDs[selectedItemIndex]);

        Debug.Log($"Selected: {selectedItem}");
        // 🔹 Optional: Update hotbar UI highlight or held object in hand here
    }

    private void DropSelectedItem()
    {
        if (itemIDs.Count == 0) return;
        int itemID = itemIDs[selectedItemIndex];

        if (IsServer)
        SpawnDroppedItem(itemID);
        else
        DropItemServerRpc(itemID);

        itemIDs.RemoveAt(selectedItemIndex);
        selectedItemIndex = Mathf.Clamp(selectedItemIndex - 1, 0, itemIDs.Count - 1);
    } 

    [ServerRpc(RequireOwnership = false)]
    private void DropItemServerRpc(int itemID)
    {
        SpawnDroppedItem(itemID);
    }

    private void SpawnDroppedItem(int itemID)
    {
        // Create dropped item
        var droppedItemData = ItemDatabase.GetItemByID(itemID);
        GameObject dropped = Instantiate(droppedItemData.prefab.gameObject, dropPoint.position, Quaternion.identity);
        dropped.GetComponent<NetworkObject>().Spawn(true);
    }

    // Call this to add items to the inventory
    public void AddItem(ItemObjects newItem)
    {
        if (IsServer)
        {
            itemIDs.Add(ItemDatabase.GetItemID(newItem));
        }
        else
        {
            AddItemServerRpc(ItemDatabase.GetItemID(newItem));
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddItemServerRpc(int itemID)
    {
        itemIDs.Add(itemID);
    }
}

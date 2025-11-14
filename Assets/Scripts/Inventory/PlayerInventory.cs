using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    private NetworkList<int> itemIDs; // holds item IDs
    private int selectedItemIndex = 0;
    private NetworkVariable<int> equippedItemID = new NetworkVariable<int>(-1);

    [SerializeField] private Transform dropPoint;
    [SerializeField] private Transform handSlot;
    [SerializeField] private GameObject currentHeldItem;

    [SerializeField] private HotbarUI hotbarUI;

    private void Start()
    {
        if (IsOwner)
            hotbarUI = FindObjectOfType<HotbarUI>();
    }

    private void Awake()
    {
        itemIDs = new NetworkList<int>();
        itemIDs.OnListChanged += OnInventoryChanged;
    }

    private void OnInventoryChanged(NetworkListEvent<int> change)
    {
        Debug.Log($"Inventory changed: now has {itemIDs.Count} items");
        if (IsOwner && hotbarUI != null)
        {
            // copy NetworkList<int> to a regular List<int> without relying on IEnumerable conversion
            var ids = new List<int>();
            for (int i = 0; i < itemIDs.Count; i++)
            {
                ids.Add(itemIDs[i]);
            }
            hotbarUI.UpdateHotbarSprites(ids, selectedItemIndex);
        }
    }

    private void OnEquippedItemChanged(int oldValue, int newValue)
    {
        // Local owner already handles visuals
        if (IsOwner) return;

        // Remove held item if unequipped
        if (newValue == -1)
        {
            if (currentHeldItem != null)
            {
                Destroy(currentHeldItem);
                currentHeldItem = null;
            }
            return;
        }

        // Update held item for remote players
        ItemObjects newItem = ItemDatabase.GetItemByID(newValue);
        UpdateHeldItem(newItem);
    }

    public override void OnNetworkSpawn()
    {
        itemIDs.OnListChanged += OnInventoryChanged;
        equippedItemID.OnValueChanged += OnEquippedItemChanged;

        if (!IsOwner) return;
        if (itemIDs.Count > 0)
            SelectItem(0);
    }
    
    public override void OnNetworkDespawn()
    {
        itemIDs.OnListChanged -= OnInventoryChanged;
        equippedItemID.OnValueChanged -= OnEquippedItemChanged;
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
                // if the same slot is pressed again
                if (selectedItemIndex == i && currentHeldItem != null)
                {
                    UnequipItem();
                }
                else
				{
				    SelectItem(i);
				}
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

    private void UnequipItem()
    {
        selectedItemIndex = -1;
        equippedItemID.Value = -1; // broadcast unequip
        Debug.Log("Unequipped item");
        if (hotbarUI != null)
        {
            var ids = new List<int>();
            for (int i = 0; i < itemIDs.Count; i++)
            {
                ids.Add(itemIDs[i]);
            }
            hotbarUI.UpdateHotbarSprites(ids, selectedItemIndex);
        }

        // Remove held item
        if (currentHeldItem != null) {
            Destroy(currentHeldItem);
            currentHeldItem = null;
        }
    }

    private void SelectItem(int index)
    {
        if (index < 0 || index >= itemIDs.Count) return;

        selectedItemIndex = index;
        int itemID = itemIDs[selectedItemIndex];
        equippedItemID.Value = itemID; // sync across network

        ItemObjects selectedItem = ItemDatabase.GetItemByID(itemIDs[selectedItemIndex]);
        if (selectedItem == null)
        {
            return;
        }

        Debug.Log($"Selected: {selectedItem}");
        if (IsOwner && hotbarUI != null)
        {
            var ids = new List<int>();
            for (int i = 0; i < itemIDs.Count; i++)
            {
                ids.Add(itemIDs[i]);
            }
            hotbarUI.UpdateHotbarSprites(ids, selectedItemIndex);
        }

        if (IsOwner)
            UpdateHeldItem(selectedItem);
    }

    private void UpdateHeldItem(ItemObjects selectedItem)
    {
    // Remove old held item
    if (currentHeldItem != null)
        Destroy(currentHeldItem);

    if (selectedItem == null || selectedItem.prefab == null)
        return;

    // Instantiate the item in the hand slot
    currentHeldItem = Instantiate(selectedItem.prefab, handSlot);

    // Adjust its local transform if needed
    currentHeldItem.transform.localPosition = Vector3.zero;
    currentHeldItem.transform.localRotation = Quaternion.identity;

    // disable colliders so it doesn’t hit the player
    foreach (var col in currentHeldItem.GetComponentsInChildren<Collider>())
        col.enabled = false;
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
        // Remove held item if it's the one being dropped
        if (currentHeldItem != null)
		{
			Destroy(currentHeldItem);
            currentHeldItem = null;
		}

        // Create dropped item
        var droppedItemData = ItemDatabase.GetItemByID(itemID);
        GameObject dropped = Instantiate(droppedItemData.prefab.gameObject, dropPoint.position, Quaternion.identity);
        
        var toolInteraction = dropped.GetComponent<ToolInteraction>();
        if (toolInteraction != null)
            toolInteraction.AssignItem(droppedItemData);
        
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

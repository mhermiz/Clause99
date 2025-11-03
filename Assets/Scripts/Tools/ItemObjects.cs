using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item Object")]
public class ItemObjects : ScriptableObject
{
    [Header("Item Properties")]
    [SerializeField] private string itemName;
    [SerializeField] public GameObject prefab;
    [SerializeField] public Sprite item_sprite;
}

public static class ItemDatabase
{
    public static List<ItemObjects> AllItems = new List<ItemObjects>();

    public static int GetItemID(ItemObjects obj) => AllItems.IndexOf(obj);
    public static ItemObjects GetItemByID(int id) => AllItems[id];
}
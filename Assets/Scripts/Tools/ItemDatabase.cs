using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemDatabase
{
    public static List<ItemObjects> AllItems = new List<ItemObjects>();

    static ItemDatabase()
    {
        LoadAllItems();
    }

    public static void LoadAllItems()
    {
        AllItems.Clear();
        ItemObjects[] loadedItems = Resources.LoadAll<ItemObjects>("");

        foreach (var item in loadedItems)
        {
            AllItems.Add(item);
        }

        Debug.Log($"[ItemDatabase] Loaded {AllItems.Count} items into database.");
    }

    public static int GetItemID(ItemObjects obj)
    {
        int id = AllItems.IndexOf(obj);
        if (id == -1)
        {
            Debug.LogWarning($"[ItemDatabase] {obj.name} not found in AllItems list!");
        }
        return id;
    }

    public static ItemObjects GetItemByID(int id)
    {
        if (id < 0 || id >= AllItems.Count)
        {
            Debug.LogError($"[ItemDatabase] Invalid item ID {id}. List size: {AllItems.Count}");
            return null;
        }
        return AllItems[id];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotbarUI : MonoBehaviour
{
    [SerializeField] private List<Image> slotIcons; // Drag each ItemIcon Image here in Inspector
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color normalColor = new Color(1, 1, 1, 0.4f);

    // Called by PlayerInventory when items or selection change
    public void UpdateHotbarSprites(List<int> itemIDs, int selectedIndex)
    {
        for (int i = 0; i < slotIcons.Count; i++)
        {
            if (i < itemIDs.Count)
            {
                ItemObjects item = ItemDatabase.GetItemByID(itemIDs[i]);
                slotIcons[i].sprite = item.item_sprite;
                slotIcons[i].color = i == selectedIndex ? selectedColor : normalColor;
            }
            else
            {
                slotIcons[i].sprite = null;
                slotIcons[i].color = normalColor;
            }
        }
    }
}

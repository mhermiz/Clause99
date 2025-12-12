using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToolType
{
    None,
    Shovel,
    Pickaxe,
    Dynamite
}

[CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item Object")]
public class ItemObjects : ScriptableObject
{
    [Header("Item Properties")]
    [SerializeField] private string itemName;
    [SerializeField] public GameObject prefab;
    [SerializeField] public Sprite item_sprite;

    [Header("Tool Properties")]
    public bool isWeapon = false;
    public ToolType toolType = ToolType.None;
    public float damage = 0f;
    public float range = 2f;
    public float cooldown = 0.5f;
}
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Item : NetworkBehaviour
{
    [SerializeField] private ItemObjects itemObject;
    public ItemObjects ItemObject => itemObject;
}

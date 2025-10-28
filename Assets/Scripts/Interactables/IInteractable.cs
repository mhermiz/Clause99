using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{

    bool RequiresHold { get; }
    void Interact(GameObject player);
}

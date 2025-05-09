using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    public Item item;

    public void Interact(PlayerStats stats)
    {
        InventoryManager.Instance.AddItem(item);
        Destroy(gameObject);
    }
}

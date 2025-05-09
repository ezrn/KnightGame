using UnityEngine;

[System.Serializable]
public class InventoryEntry
{
    public Item item;
    public int quantity = 1; 

    public InventoryEntry(Item item, int qty = 1) // helper for the bag inventory
    {
        this.item = item;
        this.quantity = Mathf.Max(1, qty);
    }
}

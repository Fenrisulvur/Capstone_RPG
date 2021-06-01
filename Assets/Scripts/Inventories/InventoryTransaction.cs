using System.Collections.Generic;
using RPG.Inventories;
using UnityEngine;

public class InventoryTransaction : MonoBehaviour
{
    [SerializeField] private List<InventoryItem> items;

    public void GiveItems()
    {
        Inventory inventory = Inventory.GetPlayerInventory();
        foreach (InventoryItem item in items)
        {
            inventory.AddToFirstEmptySlot(item, 1);
        }
    }

    public void TakeItems()
    {
        Inventory inventory = Inventory.GetPlayerInventory();
        foreach (InventoryItem item in items)
        {
            Debug.Log($"Removing {item}");
            inventory.RemoveItem(item, 1);
        }
    }
}
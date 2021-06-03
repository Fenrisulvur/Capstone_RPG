using System;
using RPG.Inventories;
using TMPro;
using UnityEngine;

namespace RPG.Shops
{
    public class ShopItem
    {
        InventoryItem item;
        int availability;
        int price;
        int quantityInTransaction;

        public ShopItem(InventoryItem item, int availability, int price, int quantityInTransaction)
        {
            this.item = item;
            this.availability = availability;
            this.price = price;
            this.quantityInTransaction = quantityInTransaction;
        }

        public string GetName()
        {
            return item.GetDisplayName();
        }

        public int GetPrice()
        {
            return price;
        }

        public Sprite GetIcon()
        {
            return item.GetIcon();
        }

        public int GetAvailability()
        {
            return availability;
        }

        public int GetQtyInTransaction()
        {
            return quantityInTransaction;
        }

        public InventoryItem GetInventoryItem()
        {
            return item;
        }
    }
}
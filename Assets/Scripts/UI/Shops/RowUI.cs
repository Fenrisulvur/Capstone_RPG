using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class RowUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI nameField;
        [SerializeField] TextMeshProUGUI priceField;
        [SerializeField] TextMeshProUGUI availabilityField;
        [SerializeField] TextMeshProUGUI qtyField;
        [SerializeField] Image iconField;

        Shop currentShop = null;
        ShopItem item = null;

        internal void Setup(Shop currentShop, ShopItem item)
        {
            this.currentShop = currentShop;
            this.item = item;
            nameField.text = item.GetName();
            priceField.text = $"{item.GetPrice()}g";
            availabilityField.text = $"{item.GetAvailability():N0}";
            iconField.sprite = item.GetIcon();
            qtyField.text = $"{item.GetQtyInTransaction()}";
        }

        public void Add()
        {
            currentShop.AddToTransaction(item.GetInventoryItem(), 1);
        }

        public void Remove()
        {
            currentShop.AddToTransaction(item.GetInventoryItem(), -1);
        }
    }
}

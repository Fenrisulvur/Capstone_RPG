using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.Inventories;
using RPG.Saving;
using UnityEngine;

namespace RPG.Shops
{
    public class Shop : MonoBehaviour, IRaycastable, ISaveable
    {
        [SerializeField] string shopName;
        [SerializeField] float sellingPercentage = 80f;
        [SerializeField] StockItemConfig[] stockConfig;

        [System.Serializable]
        class StockItemConfig
        {
            public InventoryItem item;
            public int initialStock;
            [Range(0,100)]
            public float buyingDiscountPercentage;
        }

        Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();
        Dictionary<InventoryItem, int> stock = new Dictionary<InventoryItem, int>();
        Shopper currentShopper = null;
        bool isBuyingMode = true;
        ItemCategory filter = ItemCategory.none;

        public event Action onChange;

        private void Awake() {
            foreach (StockItemConfig config in stockConfig)
            {
                stock[config.item] = config.initialStock;
            }
        }

        public IEnumerable<ShopItem> GetFilteredItems()
        {
            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                if (filter == ItemCategory.none || item.GetCategory() == filter)
                {
                    yield return shopItem;
                }
            }
        }

        public IEnumerable<ShopItem> GetAllItems()
        {
            foreach (StockItemConfig config in stockConfig)
            {
                int price = GetPrice(config);
                int quanitiyInTransaction = 0;
                transaction.TryGetValue(config.item, out quanitiyInTransaction);
                int currentAvailability = GetAvailability(config.item);
                yield return new ShopItem(config.item, currentAvailability, price, quanitiyInTransaction);
            }
        }

        private int GetAvailability(InventoryItem item)
        {
            if (isBuyingMode)
            {
                return stock[item];
            }
            return CountItemsInInventory(item);
        }

        public void SelectFilter(ItemCategory category)
        {
            filter = category;

            if (onChange != null)
            {
                onChange();
            }
        }

        public ItemCategory GetFilter() 
        {
            return filter;
        }

        public void SetShopper(Shopper shopper)
        {
            this.currentShopper = shopper;
        }

        public void SelectMode(bool isBuying)
        {
            isBuyingMode = isBuying;
            if (onChange != null)
            {
                onChange();
            }
        }

        public bool IsBuyingMode() 
        { 
            return isBuyingMode;
        }

        internal string GetShopName()
        {
            return shopName;
        }

        public int TransactionTotal()
        {
            int total = 0;
            foreach (ShopItem item in GetAllItems())
            {
                total += item.GetPrice() * item.GetQtyInTransaction();
            }
            return total;
        }

        public bool CanTransact()
        {
            if (IsTransactionEmpty()) return false;
            if (!HasSufficientFunds()) return false;
            if (!HasInventorySpace()) return false;

            return true;
        }

        public bool HasInventorySpace()
        {
            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            if (shopperInventory == null) return false;

            List<InventoryItem> flatItems = new List<InventoryItem>();
            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQtyInTransaction();
                for (int i = 0; i < quantity; i++)
                {
                    flatItems.Add(item);
                }
            }

            return shopperInventory.HasSpaceFor(flatItems);
        }

        public bool HasSufficientFunds()
        {
            if (!isBuyingMode) return true;
            Purse purse = currentShopper.GetComponent<Purse>();
            if (purse == null) return false;
            return purse.GetBalance() >= TransactionTotal();
        }

        public bool IsTransactionEmpty()
        {
            if (!isBuyingMode) return true;
            return transaction.Count == 0;
        }

        public void ConfirmTransaction()
        {
            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            Purse shopperPurse = currentShopper.GetComponent<Purse>();

            if (shopperInventory == null) return;
            
  
            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQtyInTransaction();
                int price = shopItem.GetPrice();
                for (int i = 0; i < quantity; i++)
                {
                    if (isBuyingMode)
                    {
                        BuyItem(shopperInventory, shopperPurse, item, price);
                    }
                    else
                    {
                        SellItem(shopperInventory, shopperPurse, item, price);
                    }
                } 
            }

            onChange();
        }

        public void AddToTransaction(InventoryItem item, int quantity)
        {
            if (!transaction.ContainsKey(item))
            {
                transaction[item] = 0; 
            }

            int availability = GetAvailability(item);
            if (transaction[item] + quantity > availability)
            {
               transaction[item] = stock[item]; 
            }
            else
            {
                transaction[item] += quantity;

                if (transaction[item] <= 0)
                {
                    transaction.Remove(item);
                }
                if(onChange != null)
                {
                    onChange();
                }
            }
        }

        public CursorType GetCursorType()
        {
            return CursorType.Shop;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<Shopper>().SetActiveShop(this);
            }
            return true;
        }

        private int CountItemsInInventory(InventoryItem item)
        {
            Inventory inventory = currentShopper.GetComponent<Inventory>();
            if (inventory == null) return 0;

            int total = 0;
            for (int i = 0; i < inventory.GetSize(); i++)
            {
                if (inventory.GetItemInSlot(i) == item)
                {
                    total += inventory.GetNumberInSlot(i);
                }
            }
            return total;
        }

        private int GetPrice(StockItemConfig config)
        {
            if (isBuyingMode)
            {
                return (int)(config.item.GetPrice() * (1 - config.buyingDiscountPercentage / 100));
            }

            return (int)(config.item.GetPrice() * (1 - sellingPercentage / 100));

        }

        private void SellItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, int price)
        {
            int slot = FindFirstItemSlot(shopperInventory, item);
            if (slot == -1) return;

            AddToTransaction(item, -1);
            shopperInventory.RemoveFromSlot(slot, 1);
            stock[item]++;
            shopperPurse.UpdateBalance(price);

        }

        private int FindFirstItemSlot(Inventory shopperInventory, InventoryItem item)
        {
            for (int i = 0; i < shopperInventory.GetSize(); i++)
            {
                if (shopperInventory.GetItemInSlot(i) == item)
                {
                    return i;
                }
            }

            return -1;
        }

        private void BuyItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, int price)
        {
            if (shopperPurse.GetBalance() < price) return;

            bool success = shopperInventory.AddToFirstEmptySlot(item, 1);
            if (success)
            {
                AddToTransaction(item, -1);
                stock[item]--;
                shopperPurse.UpdateBalance(-price);
            }
        }

        public object CaptureState()
        {
            Dictionary<string, int> saveObject = new Dictionary<string, int>();
            foreach (var pair in stock)
            {
                saveObject[pair.Key.GetItemID()] = pair.Value;
            }
            return saveObject;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, int> saveObject = (Dictionary<string, int>) state;
            stock.Clear();
            foreach (var pair in saveObject)
            {
                stock[InventoryItem.GetFromID(pair.Key)] = pair.Value;
            }
        }
    }
}

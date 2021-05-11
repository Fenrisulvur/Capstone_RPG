using System;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Combat;
using RPG.UI;

namespace RPG.Inventories
{
    /// <summary>
    /// Provides a store for the items equipped to a player. Items are stored by
    /// their equip locations.
    /// 
    /// This component should be placed on the GameObject tagged "Player".
    /// </summary>
    public class Equipment : MonoBehaviour, ISaveable
    {
        // STATE
        Dictionary<EquipLocation, EquipableItem> equippedItems = new Dictionary<EquipLocation, EquipableItem>();

        // PUBLIC

        /// <summary>
        /// Broadcasts when the items in the slots are added/removed.
        /// </summary>
        public event Action equipmentUpdated;

        Inventory inventory;

        private void Awake() {
            inventory = GetComponent<Inventory>();
        }

        private void Start() {
            equipmentUpdated();
        }
        /// <summary>
        /// Return the item in the given equip location.
        /// </summary>
        public EquipableItem GetItemInSlot(EquipLocation equipLocation)
        {
            if (!equippedItems.ContainsKey(equipLocation))
            {
                return null;
            }

            return equippedItems[equipLocation];
        }

        /// <summary>
        /// Add an item to the given equip location. Do not attempt to equip to
        /// an incompatible slot.
        /// </summary>
        public void AddItem(EquipLocation slot, EquipableItem item)
        {
            if (item != null && !CanHandleSwap(slot, item))
            {
                GetComponent<AlertPopup>().Send("You can not equip a shield and a two hander.");
                inventory.AddToFirstEmptySlot(item, 1);
                return;
            }    
            Debug.Assert(item.GetAllowedEquipLocation() == slot);
            equippedItems[slot] = item;

            if (equipmentUpdated != null)
            {
                equipmentUpdated();
            }
        }

        public bool CanHandleSwap(EquipLocation slot, EquipableItem item)
        {
            if (slot != EquipLocation.Weapon && slot != EquipLocation.Shield) return true;

            if (slot == EquipLocation.Weapon && item is WeaponConfig aweapon && aweapon.IsTwoHanded() && GetItemInSlot(EquipLocation.Shield) == null)
            {
                return true;
            }

            if (slot == EquipLocation.Weapon && item is WeaponConfig cweapon && !cweapon.IsTwoHanded())
            {
                return true;
            }

            if (slot == EquipLocation.Shield && GetItemInSlot(EquipLocation.Weapon) is WeaponConfig bweapon && !bweapon.IsTwoHanded())
            {
                return true;
            }

            if (slot == EquipLocation.Shield && GetItemInSlot(EquipLocation.Weapon) == null)
            {
                return true;
            }

            return false;

        }

        public void ReturnToInventory(EquipLocation equipLoc, int index)
        {
            inventory.AddItemToSlot(index, GetItemInSlot(equipLoc), 1);
        }


        /// <summary>
        /// Remove the item for the given slot.
        /// </summary>
        public void RemoveItem(EquipLocation slot)
        {
            equippedItems.Remove(slot);
            if (equipmentUpdated != null)
            {
                equipmentUpdated();
            }
        }

        /// <summary>
        /// Enumerate through all the slots that currently contain items.
        /// </summary>
        public IEnumerable<EquipLocation> GetAllPopulatedSlots()
        {
            return equippedItems.Keys;
        }

        // PRIVATE

        object ISaveable.CaptureState()
        {
            var equippedItemsForSerialization = new Dictionary<EquipLocation, string>();
            foreach (var pair in equippedItems)
            {
                equippedItemsForSerialization[pair.Key] = pair.Value.GetItemID();
            }
            return equippedItemsForSerialization;
        }

        void ISaveable.RestoreState(object state)
        {
            equippedItems = new Dictionary<EquipLocation, EquipableItem>();

            var equippedItemsForSerialization = (Dictionary<EquipLocation, string>)state;

            foreach (var pair in equippedItemsForSerialization)
            {
                var item = (EquipableItem)InventoryItem.GetFromID(pair.Value);
                if (item != null)
                {
                    equippedItems[pair.Key] = item;
                }
            }
        }
    }
}
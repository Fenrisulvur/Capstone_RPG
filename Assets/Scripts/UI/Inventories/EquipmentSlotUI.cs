using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Core.UI.Dragging;
using RPG.Inventories;
using UnityEngine.EventSystems;
using RPG.UI;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// An slot for the players equipment.
    /// </summary>
    public class EquipmentSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>, IPointerClickHandler
    {
        // CONFIG DATA

        [SerializeField] InventoryItemIcon icon = null;
        [SerializeField] EquipLocation equipLocation = EquipLocation.Weapon;

        // CACHE
        Equipment playerEquipment;

        // LIFECYCLE METHODS
       
        private void Awake() 
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            playerEquipment = player.GetComponent<Equipment>();
            playerEquipment.equipmentUpdated += RedrawUI;
        }

        private void Start() 
        {
            RedrawUI();
        }

        // PUBLIC

        public int MaxAcceptable(InventoryItem item)
        {
            EquipableItem equipableItem = item as EquipableItem;
            if (equipableItem == null) return 0;
            if (equipableItem.GetAllowedEquipLocation() != equipLocation) return 0;
            if (GetItem() != null) return 0;
            if ( (equipableItem.GetAllowedEquipLocation() == EquipLocation.Weapon || equipableItem.GetAllowedEquipLocation() == EquipLocation.Weapon) && !playerEquipment.CanHandleSwap(equipableItem.GetAllowedEquipLocation(), equipableItem)) 
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                player.GetComponent<AlertPopup>().Send("You can not equip a shield and a two hander.");
                return 0;
            }
            return 1;
        }

        public void AddItems(InventoryItem item, int number)
        {
            playerEquipment.AddItem(equipLocation, (EquipableItem) item);
        }

        public InventoryItem GetItem()
        {
            return playerEquipment.GetItemInSlot(equipLocation);
        }

        public int GetNumber()
        {
            if (GetItem() != null)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public void RemoveItems(int number)
        {
            playerEquipment.RemoveItem(equipLocation);
        }

        // PRIVATE

        void RedrawUI()
        {
            icon.SetItem(playerEquipment.GetItemInSlot(equipLocation));
        }

        public void AttemptUse()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            Inventory playerInventory = player.GetComponent<Inventory>();
            if (GetItem() != null && playerInventory.FindEmptySlot() != -1)
            {
                playerInventory.AddItemToSlot(playerInventory.FindEmptySlot(), GetItem(), 1);
                RemoveItems(1);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
                AttemptUse();
        }
    }
}
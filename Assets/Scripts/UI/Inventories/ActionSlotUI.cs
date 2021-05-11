using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Core.UI.Dragging;
using RPG.Inventories;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// The UI slot for the player action bar.
    /// </summary>
    public class ActionSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>, IPointerClickHandler
    {
        // CONFIG DATA
        [SerializeField] InventoryItemIcon icon = null;
        [SerializeField] int index = 0;
        [SerializeField] Image cooldownImg = null;
        float Cooldown = 0;
        float CooldownTimer = 0;
        // CACHE
        ActionStore store;
        CooldownManager cooldownManager;
        GameObject player;
        // LIFECYCLE METHODS
        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            store = player.GetComponent<ActionStore>();
            store.storeUpdated += UpdateIcon;
            
            cooldownManager = player.GetComponent<CooldownManager>();
            cooldownManager.cooldownTickEvent += UpdateCooldowns;
        }

        // PUBLIC

        public void AddItems(InventoryItem item, int number)
        {
            store.AddAction(item, index, number);
        }

        public InventoryItem GetItem()
        {
            return store.GetAction(index);
        }

        public int GetNumber()
        {
            return store.GetNumber(index);
        }

        public int MaxAcceptable(InventoryItem item)
        {
            return store.MaxAcceptable(item, index);
        }

        public void RemoveItems(int number)
        {
            store.RemoveItems(index, number);
        }

        void UpdateIcon()
        {
            
            icon.SetItem(GetItem(), GetNumber());
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            store.Use(index, player);
        }

        private void UpdateCooldowns()
        {
            if (GetItem() != null)
            {
                
                ActionItem thisItem = GetItem() as ActionItem;
                float cd = thisItem.GetCooldown(player);
                cooldownImg.fillAmount = Mathf.Clamp01( 1 - (thisItem.GetCDTime() - cd) / thisItem.GetCDTime()) ;
            }
            else
            {
                cooldownImg.fillAmount = 0;
            }
        }
    }
}

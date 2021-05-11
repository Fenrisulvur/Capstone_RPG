using System;
using RPG.Core;
using UnityEngine;

namespace RPG.Inventories
{
    /// <summary>
    /// An inventory item that can be placed in the action bar and "Used".
    /// </summary>
    /// <remarks>
    /// This class should be used as a base. Subclasses must implement the `Use`
    /// method.
    /// </remarks>
    public class ActionItem : InventoryItem
    {
        // CONFIG DATA
        [Tooltip("Does an instance of this item get consumed every time it's used.")]
        [SerializeField] bool consumable = false;
        [SerializeField] float cooldown = 1f;

        // PUBLIC

        /// <summary>
        /// Trigger the use of this item. Override to provide functionality.
        /// </summary>
        /// <param name="user">The character that is using this action.</param>
        public virtual void Use(GameObject user) 
        { 
            Debug.Log("No action");
        }

        protected void InitiateCooldown(GameObject user)
        {
            user.GetComponent<CooldownManager>().AddCD(this.name, cooldown);
        }

        public float GetCooldown(GameObject user)
        {
            return user.GetComponent<CooldownManager>().GetCD(this.name);
        }

        public bool IsConsumable()
        {
            return consumable;
        }

        public float GetCDTime()
        {
            return cooldown;
        }

        public bool CanUse(GameObject user)
        {
            float cd = GetCooldown(user);
            if (cd > 0)
            {
                Debug.Log("Item is coolding down. " + this.name + " Time Left: " + cd);
                return false;
            }
            return true;
        }
    }
}
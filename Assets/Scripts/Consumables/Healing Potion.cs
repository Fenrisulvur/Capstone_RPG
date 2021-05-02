using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Inventories;
using UnityEngine;


namespace RPG.Consumables
{
    [CreateAssetMenu(menuName = ("RPG/InventorySystem/Healing Potion"))]
    public class HealingPotion : ActionItem
    {
        [SerializeField] float healthToHeal = 10f;
        [SerializeField] bool isPercentage = false;
        [SerializeField] GameObject potionUseFX = null;

        public override void Use(GameObject user)
        {
            Debug.Log("Using Healing Potion");
            Health health = user.GetComponent<Health>();
            if (!health) return;
            
            Debug.Log("Healing");
            if (isPercentage)
            {
                health.Heal( health.GetHealthMaxValue()*(healthToHeal/100) );
            }
            else
            {
                health.Heal(healthToHeal);
            }

            if (potionUseFX == null) return;
            Debug.Log("Applying FX");
            Instantiate(potionUseFX, user.transform);
            
        }
    }
}

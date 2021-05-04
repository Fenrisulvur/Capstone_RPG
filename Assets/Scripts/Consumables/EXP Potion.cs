using System.Collections;
using System.Collections.Generic;
using RPG.Inventories;
using RPG.Stats;
using UnityEngine;

namespace RPG.Consumables
{
    [CreateAssetMenu(menuName = ("RPG/InventorySystem/Exp Potion"))]
    public class EXPPotion : ActionItem
    {
        [SerializeField] float expToGrant = 0f;
        [SerializeField] bool isPercentage = false;
        [SerializeField] GameObject potionUseFX = null;

        public override void Use(GameObject user)
        {
            Experience experience = user.GetComponent<Experience>();
            if (!experience) return;
            experience.GainExperience(expToGrant);

            if (potionUseFX == null) return;
            Debug.Log("Applying FX");
            Instantiate(potionUseFX, user.transform);
        }
    }

}
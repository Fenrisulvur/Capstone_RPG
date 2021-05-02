using UnityEngine;
using TMPro;
using RPG.Inventories;
using System;
using RPG.Stats;
using System.Collections;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// Root of the tooltip prefab to expose properties to other classes.
    /// </summary>
    public class ItemTooltip : MonoBehaviour
    {
        // CONFIG DATA
        [SerializeField] TextMeshProUGUI titleText = null;
        [SerializeField] TextMeshProUGUI bodyText = null;


        // PUBLIC

        public void Setup(InventoryItem item)
        {
            titleText.text = item.GetDisplayName();
            bodyText.text = item.GetDescription() + GetStatBonuses(item);
        }

        private string GetStatBonuses(InventoryItem item)
        {
            String statBuilderString = "\n\n";
            var itemData = InventoryItem.GetFromID(item.GetItemID());
            if (itemData == null) return "";
            if (!(itemData is IModifierProvider)) return "";

            var equipment = (IModifierProvider)itemData;

            if (!(equipment is StatsEquipableItem)) 
            {   
                var enumerable = equipment.GetAdditiveModifiers(Stat.Damage).GetEnumerator();
                enumerable.MoveNext();
                float dmg = enumerable.Current;
                statBuilderString += $"Weapon Damage: {dmg}";
            }

            statBuilderString += "\nStat bonuses: \n";

            foreach (Stats.Stat stat in Enum.GetValues(typeof(Stats.Stat)))
            {
                float total = 0f;
                if (stat != Stat.Damage)
                {
                    foreach (float modifier in equipment.GetAdditiveModifiers(stat))
                    {
                        total += modifier;
                    }
                    if (total > 0)
                    {
                        statBuilderString += $" +{total} {stat.ToString()} \n";
                        
                    }
                }

                total = 0f;
                foreach (float modifier in equipment.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
                if (total > 0)
                {
                    statBuilderString += $" +{total}% {stat.ToString()} \n";
                }
            }


            return statBuilderString;
        }
    }
}

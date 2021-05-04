using UnityEngine;
using RPG.Stats;
using System.Collections.Generic;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = "RPG/Inventory/(Stats)Equipable Item")]
    public class StatsEquipableItem : EquipableItem, IModifierProvider
    {
        [SerializeField] Modifier[] additiveModifiers;
        [SerializeField] Modifier[] percentageModifiers;
        [SerializeField] GameObject displayObject;

        [System.Serializable]
        struct Modifier
        {
            public Stat stat;
            public float value;
        }
        
        public GameObject GetDisplayObject()
        {
            return displayObject;
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        { 
            foreach (var modifier in additiveModifiers)
            {
                if (modifier.stat == stat)
                {
                    yield return modifier.value;
                }  
            }

        }


        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            foreach (var modifier in percentageModifiers)
            {
                if (modifier.stat == stat)
                {
                    yield return modifier.value;
                }
            }
        }


    }
}
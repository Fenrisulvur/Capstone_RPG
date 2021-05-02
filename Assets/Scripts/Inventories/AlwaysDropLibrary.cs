using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = ("RPG/Inventory/Drop Library(Always Drops)"))]
    public class AlwaysDropLibrary : ScriptableObject
    {
        [SerializeField] DropConfig[] Drops;

        [System.Serializable]
        class DropConfig
        {
            public InventoryItem item;
            public int amnt;
        }

        public struct Dropped
        {
            public InventoryItem item;
            public int number;
        }

        public IEnumerable<Dropped> GetDrops()
        {
            foreach (var drop in Drops)
            {
                var result = new Dropped();
                result.item = drop.item;
                result.number = drop.amnt;
                yield return result;
            }
        }


    }
}
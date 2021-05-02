using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;


namespace RPG.Inventories
{
    public class RandomDropper : ItemDropper
    {
        [Tooltip("How far can the pickups be scattered from the dropper.")]
        [SerializeField] float scatterDistance = 1;
        [SerializeField] DropLibrary dropLibrary;
        [SerializeField] AlwaysDropLibrary alwaysDropLibrary;
        const int MAX_ATTEMPTS = 30;

        public void RandomDrop()
        {
            DropDefaultItems(GetDropLocation());
            if (dropLibrary == null) return;

            BaseStats baseStats = GetComponent<BaseStats>();

            var drops = dropLibrary.GetRandomDrops(baseStats.GetLevel());
            foreach (var drop in drops)
            {
                DropItem(drop.item, drop.number); 
            }
        
        }


        public void RandomDrop(int level, Vector3 position)
        {
            DropDefaultItems(position);
            if (dropLibrary == null) return;

            BaseStats baseStats = GetComponent<BaseStats>();

            var drops = dropLibrary.GetRandomDrops(level);
            foreach (var drop in drops)
            {
                SpawnPickup(drop.item, position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)), drop.number);
            }

        }

        private void DropDefaultItems(Vector3 position)
        {
            if (!alwaysDropLibrary) return;
            var drops = alwaysDropLibrary.GetDrops();
            foreach (var drop in drops)
            {
                Debug.Log(drop.item.name);
                SpawnPickup(drop.item, position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)), drop.number);
            }
        }

        protected override Vector3 GetDropLocation()
        {
            for (int i = 0; i < MAX_ATTEMPTS; i++)
            {
                Vector3 randomPoint = transform.position + Random.insideUnitSphere * scatterDistance;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 0.1f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }

            return transform.position;
        }
    }
}
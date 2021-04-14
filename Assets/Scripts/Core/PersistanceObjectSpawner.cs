using UnityEngine;

namespace RPG.Core
{
    public class PersistanceObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject persistantObjectPrefab = null;

        static bool hasSpawned = false;

        private void Awake()
        {
            if (hasSpawned || persistantObjectPrefab == null) return;

            SpawnPersistantObjects();

            hasSpawned = true;
        }

        private void SpawnPersistantObjects()
        {
            GameObject persistentObject = Instantiate(persistantObjectPrefab);
            DontDestroyOnLoad(persistentObject);
        }
    }   
}

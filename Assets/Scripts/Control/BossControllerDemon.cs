using UnityEngine;
using RPG.Combat;
using RPG.Attributes;
using UnityEngine.AI;

namespace RPG.Control
{
    public class BossControllerDemon : AIController
    {

        [SerializeField] Projectile projectile = null;
        [SerializeField] GameObject AoEPrefab = null;
        [SerializeField] Transform projectileSpawner = null;
        [SerializeField] float baseProjDmg;
        [SerializeField] float projectileCooldown = 1f;
        [SerializeField] int maxAOECount = 10;
        [SerializeField] int minAOECount = 5;

        float timeSinceLastProjectile = Mathf.Infinity;

        protected override void Start() {
            guardPosition.ForceInit();
            InvokeRepeating("SpawnAOE", 0f, 5f);
        }

        protected override void AttackBehaviour()
        {
            timeSinceLastProjectile += Time.deltaTime;
            float roll = Random.value;

            fighter.Attack(player);

            
            AggrevateNearbyAllies();
        }

        protected override void Update()
        {
            if (health.IsDead()) 
            {
                CancelInvoke();
                return;
            }
            if (IsAggrevated() && fighter.CanAttack(player))
            {
                if (!aggrod)
                {
                    onAggro.Invoke();
                    aggrod = true;
                }
                timeSinceLastSawPlayer = 0;
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer <= suspicionTime)
            {
                //Suspicion 
                SuspicionBehaviour();
            }
            else
            {
                aggrod = false;
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        protected void LaunchProjectile(Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, projectileSpawner.position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }


        protected void SpawnAOE()
        {
            
            if (!IsAggrevated()) return;
            int count = Random.Range(minAOECount, maxAOECount);
            for (int i = 0; i < count; i++)
            {
                GameObject aoeObject = Instantiate(AoEPrefab, RandomNavmeshLocation(10f), Quaternion.identity);
                AoE aoe = aoeObject.GetComponent<AoE>();
                aoe.SetData(false, gameObject, 50);
            }
        }

        protected Vector3 RandomNavmeshLocation(float radius)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += transform.position;
            NavMeshHit hit;
            Vector3 finalPosition = transform.position;
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
            {
                finalPosition = hit.position;
            }
            return finalPosition;
        }
        
    }
}


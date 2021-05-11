using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using RPG.Utils;
using System;
using UnityEngine.Events;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] protected float chaseDistance = 5f;
        [SerializeField] protected float suspicionTime = 3f;
        [SerializeField] protected float aggroCooldownTime = 5f;
        [SerializeField] protected PatrolPath patrolPath;
        [SerializeField] protected float waypointTolerance = 1f;
        [SerializeField] protected float waypointDwellTime = 3f;
        [Range(0,1)]
        [SerializeField] protected float patrolSpeedFraction = 0.2f;
        [SerializeField] protected float shoutDistance = 5f;

        protected Fighter fighter;
        protected Health health;
        protected GameObject player;
        protected Mover mover;
        protected bool aggrod = false;
        [SerializeField] protected UnityEvent onAggro;

        protected LazyValue<Vector3> guardPosition;
        protected float timeSinceLastSawPlayer = Mathf.Infinity;
        protected float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        protected float timeSinceAggrevated = Mathf.Infinity;
        protected int currentWaypointIndex = 0;

        protected void Awake() {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            player = GameObject.FindWithTag("Player");
            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        protected Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        protected virtual void Start() 
        {
            guardPosition.ForceInit();
        }

        protected virtual void Update()
        {
            if (health.IsDead()) return;

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

        protected void Aggrevate()
        {
            
            timeSinceAggrevated = 0;
        }

        protected void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        protected void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition.value;

            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }

            if (timeSinceArrivedAtWaypoint >= waypointDwellTime)
            {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }

        }

        protected bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;

        }

        protected void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        protected Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        protected void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        protected virtual void AttackBehaviour()
        {
            fighter.Attack(player);
            AggrevateNearbyAllies();
        }

        protected void AggrevateNearbyAllies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);

            foreach (RaycastHit hit in hits)
            {
                AIController ai = hit.collider.GetComponent<AIController>();
                if (ai == null) continue;
                ai.Aggrevate();
            }
        }

        protected bool IsAggrevated()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position) ;
            return distanceToPlayer <= chaseDistance || timeSinceAggrevated <= aggroCooldownTime;
        }

        // Called by Unity
        protected void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}


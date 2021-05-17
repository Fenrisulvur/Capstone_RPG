using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class DirectionalProjectile : MonoBehaviour
    {
        [SerializeField] float speed = 1;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifetime = 10;
        [SerializeField] float lifeAfterImpact = 2;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] UnityEvent onHit;
        private bool targetPlayer;
        private bool penetrate = false;
        Vector3 aimPoint;
        Vector3 spawnPoint;
        float maxDist = 10f;

        float damage = 0;
        Health target = null;
        GameObject instigator = null;
        private void Start() {
            transform.LookAt(GetAimLocation());
        }

        private void Update() {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, spawnPoint) > maxDist)
            {
                Debug.Log("Too far - Distance:"+Vector3.Distance(transform.position, spawnPoint)+" / Max Distance: "+maxDist);
                Destroy(gameObject);
            }
                
        }

        public void SetTarget(GameObject instigator, Vector3 aimPos, bool isPlayer, float damage, float distance, bool penetrate)
        {
            this.spawnPoint = transform.position;
            this.maxDist = distance;
            this.penetrate = penetrate;
            this.aimPoint = aimPos;
            this.targetPlayer = !isPlayer;
            this.damage = damage;
            this.instigator = instigator;
            transform.LookAt(GetAimLocation());
            Destroy(gameObject, maxLifetime);
        }

        private void OnTriggerEnter(Collider other) {
            if (!other.GetComponent<Health>()) return;
            if (other.GetComponent<Health>() == instigator.GetComponent<Health>()) return;
            if(!targetPlayer && other.tag == "Player") return;
            if (other.GetComponent<Health>().IsDead()) return;

            other.GetComponent<Health>().TakeDamage(instigator, damage);

            

            onHit.Invoke();

            if (hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }
            if (!penetrate)
            {
                speed = 0;
                foreach (GameObject toDestroy in destroyOnHit)
                {
                    Destroy(toDestroy);
                }

                Destroy(gameObject, lifeAfterImpact);
            }
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = instigator.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
            {
                return aimPoint;
            }
            return aimPoint + Vector3.up * targetCapsule.height / 2;
        }
    }
}


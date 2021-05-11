using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;


namespace RPG.Combat
{
    public class AoE : MonoBehaviour
    {
        [SerializeField] float maxLifetime = 10;
        [SerializeField] float damageDelay = 1.2f;
        // [SerializeField] UnityEvent onHit;

        float damage = 0;
        bool playerCasted = false;
        GameObject instigator = null;


        public void SetData(bool playerCasted, GameObject instigator, float damage)
        {
            this.playerCasted = playerCasted;
            this.damage = damage;
            this.instigator = instigator;
            Invoke("Detect", damageDelay);
            Destroy(gameObject, maxLifetime);
        }

        private void Detect()
        {
            SphereCollider collider = gameObject.GetComponent<SphereCollider>();
            Collider[] collisions = Physics.OverlapSphere(collider.transform.position, collider.radius);
            foreach (Collider hit in collisions)
            {
                if (hit.gameObject.tag != "Player" && !playerCasted || playerCasted && hit.gameObject.tag == "Player") continue;
                hit.GetComponent<Health>().TakeDamage(gameObject, damage);
            }
        }

    

        // private void OnTriggerEnter(Collider other)
        // {
        //     if (other.GetComponent<Health>() != target) return;
        //     if (isHoming && target.IsDead()) return;
        //     target.TakeDamage(instigator, damage);

        //     speed = 0;

        //     onHit.Invoke();

        //     if (hitEffect != null)
        //     {
        //         Instantiate(hitEffect, GetAimLocation(), transform.rotation);
        //     }

        //     foreach (GameObject toDestroy in destroyOnHit)
        //     {
        //         Destroy(toDestroy);
        //     }

        //     Destroy(gameObject, lifeAfterImpact);
        // }

    }
}

using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using UnityEngine;


namespace RPG.Attributes
{
    public class Regen : MonoBehaviour
    {
        [SerializeField] float periodicHealAmount = 5.0f;
        [SerializeField] float periodicHealInterval = 1.0f;
        float timeSinceLastRestored=Mathf.Infinity;
        Fighter fighter;
        Health health;
        
        private void Awake() 
        {
            fighter = GetComponent<Fighter>();
            health  = GetComponent<Health>();
        }
        
        private void Update()
        {
            if(health.IsDead()) return;
            if (fighter.GetTarget()!=null) 
            {
                timeSinceLastRestored=0;
                return;
            }
            timeSinceLastRestored+=Time.deltaTime;
            if(timeSinceLastRestored>periodicHealInterval)
            {
                timeSinceLastRestored=0;
                health.Heal(periodicHealAmount);
            }
        }
    }  
}


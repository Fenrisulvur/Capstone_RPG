using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
    public class AggroGroup : MonoBehaviour
    {
        [SerializeField] Fighter[] fighters;
        [SerializeField] bool activateOnStart = false;

        private void Start() {
            Activate(activateOnStart);
        }   

        public void Activate(bool shouldActivate)
        {
            foreach (Fighter fighter in fighters)
            {
                CombatTarget target = fighter.GetComponent<CombatTarget>();
                Health health = fighter.GetComponent<Health>();
                if (target != null)
                {
                    target.enabled = shouldActivate;
                }
                if (health != null)
                {
                    health.enabled = shouldActivate;
                }
                fighter.enabled = shouldActivate;
            }
        }
    }
}

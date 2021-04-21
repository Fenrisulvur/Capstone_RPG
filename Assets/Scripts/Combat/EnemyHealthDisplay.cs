using System;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        
        Fighter fighter;

        private void Awake() {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        private void Update() {
            Health health = fighter.GetTarget();
            if (health != null)
            {
                GetComponent<Text>().text =  String.Format("{0:0.0}%", health.GetPercentage());  
            }
            else
            {
                GetComponent<Text>().text =  "N/A";
            }
        }
    }
}

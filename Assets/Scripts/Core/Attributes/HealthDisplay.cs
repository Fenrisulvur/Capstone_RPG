using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        Health health;

        private void Awake() {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        private void Update() {
            GetComponent<Text>().text =  String.Format("{0:0.0}% - ({1:0.0} / {2:0.0})", health.GetPercentage(), health.GetHealthValue(), health.GetHealthMaxValue());  
        }
    }
}

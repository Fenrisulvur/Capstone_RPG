using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RPG.Core
{
    public class CooldownManager : MonoBehaviour
    {
        Dictionary<string, float> timers;
        public event Action cooldownTickEvent;

        private void Awake() {
            timers = new Dictionary<string, float>();
        }

        private void Start() {
            InvokeRepeating("UpdateCooldowns",0.1f, 0.1f);
        }

        private void UpdateCooldowns() {
            foreach (var kvp in timers.ToArray())
            {
                if (timers[kvp.Key] == 0)
                {
                    timers.Remove(kvp.Key);
                    continue;
                }
                
                timers[kvp.Key] = Mathf.Max(0, timers[kvp.Key] - .1f);
            }
            cooldownTickEvent.Invoke();
        }

        public void AddCD(string id, float cd)
        {
            if (timers.ContainsKey(id))
            {
                timers[id] = cd;
                return;
            }
            
            timers.Add(id, cd);
        }

        public float GetCD(string id)
        {
            if (timers.ContainsKey(id))
                return timers[id];
            return 0;
        }

    }  
}



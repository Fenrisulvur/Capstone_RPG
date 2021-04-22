using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using System;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float regenerationPercentage = 70;
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {

        }

        LazyValue<float> healthPoints;
        bool isDead = false;

        private void Awake() {
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Start() 
        {
            healthPoints.ForceInit();
            //print(healthPoints.value+ " / " + GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        private void OnEnable() {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }

        private void OnDisable() {
            GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
        }

        private void RegenerateHealth()
        {
            float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (regenerationPercentage / 100);
            healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);
        }

        public float GetPercentage()
        {
            return 100 * GetFraction();
        }

        public float GetFraction()
        {
            return (healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        public float GetHealthValue()
        {
            return healthPoints.value;
        }
        public float GetHealthMaxValue()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public bool IsDead()
        {
            return isDead;
        }


        public void TakeDamage(GameObject instigator, float damage)
        {
            //print(gameObject.name + " took damage: " + damage);
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            takeDamage.Invoke(damage);
            if (healthPoints.value == 0)
            {
                onDie.Invoke();
                Die();
                AwardExperience(instigator);
            }
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;

            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        private void Die()
        {
            if (isDead) return;
            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
        
        public object CaptureState()
        {
            return healthPoints.value;
        }
        
        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;

            if (healthPoints.value == 0)
            {
                Die();
            }
        }
    }
}
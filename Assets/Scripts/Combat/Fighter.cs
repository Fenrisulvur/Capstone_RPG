using UnityEngine;
using RPG.Movement;
using RPG.Core;
using System;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider 
    {
        
        [SerializeField] float attackDelay = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform lefttHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;
    
        Health target;
        float timeSinceLastAttack = Mathf.Infinity;
        LazyValue<WeaponConfig> currentWeapon;

        private void Awake() {
            currentWeapon = new LazyValue<WeaponConfig>(setupDefaultWeapon);
        }

        private WeaponConfig setupDefaultWeapon()
        {
            AttachWeapon(defaultWeapon);
            return defaultWeapon;
        }

        private void Start() {
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;
            if (target.IsDead()) return;

            if (!GetIsInRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeapon.value = weapon;
            AttachWeapon(weapon);

        }

        private void AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(rightHandTransform, lefttHandTransform, animator);
        }

        public Health GetTarget()
        {
            return target;
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack >= attackDelay)
            {
                // this will trigger hit()
                TriggerAttack();
                timeSinceLastAttack = 0;

            }
        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        // Animation Event
        void Hit()
        {
            if (target == null) return;
            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            if (currentWeapon.value.HasProjectile())
            {
                currentWeapon.value.LaunchProjectile(rightHandTransform, lefttHandTransform, target, gameObject, damage);
            }
            else
            {
                target.TakeDamage(gameObject, damage);
            }
        }

        // Animation Event
        void Shoot()
        {
            Hit();
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(target.transform.position, transform.position) <= currentWeapon.value.GetRange();
        }

        public bool CanAttack(GameObject CombatTarget)
        {
            if (CombatTarget == null) return false;

            Health targetToTest = CombatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Attack(GameObject CombatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = CombatTarget.GetComponent<Health>();
            //print("Parry this, filthy casual!");
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
            GetComponent<Mover>().Cancel();
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetPercentageBonus();
            }
        }

        public object CaptureState()
        {
            return currentWeapon.value.name;
        }

        public void RestoreState(object state)
        {
            string weaponName= (String)state;
            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }

        
    }
}
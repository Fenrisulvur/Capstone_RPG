using UnityEngine;
using RPG.Movement;
using RPG.Core;
using System;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using RPG.Utils;
using RPG.Inventories;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable 
    {
        
        [SerializeField] float attackDelay = 1f;
        [SerializeField] float verbalPhraseDelay = 60f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform lefttHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;

        [SerializeField] UnityEvent onPhrase;

        Health target;
        Equipment equipment;
        float timeSinceLastAttack = Mathf.Infinity;
        float timeSinceLastPhrase = 0;
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;

        private void Awake() {
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(setupDefaultWeapon);
            equipment = GetComponent<Equipment>();
            if (equipment)
            {
                equipment.equipmentUpdated += UpdateItems;
            }
        }

        private Weapon setupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon); ;
        }

        private void Start() {
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;
            if (target.IsDead()) return;
            timeSinceLastPhrase += Time.deltaTime;

            if (timeSinceLastPhrase > verbalPhraseDelay)
            {
                onPhrase.Invoke();
                timeSinceLastPhrase = 0;
            }

            if (!GetIsInRange(target.transform))
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
                return;
            }

            
            GetComponent<Mover>().Cancel();
            AttackBehaviour();
            
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private void UpdateItems()
        {
            if (GetComponent<Health>().GetHealthValue() > GetComponent<Health>().GetHealthMaxValue()) GetComponent<Health>().StabilizeHealthOverflow();
            var weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            if (weapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
            else
            {
                EquipWeapon(weapon);
            }
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(rightHandTransform, lefttHandTransform, animator);
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

            if (currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            } 

            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, lefttHandTransform, target, gameObject, damage);
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


        void Draw()
        {
            if (currentWeapon.value != null)
            {
                currentWeapon.value.OnDraw();
            }
        }

        void Swing()
        {
            if (currentWeapon.value != null)
            {
                currentWeapon.value.OnSwing();
            }
        }

        void FootL()
        {
            
        }
        
        void FootR()
        {

        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(targetTransform.position, transform.position) <= currentWeaponConfig.GetRange();
        }

        public bool CanAttack(GameObject CombatTarget)
        {
            if (CombatTarget == null) return false;
            if (!GetComponent<Mover>().CanMoveTo(CombatTarget.transform.position) && !GetIsInRange(CombatTarget.transform)) return false;

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

        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName= (String)state;
            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }

        
    }
}
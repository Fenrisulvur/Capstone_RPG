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

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable 
    {
        
        [SerializeField] float attackDelay = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform lefttHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;
    
        Health target;
        Equipment equipment;
        float timeSinceLastAttack = Mathf.Infinity;
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;

        private void Awake() {
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(setupDefaultWeapon);
            equipment = GetComponent<Equipment>();
            if (equipment)
            {
                equipment.equipmentUpdated += UpdateWeapon;
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

            if (!GetIsInRange(target.transform))
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
                return;
            }
            // if( GetIsInRange(target.transform) && !IsLOS(target.gameObject))
            // {
            //     GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            //     return;
            // }
            
            GetComponent<Mover>().Cancel();
            AttackBehaviour();
            
        }

        private bool IsLOS(GameObject target)
        {
            float distance = 1000.0f; // how far they can see the target
            float arc = 45.0f; // their field of view
            float heightOffet = 1.7f;

            if (Vector3.Distance(transform.position, target.transform.position) < distance)
            {
                if (Vector3.Distance(transform.position, target.transform.position) < 4 ) return true;
                // enemy is within distance

                if (Vector3.Dot(transform.forward, target.transform.position) > 0 && Vector3.Angle(transform.forward, target.transform.position) < arc)
                {
                    // enemy is ahead of me and in my field of view
                    RaycastHit hitInfo;

                    Debug.DrawRay(transform.position + new Vector3(0, heightOffet, 0), ((target.transform.position + new Vector3(0, heightOffet, 0)) - (transform.position + new Vector3(0, heightOffet, 0))) * 1000, Color.yellow);
                    if (Physics.SphereCast(transform.position + new Vector3(0, heightOffet, 0), .2f, (target.transform.position + new Vector3(0, heightOffet, 0)) - ( transform.position + new Vector3(0, heightOffet, 0) ), out hitInfo) == true)
                    {
                        
                        // we hit SOMETHING, not necessarily a player
                        if (hitInfo.collider.gameObject.tag == "Player" || hitInfo.collider.gameObject.tag == "Enemy")
                            return true;
                    }
                }
            }

            return false;

        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);

        }

        private void UpdateWeapon()
        {
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
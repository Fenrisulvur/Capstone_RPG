using RPG.Attributes;
using UnityEngine;
using RPG.Inventories;
using RPG.Stats;
using System.Collections.Generic;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : EquipableItem, IModifierProvider
    {
        
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] Weapon equippedPrefab = null;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] float percentageBonus = 0f;
        [SerializeField] float NPCScale = 1f;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] bool isTwoHanded = false;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand, weaponName);
            Weapon weapon = null;
            
            if (equippedPrefab != null)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                weapon = Instantiate(equippedPrefab, handTransform);
                weapon.gameObject.name = weaponName;
                if (animator.gameObject.tag != "Player")
                    weapon.transform.localScale = new Vector3(NPCScale,NPCScale,NPCScale);
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            if (weapon == null)
            {
                Debug.LogError("Spawned weapon is null");
            }
            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand, string name)
        {
            Transform oldWeapon = rightHand.Find(name);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(name);
            }
            if (oldWeapon == null) return;
            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }
        


        private Transform GetTransform(Transform rightHandTransform, Transform leftHandTransform)
        {
            return (isRightHanded ? rightHandTransform : leftHandTransform);
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public bool IsTwoHanded()
        {
            return isTwoHanded;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator,  calculatedDamage);
        }

        public float GetDamage()
        {
            return weaponDamage;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }

        public float GetRange()
        {
            return weaponRange;
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return weaponDamage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return percentageBonus;
            }
        }
    }
}
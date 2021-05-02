﻿using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Utils;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1,99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticleEffect = null;
        [SerializeField] bool shouldUseModifiers = false;

        public event Action onLevelUp;

        LazyValue<int> currentLevel;

        Experience experience;

        private void Awake() {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start() {
            currentLevel.ForceInit();
        }

        private void OnEnable() {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable() {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        private void UpdateLevel() {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                LevelUpEffect();
                onLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifiers(stat)) * (1 + GetPercentageModifier(stat)/100);
        }


        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        public int GetLevel()
        {
            return currentLevel.value;
        }

        public float GetLevelXpReq(int level)
        {
            if (level == 0) return 0;
            Experience experience = GetComponent<Experience>();
            if (experience == null) return 0;

            float currentXP = experience.GetExperience();
            int maxLevel = progression.GetLevels(Stat.ExperienceToLevel, characterClass);
            if (level >= maxLevel - 1) return currentXP;

            return progression.GetStat(Stat.ExperienceToLevel, characterClass, level);
        }

        private float GetAdditiveModifiers(Stat stat)
        {
            if (!shouldUseModifiers) return 0;
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }
        
        private int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();
            if (experience == null) return startingLevel;

            float currentXP = experience.GetExperience();
            int maxLevel = progression.GetLevels(Stat.ExperienceToLevel, characterClass);

            for (int level = 1; level < maxLevel; level++)
            {
               float xpToLevelUp = progression.GetStat(Stat.ExperienceToLevel, characterClass, level);
               if (xpToLevelUp > currentXP) 
               {
                   return level;
               }
            }

            return maxLevel;
        }


    }
}

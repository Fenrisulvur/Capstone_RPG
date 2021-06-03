using System;
using RPG.Saving;
using UnityEngine;

namespace RPG.Inventories
{
    public class Purse : MonoBehaviour, ISaveable
    {
        [SerializeField] int startingBalance = 400;

        int balance = 0;

        public event Action onChange;
        
        private void Awake() {
            balance = startingBalance;
        }

        public int GetBalance()
        {
            return balance;
        }

        public void UpdateBalance(int amount)
        {
            balance += amount;
            if (onChange != null)
            {
                onChange();
            }
        }

        public object CaptureState()
        {
            return balance;
        }

        public void RestoreState(object state)
        {
            balance = (int)state;
        }
    }
}

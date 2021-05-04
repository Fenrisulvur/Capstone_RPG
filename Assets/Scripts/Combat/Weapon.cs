using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] UnityEvent onHit;
        [SerializeField] UnityEvent onDraw;
        [SerializeField] UnityEvent onSwing;

        internal void OnHit()
        {
            onHit.Invoke();
        }

        internal void OnDraw()
        {
            onDraw.Invoke();
        }

        internal void OnSwing()
        {
            onSwing.Invoke();
        }
        
    }
}


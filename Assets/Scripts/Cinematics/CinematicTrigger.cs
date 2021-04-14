using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        private bool played = false;

        private void OnTriggerEnter(Collider other) {
            if (played || other.gameObject.tag != "Player") return;
            played = !played;
            GetComponent<PlayableDirector>().Play();
        }
    }
}


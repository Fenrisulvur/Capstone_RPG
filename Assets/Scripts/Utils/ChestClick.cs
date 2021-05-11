using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.Inventories;
using RPG.Saving;
using UnityEngine;

namespace RPG.Interactables
{
    public class ChestClick : MonoBehaviour, IRaycastable, ISaveable
    {
        [SerializeField] Transform hinge = null;
        [SerializeField] float closedRot = 0;
        [SerializeField] float openRot = 90;
        [SerializeField] Transform overridePosition = null;

        bool opened = false;

        private IEnumerator open()
        {
            opened = true;
            for (int i = 0; i < openRot; i++)
            {
                hinge.transform.localRotation = Quaternion.Euler(-i, 0, openRot);
                yield return new WaitForSeconds(0);
            }
            yield return new WaitForSeconds(0);
        }

        public CursorType GetCursorType()
        {
            if (opened)
            {
                return CursorType.FullPickup;
            }
            return CursorType.Interactable;
    
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0) && !opened)
            {
                StartCoroutine(open());
                if (overridePosition)
                {
                    GetComponent<RandomDropper>().RandomDrop(1, overridePosition.position );
                }
                //pickup.PickupItem();position + new Vector3(Random.Range(-.2f, .2f), 0, Random.Range(-.2f, .2f))
            }
            return true;
        }

        public object CaptureState()
        {
            return opened;
        }

        public void RestoreState(object state)
        {
            opened = (bool)state;
            if (opened)
                StartCoroutine(open());
        }
    }
}
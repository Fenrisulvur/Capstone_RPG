using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Inventories;
using RPG.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Consumables
{
    [CreateAssetMenu(menuName = ("RPG/InventorySystem/Consumables/Directional Test Spell"))]
    public class TestDirectionalSpell : ActionItem
    {
        [SerializeField] float dmg = 50f;
        [SerializeField] DirectionalProjectile projectile = null;
        [SerializeField] float maxRange = 20f;
        private Transform targeter;
        private bool aiming = false;
        private GameObject curUser;
        private Vector3 position;

        Task t;


        public override void Use(GameObject user)
        {
            //if (aiming) return;
            Debug.Log("Spell Started");
            curUser = user;
            targeter = curUser.transform.Find("Skillshot");
            //targeter.Find("Image").GetComponent<Image>().transform.localScale = new Vector3(2, maxRange/2, 1);
            targeter.Find("Cube").transform.localScale = new Vector3(10, maxRange, 100);
            targeter.Find("Cube").transform.localPosition = new Vector3(0, maxRange/2, 0);
            aiming = true;
            t = new Task(Test());
            t.Finished+= delegate (bool manual)
            {
                targeter.gameObject.SetActive(false);
                if (manual)
                    Debug.Log("Spell cancelled");
                else
                    InitiateCooldown(curUser);
            };
        }

        private IEnumerator Test()
        {

            targeter.gameObject.SetActive(true);
            while (aiming)
            {   
                Debug.Log("Test");
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                }



                Quaternion transRot = Quaternion.LookRotation(position - curUser.transform.position);
                transRot.eulerAngles = new Vector3(90, transRot.eulerAngles.y, 0);
                targeter.rotation = Quaternion.Lerp(transRot, targeter.rotation, 0f);

                if (Input.GetMouseButton(0))
                {
                    Debug.Log("Firing Spell");
                    aiming = false;
                    SpawnSpell(position + new Vector3(0, .1f, 0));
                }
                if (Input.GetMouseButton(1))
                {
                    Debug.Log("Cancelling Spell");
                    aiming = false;
                    t.Stop();
                }
                yield return new WaitForSeconds(0.01f);
            }

        }

        protected void SpawnSpell(Vector3 pos)
        {
            DirectionalProjectile projectileInstance = Instantiate(projectile, curUser.transform.position+Vector3.up, Quaternion.identity);
            projectileInstance.SetTarget(curUser, pos, true, dmg, maxRange, true);
        }
        
    }

}
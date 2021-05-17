using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Inventories;
using RPG.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Consumables
{
    [CreateAssetMenu(menuName = ("RPG/InventorySystem/Consumables/AOE Test Spell"))]
    public class AOETestSpell : ActionItem
    {
        [SerializeField] float dmg = 50f;
        [SerializeField] GameObject AoEPrefab = null;
        [SerializeField] float maxRange = 10f;
        private Transform targeter;
        private Transform targeterAimer;
        private Transform rangefinder;
        private Image rangeImg;
        private bool aiming = false;
        private GameObject curUser;
        private Vector3 position;
        private Vector3 posUp;

        Task t;


        public override void Use(GameObject user)
        {
            
                
            curUser = user;
            targeter = curUser.transform.Find("CircleCanvas");
            targeterAimer = targeter.Find("Cylinder");
            rangefinder = curUser.transform.Find("RangeIndicator2");
            rangefinder.Find("Cylinder").transform.localScale = new Vector3(maxRange, maxRange, 100);
            
            aiming = true;
            t = new Task(Test());
            t.Finished+= delegate (bool manual)
            {
                targeter.gameObject.SetActive(false);
                rangefinder.gameObject.SetActive(false);
                if (manual)
                    Debug.Log("Spell cancelled");
                else
                    InitiateCooldown(curUser);
            };
        }

        private IEnumerator Test()
        {

            targeter.gameObject.SetActive(true);
            rangefinder.gameObject.SetActive(true);
            while (aiming)
            {   
                Debug.Log("Test");
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (hit.collider.gameObject != targeter.gameObject)
                    {
                        posUp = new Vector3(hit.point.x, 10f, hit.point.z);
                        position = hit.point;
                    }
                }

                
                var hitPosDir = (hit.point - curUser.transform.position).normalized;
                float distance = Vector3.Distance(hit.point, curUser.transform.position);
                distance = Mathf.Min(distance, maxRange);

                var newHitPos = curUser.transform.position + hitPosDir * distance;
                targeterAimer.position = newHitPos;

                Debug.Log("Distance= "+distance +" Max Dist = "+maxRange +" Hit Pos =" + position+" Aim Pos =" + targeterAimer.position);
                if (Input.GetMouseButton(0))
                {
                    Debug.Log("Firing AoE");
                    aiming = false;
                    SpawnAOE(targeterAimer.position + new Vector3(0, .1f, 0));
                }
                if (Input.GetMouseButton(1))
                {
                    Debug.Log("Cancelling AoE");
                    aiming = false;
                    t.Stop();
                }
                yield return new WaitForSeconds(0.01f);
            }

        }

        protected void SpawnAOE(Vector3 pos)
        {
            GameObject aoeObject = Instantiate(AoEPrefab, pos, Quaternion.identity);
            AoE aoe = aoeObject.GetComponent<AoE>();
            aoe.SetData(true, curUser, 50);
        }
        
    }

}
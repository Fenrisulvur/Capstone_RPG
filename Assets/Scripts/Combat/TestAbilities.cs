using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class TestAbilities : MonoBehaviour
    {
        [Header("Ability 1")]
        public Image abilityImage1;
        public float cooldown1 = 5;
        bool isCooldown = false;
        public KeyCode ability1;

        //Ability 1 Input Variables
        Vector3 position;
        public Canvas ability1Canvas;
        public Image skillshot;
        public GameObject skillshotObj;
        public Transform player;

        [Header("Ability 2")]
        public Image abilityImage2;
        public float cooldown2 = 10;
        bool isCooldown2 = false;
        public KeyCode ability2;

        //Ability 2 Input Variables
        public Image targetCircle;
        public GameObject targetCircleObj;
        public Image indicatorRangeCircle;
        public GameObject rangeCircleObj;
        public Canvas ability2Canvas;
        private Vector3 posUp;
        public float maxAbility2Distance;

        [Header("Ability 3")]
        public Image abilityImage3;
        public float cooldown3 = 7;
        bool isCooldown3 = false;
        public KeyCode ability3;



        // Start is called before the first frame update
        void Start()
        {
            // abilityImage1.fillAmount = 0;
            // abilityImage2.fillAmount = 0;
            // abilityImage3.fillAmount = 0;

            skillshot.GetComponent<Image>().enabled = false;
            targetCircle.GetComponent<Image>().enabled = false;
            skillshotObj.SetActive(false);
            targetCircleObj.SetActive(false);
            rangeCircleObj.SetActive(false);
            indicatorRangeCircle.GetComponent<Image>().enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            Ability1();
            Ability2();
            Ability3();

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //Ability 1 Inputs
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }

            //Ability 2 Inputs
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject != this.gameObject)
                {
                    posUp = new Vector3(hit.point.x, 10f, hit.point.z);
                    position = hit.point;
                }
            }


            //Ability 1 Canvas Inputs
            Quaternion transRot = Quaternion.LookRotation(position - player.transform.position);
            transRot.eulerAngles = new Vector3(90, transRot.eulerAngles.y, 0);
            ability1Canvas.transform.rotation = Quaternion.Lerp(transRot, ability1Canvas.transform.rotation, 0f);

            //Ability 2 Canvas Inputs
            var hitPosDir = (hit.point - transform.position).normalized;
            float distance = Vector3.Distance(hit.point, transform.position);
            distance = Mathf.Min(distance, maxAbility2Distance);

            var newHitPos = transform.position + hitPosDir * distance;
            ability2Canvas.transform.position = (newHitPos);

        }

        void Ability1()
        {
            if (Input.GetKey(ability1) && isCooldown == false)
            {
                skillshot.GetComponent<Image>().enabled = true;
                skillshotObj.SetActive(true);
            }

            if (skillshot.GetComponent<Image>().enabled == true && Input.GetMouseButtonDown(0))
            {
                isCooldown = true;
                abilityImage1.fillAmount = 1;
            }

            if (isCooldown)
            {
                abilityImage1.fillAmount -= 1 / cooldown1 * Time.deltaTime;
                skillshot.GetComponent<Image>().enabled = false;
                skillshotObj.SetActive(false);
                if (abilityImage1.fillAmount <= 0)
                {
                    abilityImage1.fillAmount = 0;
                    isCooldown = false;
                }
            }
        }

        void Ability2()
        {
            if (Input.GetKey(ability2) && isCooldown2 == false)
            {
                indicatorRangeCircle.GetComponent<Image>().enabled = true;
                targetCircle.GetComponent<Image>().enabled = true;

                //Disable Skillshot UI
                skillshot.GetComponent<Image>().enabled = false;
                skillshotObj.SetActive(false);
                targetCircleObj.SetActive(true);
                rangeCircleObj.SetActive(true);
            }

            if (targetCircle.GetComponent<Image>().enabled == true && Input.GetMouseButtonDown(0))
            {
                isCooldown2 = true;
                abilityImage2.fillAmount = 1;
            }

            if (isCooldown2)
            {
                abilityImage2.fillAmount -= 1 / cooldown2 * Time.deltaTime;

                indicatorRangeCircle.GetComponent<Image>().enabled = false;
                targetCircle.GetComponent<Image>().enabled = false;
                targetCircleObj.SetActive(false);
                rangeCircleObj.SetActive(false);

                if (abilityImage2.fillAmount <= 0)
                {
                    abilityImage2.fillAmount = 0;
                    isCooldown2 = false;
                }
            }
        }

        void Ability3()
        {

        }


    }
}


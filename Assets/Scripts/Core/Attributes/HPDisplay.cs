using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HPDisplay : MonoBehaviour
    {
        [SerializeField] Health healthComponent = null;
        [SerializeField] RectTransform foreground = null;
        [SerializeField] TextMeshProUGUI text = null;

        private void Awake()
        {
            healthComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        }

        void Update()
        {
            text.SetText(healthComponent.GetHealthValue()+"/"+healthComponent.GetHealthMaxValue());
            foreground.localScale = new Vector3(healthComponent.GetFraction(), 1, 1);
        }
    }
}

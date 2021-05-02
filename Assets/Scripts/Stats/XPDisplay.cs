using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class XPDisplay : MonoBehaviour
    {
        BaseStats stats = null;
        Experience experience = null;

        [SerializeField] RectTransform foreground = null;
        [SerializeField] TextMeshProUGUI lvlDisplay = null;
        [SerializeField] TextMeshProUGUI xpDisplay = null;

        private void Awake()
        {
            stats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
            experience.onExperienceGained += RedrawXPBar;

        }

        private void RedrawXPBar()
        {
            xpDisplay.SetText(String.Format("{0:0.0}/{1:0.0}", experience.GetExperience(), stats.GetNextLevelXpReq()));
            lvlDisplay.SetText("Lvl: "+stats.GetLevel());
            foreground.localScale = new Vector3((experience.GetExperience()/stats.GetNextLevelXpReq()), 1, 1);
        }


        private void Start() {
            RedrawXPBar();
        }

    }
}

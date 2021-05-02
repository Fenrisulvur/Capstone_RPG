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
            stats.onLevelUp += RedrawXPBar;
        }

        private void RedrawXPBar()
        {
            int lvl = stats.GetLevel();
            xpDisplay.SetText(String.Format("{0:0.0}/{1:0.0}", experience.GetExperience()-stats.GetLevelXpReq(lvl - 1), stats.GetLevelXpReq(lvl)-stats.GetLevelXpReq(lvl-1)));
            lvlDisplay.SetText("Lvl: "+stats.GetLevel());
            foreground.localScale = new Vector3( Mathf.Clamp( ( (experience.GetExperience()-stats.GetLevelXpReq(lvl - 1)) / (stats.GetLevelXpReq(lvl)-stats.GetLevelXpReq(lvl - 1) )), 0, 1)  , 1, 1);
        }


        private void Start() {
            RedrawXPBar();
        }

    }
}

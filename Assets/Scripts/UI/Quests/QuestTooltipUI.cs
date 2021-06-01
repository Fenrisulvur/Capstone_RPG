using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Quests;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] Transform objectiveContainer;
        [SerializeField] GameObject objectivePrefab;
        [SerializeField] GameObject objectiveIncompletePrefab;
        [SerializeField] TextMeshProUGUI rewardText;

        public void Setup(QuestStatus status)
        {
            Quest quest = status.GetQuest();
            title.text = quest.GetTitle();
            foreach (Transform item in objectiveContainer)
            {
                Destroy(item.gameObject);
            }
            foreach (var objective in quest.GetObjectives())
            {
                GameObject prefab = objectiveIncompletePrefab;
                FontStyles style = FontStyles.Normal;
                if (status.IsObjectiveComplete(objective.reference))
                {
                    prefab = objectivePrefab;
                    style = FontStyles.Strikethrough;
                }

                GameObject objectiveInstance = Instantiate(prefab, objectiveContainer);
                TextMeshProUGUI objectiveText = objectiveInstance.GetComponentInChildren<TextMeshProUGUI>();

                objectiveText.text = objective.description;
                objectiveText.fontStyle = style;
            }
            rewardText.text = GetRewardText(quest);
        }

        private string GetRewardText(Quest quest)
        {
            string rewardText = "";
            foreach (var reward in quest.GetRewards())
            {
                if (rewardText != "")
                {
                    rewardText += ", ";
                }
                if (reward.number > 1)
                {
                    rewardText += reward.number+"x ";
                }
                rewardText += reward.item.GetDisplayName();
            }
            if (rewardText == "")
            {
                rewardText = "No Reward";
            }
            rewardText += ".";

            return rewardText;
        }
    }
}

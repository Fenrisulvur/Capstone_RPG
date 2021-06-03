using System.Collections;
using System.Collections.Generic;
using RPG.Quests;
using TMPro;
using UnityEngine;

public class QuestItemUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI progress;

    QuestStatus status;

    public void Setup(QuestStatus status)
    {
        this.status = status;
        title.text = status.GetQuest().GetTitle();
        progress.text = Mathf.Min(status.GetCompletedCount(), status.GetQuest().GetObjectiveCount()) +"/"+status.GetQuest().GetObjectiveCount();
        if (status.IsComplete())
        {
            title.fontStyle = FontStyles.Strikethrough;
        }
    }

    public QuestStatus GetQuestStatus()
    {
        return status;
    }
}

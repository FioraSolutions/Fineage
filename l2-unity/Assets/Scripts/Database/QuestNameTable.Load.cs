using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class QuestNameTable
{
    public QuestName GetQuestName(int questId, int questProg)
    {
        if (_questNames.ContainsKey(questId))
        {
            foreach (var quest in _questNames[questId])
            {
                if (quest.QuestProg == questProg)
                {
                    return quest;
                }
            }
        }
        return null;
    }

    public List<QuestName> GetQuestsWithLastSubtask()
    {
        List<QuestName> quests = new List<QuestName>();

        foreach (var entry in _questNames)
        {
            QuestName lastQuest = entry.Value[entry.Value.Count - 1];
            quests.Add(lastQuest);
        }

        return quests;
    }

    public void Initialize()
    {
        try
        {
            string dataPathE = Path.Combine(Application.streamingAssetsPath, "Data/Meta/Questname-e_interlude.txt");
            //string dataPathR = Path.Combine(Application.streamingAssetsPath, "Data/Meta/Questname-r_interlude.txt");
            ReadQuestNameInterlude(dataPathE);
            //ReadArmorInterlude(dataPathR);
            //DebugPrint();
        }
        catch (Exception)
        {
            // Debug.LogError($"Failed to initialize QuestNameTable: {ex.Message}");
        }
    }
}

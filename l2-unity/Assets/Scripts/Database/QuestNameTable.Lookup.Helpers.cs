using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class QuestNameTable
{
    private QuestName CreateModel(string[] ids , int quest_id)
    {
        QuestName quest = new QuestName();

        int questProg = int.Parse(ids[indexQuestProg]);
        string mainName = DatUtils.CleanupStringOldData(ids[indexMainName]);
        string progName = DatUtils.CleanupStringOldData(ids[indexProgname]);
        string description = DatUtils.CleanupStringOldData(ids[indexDescription]);
        string descriptionToolTips = DatUtils.CleanupStringOldData(ids[indexDescriptionToolTips]);
        int questType = int.Parse(ids[indexQuestType]);
        int lvlMin = int.Parse(ids[indexLvlMin]);
        int lvlMax = int.Parse(ids[indexLvlMax]);
        string requirements = DatUtils.CleanupStringOldData(ids[indexRequirements]);
        string entity_name = DatUtils.CleanupStringOldData(ids[indexEntity_name]);
        int repeat = int.Parse(ids[indexRepaet]);
        int areaId = int.Parse(ids[indexAreaId]);

        quest.Quest_id = quest_id;
        quest.QuestProg = questProg;
        quest.Main_name = mainName;
        quest.ProgName = progName;
        quest.Description = DatUtils.CleanupStringNewN(description);
        quest.DescriptionToolTip = descriptionToolTips;
        quest.QuestType = questType;
        quest.LvlMin = lvlMin;
        quest.LvlMax = lvlMax;
        quest.Requirements = requirements;
        quest.EntityName = entity_name;
        quest.Repeat = repeat;
        quest.AreaId = areaId;

        return quest;
    }

    private void DebugPrint()
    {
        foreach (KeyValuePair<int, List<QuestName>> entry in _questNames)
        {
            foreach(QuestName name in entry.Value)
            {
                //EntityName берем последнего из списка он и будет Клиентом в форме или источником
                // Debug.Log(" ID Номер quest " + entry.Key + " value " + name.Main_name + " entityName " + name.EntityName + " questType " + name.Repeat);
            }
            
        }
    }
}

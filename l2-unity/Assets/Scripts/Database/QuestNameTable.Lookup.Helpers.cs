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
        if (ids.Length > indexQuestWorldX && float.TryParse(ids[indexQuestWorldX], out float qx))
            quest.QuestWorldX = (int)qx;
        if (ids.Length > indexQuestWorldY && float.TryParse(ids[indexQuestWorldY], out float qy))
            quest.QuestWorldY = (int)qy;
        if (ids.Length > indexQuestWorldZ && float.TryParse(ids[indexQuestWorldZ], out float qz))
            quest.QuestWorldZ = (int)qz;
        if (ids.Length > indexContactNpcId && int.TryParse(ids[indexContactNpcId], out int contactNpc))
            quest.ContactNpcId = contactNpc;
        if (ids.Length > indexContactNpcX && float.TryParse(ids[indexContactNpcX], out float cx))
            quest.ContactNpcX = (int)cx;
        if (ids.Length > indexContactNpcY && float.TryParse(ids[indexContactNpcY], out float cy))
            quest.ContactNpcY = (int)cy;
        if (ids.Length > indexContactNpcZ && float.TryParse(ids[indexContactNpcZ], out float cz))
            quest.ContactNpcZ = (int)cz;
        for (int slot = 0; slot < 14; slot++)
        {
            int itemCol = indexItem0 + slot;
            int countCol = indexNumItem0 + slot;
            if (ids.Length <= countCol)
                break;
            if (!int.TryParse(ids[itemCol], out int itemId) || itemId <= 0)
                continue;
            if (!int.TryParse(ids[countCol], out int need) || need <= 0)
                continue;
            quest.AddRequiredItem(itemId, need);
        }

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
        if (ids.Length > indexClanPetQuest && int.TryParse(ids[indexClanPetQuest], out int clanPet))
            quest.ClanPetQuest = clanPet;

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class QuestWindow
{
    public List<QuestInstance> Journal()
    {
        var all = new List<QuestInstance>();
        void AddRange(List<QuestInstance> rows)
        {
            if (rows == null)
                return;
            for (int i = 0; i < rows.Count; i++)
            {
                QuestInstance row = rows[i];
                if (row == null || all.Exists(q => q.QuestID == row.QuestID))
                    continue;
                all.Add(row);
            }
        }
        AddRange(_tabSingle);
        AddRange(_tabRepeat);
        AddRange(_tabEpic);
        AddRange(_tabTransfer);
        AddRange(_tabSpecial);
        return all;
    }

    public void AddData(List<QuestInstance> quests)
    {
        //if (_questsList != null) _questsList.Clear();

        repeatQuests = quests ?? new List<QuestInstance>();
        nonRepeatQuests = quests ?? new List<QuestInstance>();
        PartitionActiveQuests(quests ?? new List<QuestInstance>());
        RefreshCompletedTab();
        HudCorePacketServices.CharacterInfo?.FillQuestPanel(Journal());

        _questTabPanelSingle.AddElementsToContent(_tabSingle);
        _questTabPanelRepeat.AddElementsToContent(_tabRepeat);
        _questTabPanelEpic.AddElementsToContent(_tabEpic);
        _questTabPanelTransfer.AddElementsToContent(_tabTransfer);
        _questTabPanelSpecial.AddElementsToContent(_tabSpecial);
        _questTabPanelCompleted.AddElementsToContent(_tabCompleted);
        if (!IsSuccess())
        {
            HideDetailedInfo();
        } 
    }

    private void ToggleDetailedInfo(List<QuestInstance> list)
    {
        if (list == null || list.Count == 0)
        {
            _detailedInfoElement.style.display = DisplayStyle.None;
        }
    }

}

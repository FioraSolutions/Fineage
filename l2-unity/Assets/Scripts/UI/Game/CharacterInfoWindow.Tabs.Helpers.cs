using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine;

public partial class CharacterInfoWindow
{
    void OpenClanTab()
    {
        SelectMainTab(4, updateTitle: true);
        ShowMainPanel(4);
        RefreshClanTab();
    }

    void OpenQuestTab()
    {
        SelectMainTab(5, updateTitle: true);
        ShowMainPanel(5);
        QuestWindow quest = QuestPacketServices.Quest;
        if (quest != null)
            FillQuestPanel(quest.Journal());
        if (ZoneCommands.IsConnected)
            ZoneCommands.Send(new RequestQuestListCommand());
    }

    void ShowMainPanel(int index)
    {
        _mainTab = index;
        if (_detailStatusWnd != null)
            _detailStatusWnd.style.display = index == 1 ? DisplayStyle.Flex : DisplayStyle.None;
        if (_skillsHost != null)
            _skillsHost.style.display = index == 2 ? DisplayStyle.Flex : DisplayStyle.None;
        if (_actionsHost != null)
            _actionsHost.style.display = index == 3 ? DisplayStyle.Flex : DisplayStyle.None;
        if (_clanHost != null)
            _clanHost.style.display = index == 4 ? DisplayStyle.Flex : DisplayStyle.None;
        if (_questHost != null)
            _questHost.style.display = index == 5 ? DisplayStyle.Flex : DisplayStyle.None;
    }

    void SelectMainTab(int index, bool updateTitle)
    {
        for (int i = 1; i <= 5; i++)
        {
            bool on = i == index;
            string btnPath = on
                ? $"Data/UI/Status/Interlude/Status_Interlude_MainWndTabBtn{i}On"
                : $"Data/UI/Status/Interlude/Status_Interlude_MainWndTabBtn{i}";
            Paint($"MainWndTab{i}", btnPath);
            Button tab = _windowEle?.Q<Button>($"MainWndTab{i}");
            if (tab == null)
                continue;
            if (on)
                tab.AddToClassList("status-mainwnd-tab-active");
            else
                tab.RemoveFromClassList("status-mainwnd-tab-active");
        }

        if (!updateTitle || _windowTitleLabel == null)
            return;
        _windowTitleLabel.text = index switch
        {
            2 => "Skills",
            3 => "Actions",
            4 => "Clan",
            5 => "Quest",
            _ => "Status"
        };
    }

    public void FillQuestPanel(IReadOnlyList<QuestInstance> quests)
    {
        if (_questList == null)
            return;
        _questList.Clear();
        if (quests == null || quests.Count == 0)
        {
            var empty = new Label("No quests in progress.");
            empty.style.fontSize = 11;
            empty.style.color = new Color(149f / 255f, 149f / 255f, 149f / 255f);
            empty.style.marginLeft = 8;
            empty.style.marginTop = 6;
            _questList.Add(empty);
            return;
        }

        for (int i = 0; i < quests.Count; i++)
        {
            QuestInstance quest = quests[i];
            string name = quest.QuestName();
            if (string.IsNullOrEmpty(name))
                name = "Quest " + quest.QuestID;
            string line = string.IsNullOrEmpty(quest.QuestProgName()) ? name : $"{name} — {quest.QuestProgName()}";
            var row = new Label(line);
            row.style.fontSize = 11;
            row.style.color = new Color(223f / 255f, 222f / 255f, 222f / 255f);
            row.style.height = 16;
            row.style.whiteSpace = WhiteSpace.Normal;
            row.style.marginLeft = 8;
            _questList.Add(row);
        }

        if (QuestJournalState.Completed.Count > 0)
        {
            var doneHeader = new Label("Completed");
            doneHeader.style.fontSize = 10;
            doneHeader.style.color = new Color(0.6f, 0.85f, 0.55f);
            doneHeader.style.marginTop = 8;
            doneHeader.style.marginLeft = 8;
            _questList.Add(doneHeader);
            foreach (int questId in QuestJournalState.Completed)
            {
                QuestName meta = QuestJournalResolver.ResolveCompletedTitle(questId);
                string title = meta?.Main_name;
                if (string.IsNullOrEmpty(title))
                    title = "Quest " + questId;
                var row = new Label(title);
                row.style.fontSize = 11;
                row.style.color = new Color(0.7f, 0.7f, 0.7f);
                row.style.marginLeft = 8;
                _questList.Add(row);
            }
        }
    }
}

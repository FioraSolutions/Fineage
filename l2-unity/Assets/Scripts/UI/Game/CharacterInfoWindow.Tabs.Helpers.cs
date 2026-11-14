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
        FillClanPanel();
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

    void FillClanPanel()
    {
        NetClanBook book = NetClanBook.Shared;
        SetClanText("ClanNameValue", book.HasClan ? book.Name : "No Clan");
        SetClanText("ClanLeaderValue", book.HasClan ? book.LeaderName() : "-");
        SetClanText("ClanHallValue", "None");
        SetClanText("ClanStatusValue", !book.HasClan ? "-" : book.Dissolving ? "Dissolving" : "At Peace");
        SetClanText("ClanLevelValue", book.HasClan ? book.Level.ToString() : "-");
        int count = book.Members != null ? book.Members.Length : 0;
        SetClanText("ClanMembersValue", book.HasClan ? count.ToString() : "-");
        if (_clanMemberList == null)
            return;
        _clanMemberList.Clear();
        if (!book.HasClan || book.Members == null)
            return;
        for (int i = 0; i < book.Members.Length; i++)
        {
            NetPledgeMember member = book.Members[i];
            var row = new Label(member.Name + "   " + member.Level);
            row.style.fontSize = 11;
            row.style.color = member.Online
                ? new Color(223f / 255f, 222f / 255f, 222f / 255f)
                : new Color(149f / 255f, 149f / 255f, 149f / 255f);
            row.style.height = 16;
            row.style.unityFontStyleAndWeight = FontStyle.Normal;
            _clanMemberList.Add(row);
        }
    }

    void SetClanText(string name, string value)
    {
        Label label = _windowEle?.Q<Label>(name);
        if (label != null)
            label.text = value ?? "";
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
            var row = new Label(name);
            row.style.fontSize = 11;
            row.style.color = new Color(223f / 255f, 222f / 255f, 222f / 255f);
            row.style.height = 16;
            row.style.marginLeft = 8;
            _questList.Add(row);
        }
    }
}

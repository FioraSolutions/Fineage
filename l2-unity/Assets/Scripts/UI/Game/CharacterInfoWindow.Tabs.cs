using System;
using UnityEngine.UIElements;

public partial class CharacterInfoWindow
{
    void WireMainWndTab(int index, Action onClick)
    {
        Button btn = _windowEle?.Q<Button>($"MainWndTab{index}");
        if (btn == null)
            return;
        btn.AddManipulator(new ButtonClickSoundManipulator(btn));
        btn.RegisterCallback<ClickEvent>(_ => onClick());
    }

    void SelectStatusTab()
    {
        SelectMainTab(1, updateTitle: true);
        ShowMainPanel(1);
    }

    void OpenSkillsTab()
    {
        SelectMainTab(2, updateTitle: true);
        ShowMainPanel(2);
        SkillListWindow skills = SkillLearnPacketServices.SkillList;
        if (skills != null)
            skills.EmbedInto(_skillsHost);
    }

    void OpenActionsTab()
    {
        SelectMainTab(3, updateTitle: true);
        ShowMainPanel(3);
        ActionWindow actions = HudCorePacketServices.Action;
        if (actions != null)
        {
            // Ensure ActionWnd slots exist, then park content under MainWnd (no second popup).
            actions.ShowWindow();
            actions.EmbedInto(_actionsHost);
        }
    }

    void OpenClanTab()
    {
        SelectMainTab(4, updateTitle: true);
        ShowMainPanel(1); // keep Status visible; Clan stays a sibling popup (Interlude PledgeWnd)
        ClanWindow clan = ClanPacketServices.Clan;
        if (clan != null)
            clan.ToggleHideWindow();
    }

    void OpenQuestTab()
    {
        SelectMainTab(5, updateTitle: true);
        ShowMainPanel(1);
        QuestWindow quest = QuestPacketServices.Quest;
        if (quest != null)
            quest.ToggleHideWindow();
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
}

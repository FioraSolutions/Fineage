using System;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine;

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
}

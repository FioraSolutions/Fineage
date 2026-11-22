using System;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class ChatWindow
{
    static string LayoutTarget(ChatType type)
    {
        if (type == ChatType.GENERAL)
            return "NormalChat";
        if (type == ChatType.TRADE)
            return "TradeChat";
        if (type == ChatType.PARTY)
            return "PartyChat";
        if (type == ChatType.CLAN)
            return "ClanChat";
        if (type == ChatType.ALLIANCE)
            return "AllyChat";
        return null;
    }

    private void CreateTabs()
    {
        if (_tabs == null || _tabs.Count == 0)
        {
            Debug.LogError("[ChatWindow] no chat tabs are assigned.");
            return;
        }

        VisualElement tabCtrl = GetElementById("ChatTabCtrl");
        if (tabCtrl == null)
        {
            Debug.LogError("[ChatWindow] ChatTabCtrl is missing from the extracted layout.");
            return;
        }

        ChatTab first = null;
        for (int i = 0; i < _tabs.Count; i++)
        {
            string target = LayoutTarget(_tabs[i].ChatType);
            if (string.IsNullOrEmpty(target))
            {
                Debug.LogError("[ChatWindow] no extracted tab for chat type " + _tabs[i].ChatType + ".");
                continue;
            }

            Button header = tabCtrl.Q<Button>("ChatTabCtrl_" + target);
            VisualElement area = GetElementById(target);
            if (header == null || area == null)
            {
                Debug.LogError("[ChatWindow] extracted chat tab '" + target + "' is missing.");
                continue;
            }

            VisualElement tabElement = _tabTemplate.CloneTree()[0];
            tabElement.name = _tabs[i].TabName;
            tabElement.AddToClassList("unselected-tab");
            tabElement.style.position = Position.Absolute;
            tabElement.style.left = 0f;
            tabElement.style.top = 0f;
            tabElement.style.right = 0f;
            tabElement.style.bottom = 0f;
            area.Add(tabElement);

            _tabs[i].SetMessageTemplate(_messageLabelTemplate);
            _tabs[i].Initialize(_windowEle, tabElement, header);
            if (first == null)
                first = _tabs[i];
        }

        if (first == null)
            return;

        SwitchTab(first);
        Ready = true;
        NetChatFeed.Flush();
    }

    public bool SwitchTab(ChatTab switchTo)
    {
        if (_activeTab != switchTo)
        {
            if (_activeTab != null)
            {
                _activeTab.TabContainer.AddToClassList("unselected-tab");
                _activeTab.TabHeader.RemoveFromClassList("active");
                InterludeWindowLayout.SetPressed(_activeTab.TabHeader, "ChatWnd", _activeTab.TabHeader.name, false);
            }

            switchTo.TabContainer.RemoveFromClassList("unselected-tab");
            switchTo.TabHeader.AddToClassList("active");
            InterludeWindowLayout.SetPressed(switchTo.TabHeader, "ChatWnd", switchTo.TabHeader.name, true);
            ScrollDown(switchTo.Scroller);

            _activeTab = switchTo;

            // Debug.Log("switching to: " + _activeTab.TabName);
            return true;
        }

        return false;
    }
}

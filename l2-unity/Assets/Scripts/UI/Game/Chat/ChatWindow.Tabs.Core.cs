using System;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class ChatWindow
{
    private void CreateTabs()
    {
        _chatTabView = GetElementById("ChatTabView");

        VisualElement tabHeaderContainer = _chatTabView.Q<VisualElement>("tab-header-container");
        if (tabHeaderContainer == null)
        {
            // Debug.LogError("tab-header-container is null");
        }
        VisualElement tabContainer = _chatTabView.Q<VisualElement>("tab-content-container");

        if (tabContainer == null)
        {
            // Debug.LogError("tab-content-container");
        }

        for (int i = 0; i < _tabs.Count; i++)
        {
            VisualElement tabElement = _tabTemplate.CloneTree()[0];
            // tabElement.name = _tabs[i].TabName;
            tabElement.name = _tabs[i].TabName;
            tabElement.AddToClassList("unselected-tab");

            VisualElement tabHeaderElement = _tabHeaderTemplate.CloneTree()[0];
            tabHeaderElement.name = _tabs[i].TabName;
            tabHeaderElement.Q<Label>().text = _tabs[i].TabName;

            tabHeaderContainer.Add(tabHeaderElement);
            tabContainer.Add(tabElement);

            _tabs[i].SetMessageTemplate(_messageLabelTemplate);
            _tabs[i].Initialize(_windowEle, tabElement, tabHeaderElement);

        }

        // Chatting_Tab3 is the top edge that continues to the right of the last tab.
        VisualElement tabFill = new VisualElement();
        tabFill.name = "TabRowFill";
        tabFill.pickingMode = PickingMode.Ignore;
        tabFill.style.flexGrow = 1f;
        tabFill.style.height = 13f;
        tabFill.style.alignSelf = Align.FlexStart;
        Texture2D tabEdge = Resources.Load<Texture2D>("Data/UI/Assets/Chat/chatting_tab3");
        if (tabEdge != null)
        {
            tabFill.style.backgroundImage = new StyleBackground(tabEdge);
            tabFill.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
        }
        tabHeaderContainer.Add(tabFill);

        if (_tabs.Count > 0)
        {
            SwitchTab(_tabs[0]);
        }

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
            }

            switchTo.TabContainer.RemoveFromClassList("unselected-tab");
            switchTo.TabHeader.AddToClassList("active");
            ScrollDown(switchTo.Scroller);

            _activeTab = switchTo;

            // Debug.Log("switching to: " + _activeTab.TabName);
            return true;
        }

        return false;
    }
}

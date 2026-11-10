

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

    void Update()
    {
        InputManager input = ControllerPacketServices.Input;
        if (input == null || !input.Validate)
            return;

        if (_chatOpened)
            CloseChat(true);
        else
            StartCoroutine(OpenChat());
    }

    IEnumerator OpenChat()
    {
        _chatOpened = true;
        SystemUIPacketServices.Ui.BlurFocus();
        yield return new WaitForEndOfFrame();
        _chatInput.Focus();
    }

    public void CloseChat(bool sendMessage)
    {
        _chatOpened = false;

        SystemUIPacketServices.Ui.BlurFocus();

        if (sendMessage)
        {
            string text = _chatInput.text;
            if (text.Length > 0)
            {
                if (CanSend(1100))
                {
                    if (IsGmCommand(text))
                    {
                        String prefix = "admin_";
                        string gmCommand = text.Substring(2);
                        string command = prefix + gmCommand;
                        ZoneCommands.Send(new RequestBypassToServerCommand(command));
                        // Debug.Log("ChatWindow: requested admin command :" + command);
                    }
                    else
                    {
                        var commands = PlayerCommands.FindByText(text);
                        if (commands != null)
                        {
                            ZoneCommands.Send(new RequestUserCommandCommand(commands.Id));
                            // Debug.Log("ChatWindow: requested player command :" + commands  + " id: " + commands.Id);
                        }
                        else
                        {
                            SendChatMessage(_activeTab, text);
                        }
                    }

                    _chatInput.value = "";
                }
                else
                    _activeTab.ConcatMessage("Server:Please dont spam the chat.");
            }
        }
    }

}

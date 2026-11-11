public partial class ChatWindow
{
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

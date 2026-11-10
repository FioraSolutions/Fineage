

public partial class ChatWindow
{
    public static bool IsGmCommand(string text)
    {
        return text != null
            && text.Length > 1
            && text[0] == '/'
            && text[1] == '/';
    }

    private long _flood = 0;

    bool CanSend(long cooldownMs)
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        if (now < _flood)
            return false;

        _flood = now + cooldownMs;
        return true;
    }

    private void OnChatInputFocus(FocusEvent evt)
    {
        if (!_chatInputContainer.ClassListContains("highlighted"))
        {
            _chatInputContainer.AddToClassList("highlighted");
        }

        if (!_chatOpened)
        {
            _chatOpened = true;
        }
    }

    private void OnChatInputBlur(BlurEvent evt)
    {
        if (_chatInputContainer.ClassListContains("highlighted"))
        {
            _chatInputContainer.RemoveFromClassList("highlighted");
        }

        if (_chatOpened)
        {
            _chatOpened = false;
        }
    }

    public void SendChatMessage(ChatTab tab, string text)
    {
        ChatTypeData data = ChatTypes.GetById(tab.TabId);
        if (data != null)
        {
            bool whisper = text[0] == '"';

            if (whisper)
            {
                text = text.Substring(1);

                int spaceIndex = text.IndexOf(' ');
                if (spaceIndex <= 0)
                    return;

                string targetName = text.Substring(0, spaceIndex);
                string message = text.Substring(spaceIndex + 1);

                ZoneCommands.Send(new RequestSay2Command(ChatTypes.GetById(2), message, targetName));
                // Debug.Log("whisper chat: sending to :" + targetName + " message: " + message);
            }
            else
            {
                ZoneCommands.Send(new RequestSay2Command(data, text, ""));
            }
        }
        else
        {
            // Debug.Log("SendChatMessage : Incorrect data for tab :" + tab.TabId);
        }
    }

}

public partial class ChatWindow
{
    public void ReceiveChatMessage(CreatureMessage data)
    {
        ChatType messageType = (ChatType)data._data.Type;

        // Debug.Log("Creature Say: Received:" + data.ToString() + " Type: " + messageType + " ChatTypeId: " + data._data.Type);

        bool isAnnounce = messageType == ChatType.ANNOUNCEMENT || messageType == ChatType.CRITICAL_ANNOUNCE;
        for (int i = 0; i < _tabs.Count; i++)
        {
            ChatTab tab = _tabs[i];

            if (tab.ChatType == messageType || tab.ChatType == ChatType.GENERAL && isAnnounce || tab.ChatType == ChatType.GENERAL && messageType == ChatType.WHISPER)
            {
                tab.ConcatMessage(data.ToString());
            }
        }
    }

    public void ReceiveSystemMessage(SystemMessage message)
    {
        if (message == null)
            return;
        AppendSystemLine(message.ToString());
    }

    public void ReceiveZoneSay(string name, string text, int chatType)
    {
        if (_tabs == null || _tabs.Count == 0)
            return;

        string line = NetChat.FormatLine(name, text, chatType);
        bool shown = false;
        for (int i = 0; i < _tabs.Count; i++)
        {
            ChatTab tab = _tabs[i];
            if (tab == null)
                continue;
            bool match = tab.TabId == chatType;
            bool onAll = tab.ChatType == ChatType.GENERAL && NetChat.ShowsOnAllTab(chatType);
            if (!match && !onAll)
                continue;
            tab.ConcatMessage(line);
            shown = true;
        }

        if (shown)
            return;

        for (int i = 0; i < _tabs.Count; i++)
        {
            ChatTab tab = _tabs[i];
            if (tab != null && tab.ChatType == ChatType.GENERAL)
            {
                tab.ConcatMessage(line);
                return;
            }
        }
    }
}

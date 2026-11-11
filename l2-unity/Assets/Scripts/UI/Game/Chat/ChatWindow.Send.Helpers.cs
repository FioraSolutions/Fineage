using System;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class ChatWindow
{
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

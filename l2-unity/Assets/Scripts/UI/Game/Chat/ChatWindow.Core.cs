using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ChatWindow : L2Window
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            WireChatPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireChatPacketServices();
    }

    private void WireChatPacketServices()
    {
        MessagePacketApply messageApply = MessagePacketServices.MessageApply;
        MessagePacketServices.Bind(messageApply: messageApply, chat: this);
    }
}

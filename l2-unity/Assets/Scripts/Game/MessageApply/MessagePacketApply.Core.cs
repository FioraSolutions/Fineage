using System;
using UnityEngine;
using static SMParam;
using System.Collections.Generic;

public sealed partial class MessagePacketApply
{
    public MessagePacketApply(
        ChatWindow chat,
        SystemMessageWindow systemMessageUi,
        PacketApplyQueue queue,
        SystemMessageTable systemMessages,
        SkillNameTable skillNames)
    {
        _chat = chat != null ? chat : MessagePacketServices.Chat;
        _systemMessageUi = systemMessageUi != null ? systemMessageUi : SystemMessagePacketServices.Window;
        _queue = queue;
        _systemMessages = systemMessages;
        _skillNames = skillNames;
    }

    public void SystemMessage(SystemMessageDto packet)
    {
        SMParam[] smParams = packet.Params;
        int messageId = packet.Id;
        SystemMessageDat messageData = _systemMessages.GetSystemMessage(messageId);
        OpenMessageWindow(messageId, messageData, smParams);

        if (messageData != null)
            ReceiveSystem(new SystemMessage(smParams, messageData));
        else
            ReceiveSystem(new UnhandledMessage());
    }

    public void CreatureSay(CreatureMessage message)
    {
        if (_chat == null)
        {
            ReceiveSystem(message);
            return;
        }

        _queue.Queue(() =>
        {
            try
            {
                _chat.ReceiveChatMessage(message);
            }
            catch (Exception)
            {
                ReceiveSystem(message);
            }
        });
    }
}

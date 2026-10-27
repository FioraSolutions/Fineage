using System;

/// <summary>
/// Chat/message wire packet handler access via MessagePacketServices (#142 sub-slice 59).
/// </summary>
public static class MessageIncomingPacketServices
{
    public static void ApplyMessage(Action<MessagePacketApply> apply) =>
        MessagePacketServices.ApplyMessage(apply);

    public static void ApplyCreatureSay(CreatureMessage message) =>
        ApplyMessage(apply => apply.CreatureSay(message));

    public static void ApplyNpcSay(SystemMessage npcMessage) =>
        ApplyMessage(apply => apply.NpcSay(npcMessage));

    public static void ApplySystemMessage(SystemMessageDto packet) =>
        ApplyMessage(apply => apply.SystemMessage(packet));
}

using System;

/// <summary>
/// NPC say wire packet handler entry (#142 batch slice 143).
/// </summary>
public static class NpcSayIncomingPacketServices
{
    public static void ApplyNpcSay(NpcSayDto packet)
    {
        if (packet.NpcMessage == null)
            return;

        ApplyNpcSay(packet.NpcMessage);
    }

    public static void ApplyNpcSay(SystemMessage npcMessage) =>
        MessagePacketServices.ApplyMessage(apply => apply.NpcSay(npcMessage));
}

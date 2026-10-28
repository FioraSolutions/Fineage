using System;

/// <summary>
/// Apply facade (#142 batch slice 152). Re-homes from <see cref="NpcSayIncomingPacketServices"/>.
/// 
/// NPC say wire packet handler entry (#142 batch slice 143).
/// </summary>
public static class NpcSayApplyIncomingPacketServices
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

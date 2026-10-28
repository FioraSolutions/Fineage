using System;

/// <summary>
/// NPC info wire packet handler entry (#142 batch slice 138).
/// </summary>
public static class NpcInfoIncomingPacketServices
{
    public static void ApplyNpcInfo(NpcInfoDto packet) =>
        NpcOrPetInfoIncomingPacketServices.Apply(packet);
}

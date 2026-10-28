using System;

/// <summary>
/// NPC info wire packet handler entry (#142 batch slice 138).
/// Apply facade (#142 batch slice 155). Re-homes from <see cref="NpcInfoIncomingPacketServices"/>.
/// </summary>
public static class NpcInfoApplyIncomingPacketServices
{
    public static void ApplyNpcInfo(NpcInfoDto packet) =>
        NpcOrPetInfoIncomingPacketServices.Apply(packet);
}

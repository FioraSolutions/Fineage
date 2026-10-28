using System;

/// <summary>
/// Pet info wire packet handler entry (#142 batch slice 138).
/// </summary>
public static class PetInfoIncomingPacketServices
{
    public static void ApplyPetInfo(NpcInfoDto packet) =>
        NpcOrPetInfoIncomingPacketServices.Apply(packet);
}

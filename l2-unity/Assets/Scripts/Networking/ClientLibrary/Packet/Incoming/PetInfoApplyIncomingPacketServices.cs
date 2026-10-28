using System;

/// <summary>
/// Pet info wire packet handler entry (#142 batch slice 138).
/// Apply facade (#142 batch slice 155). Re-homes from <see cref="PetInfoIncomingPacketServices"/>.
/// </summary>
public static class PetInfoApplyIncomingPacketServices
{
    public static void ApplyPetInfo(NpcInfoDto packet) =>
        NpcOrPetInfoApplyIncomingPacketServices.Apply(packet);
}

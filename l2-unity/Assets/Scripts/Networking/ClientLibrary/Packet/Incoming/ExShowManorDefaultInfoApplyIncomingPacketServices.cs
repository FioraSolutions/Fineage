using System;

/// <summary>
/// Ex show-manor-default-info wire packet handler entry (#142 batch slice 138).
/// Splits apply out of <see cref="ManorIncomingPacketServices"/> (sub-slice 64).
/// Apply facade (#142 batch slice 156). Re-homes from <see cref="ExShowManorDefaultInfoIncomingPacketServices"/>.
/// </summary>
public static class ExShowManorDefaultInfoApplyIncomingPacketServices
{
    public static void Queue(Action action) => GameApplyIncomingPacketServices.Queue(action);

    public static void ApplyExShowManorDefaultInfo(ExShowManorDefaultInfoDto packet) =>
        Queue(() =>
        {
            WorldApplyIncomingPacketServices.SeedInfo.SetDataDefaultManorInfo(packet.List);
            WorldApplyIncomingPacketServices.SeedInfo.ShowWindowActiveTabAllDefault();
        });

}

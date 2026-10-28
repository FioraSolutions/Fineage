using System;

/// <summary>
/// Die wire packet handler entry (#142 batch slice 144).
/// </summary>
public static class DieIncomingPacketServices
{
    public static void ApplyDie(DieDto packet) =>
        WorldApplyPacketServices.ApplyWorld(apply => apply.Die(packet));
}

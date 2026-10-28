/// <summary>
/// Apply facade (#142 batch slice 152). Re-homes from <see cref="ChangeWaitTypeIncomingPacketServices"/>.
/// 
/// Change wait-type wire packet handler entry (#142 batch slice 141).
/// </summary>
public static class ChangeWaitTypeApplyIncomingPacketServices
{
    public static void ApplyChangeWaitType(ChangeWaitTypeDto packet) =>
        WorldEffectApplyIncomingPacketServices.ApplyWorld(apply => apply.ChangeWaitType(packet));
}

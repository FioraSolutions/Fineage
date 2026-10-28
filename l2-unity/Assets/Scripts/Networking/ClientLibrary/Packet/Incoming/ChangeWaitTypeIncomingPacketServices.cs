/// <summary>
/// Change wait-type wire packet handler entry (#142 batch slice 141).
/// </summary>
public static class ChangeWaitTypeIncomingPacketServices
{
    public static void ApplyChangeWaitType(ChangeWaitTypeDto packet) =>
        WorldIncomingPacketServices.ApplyWorld(apply => apply.ChangeWaitType(packet));
}

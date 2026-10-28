/// <summary>
/// Revive wire packet handler entry (#142 batch slice 143).
/// </summary>
public static class ReviveIncomingPacketServices
{
    public static void ApplyRevive(ReviveDto dto) =>
        WorldIncomingPacketServices.ApplyWorld(apply => apply.Revive(dto));
}

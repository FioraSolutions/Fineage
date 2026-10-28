/// <summary>
/// Apply facade (#142 batch slice 152). Re-homes from <see cref="ReviveIncomingPacketServices"/>.
/// 
/// Revive wire packet handler entry (#142 batch slice 143).
/// </summary>
public static class ReviveApplyIncomingPacketServices
{
    public static void ApplyRevive(ReviveDto dto) =>
        PresenceApplyIncomingPacketServices.ApplyWorld(apply => apply.Revive(dto));
}

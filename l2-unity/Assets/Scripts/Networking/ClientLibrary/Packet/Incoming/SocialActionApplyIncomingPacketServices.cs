/// <summary>
/// Apply facade (#142 batch slice 152). Re-homes from <see cref="SocialActionIncomingPacketServices"/>.
/// 
/// Social action wire packet handler entry (#142 batch slice 141).
/// </summary>
public static class SocialActionApplyIncomingPacketServices
{
    public static void ApplySocialAction(SocialActionDto packet) =>
        PresenceApplyIncomingPacketServices.ApplyWorld(apply => apply.SocialAction(packet));
}

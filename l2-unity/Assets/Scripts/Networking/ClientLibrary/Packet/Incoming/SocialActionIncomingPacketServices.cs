/// <summary>
/// Social action wire packet handler entry (#142 batch slice 141).
/// </summary>
public static class SocialActionIncomingPacketServices
{
    public static void ApplySocialAction(SocialActionDto packet) =>
        WorldIncomingPacketServices.ApplyWorld(apply => apply.SocialAction(packet));
}

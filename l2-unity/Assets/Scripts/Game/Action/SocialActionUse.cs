/// <summary>
/// Panel / hotkey social gestures — Mobius RequestActionUse ids 12–14 (#465).
/// Zone fans SocialAction; client already applies it in NetSession.
/// </summary>
public sealed class SocialActionUse : L2Action
{
    readonly int _actionId;

    public SocialActionUse(int actionId)
    {
        _actionId = actionId;
    }

    public override void UseAction()
    {
        if (PlayerStatePacketServices.PlayerState != null &&
            PlayerStatePacketServices.PlayerState.State == PlayerState.DEAD)
            return;

        _ = NetSession.Ensure().SendActionUse(_actionId);
    }
}
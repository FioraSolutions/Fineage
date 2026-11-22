using UnityEngine;

/// <summary>
/// Shared Mobius <c>RequestActionUse</c> client entry: every ActionWnd / hotkey wired action
/// sends the Interlude action id to the zone and relies on the zone reply
/// (<c>ChangeWaitType</c>, <c>ChangeMoveType</c>, <c>SocialAction</c>, …) for pose and UI.
/// </summary>
public static class InterludeRequestActionUse
{
    public static bool TrySend(int actionId, bool ctrlPressed = false, bool shiftPressed = false)
    {
        if (actionId < 0)
        {
            Debug.LogError("[Action] RequestActionUse refused: invalid actionId=" + actionId);
            return false;
        }

        if (PlayerStatePacketServices.PlayerState != null &&
            PlayerStatePacketServices.PlayerState.State == PlayerState.DEAD)
            return false;

        NetSession session = NetSession.TryGetBound();
        if (session == null)
        {
            Debug.LogError(
                "[Action] RequestActionUse not sent: NetSession not bound actionId=" + actionId);
            return false;
        }

        _ = session.SendActionUse(actionId, ctrlPressed, shiftPressed);
        return true;
    }
}
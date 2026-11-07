using UnityEngine;

public class WalkRunAction : L2Action
{
    public WalkRunAction() : base() { }

    /// <summary>Mobius RequestActionUse 1 — zone toggles walk/run and fans ChangeMoveType (#465).</summary>
    public override void UseAction()
    {
        if (PlayerStatePacketServices.PlayerState != null &&
            PlayerStatePacketServices.PlayerState.State == PlayerState.DEAD)
            return;

        _ = NetSession.Ensure().SendActionUse((int)ActionType.WalkRun);
    }
}
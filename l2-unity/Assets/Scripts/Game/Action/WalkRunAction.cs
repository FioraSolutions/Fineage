using UnityEngine;

public class WalkRunAction : L2Action
{
    public WalkRunAction() : base() { }

    /// <summary>Mobius RequestActionUse 1 — zone toggles walk/run and fans ChangeMoveType (#465).</summary>
    public override void UseAction()
    {
        if (PlayerStateMachine.Instance != null &&
            PlayerStateMachine.Instance.State == PlayerState.DEAD)
            return;

        NetSession.Ensure().SendActionUse((int)ActionType.WalkRun);
    }
}
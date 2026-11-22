using UnityEngine;

/// <summary>RequestActionUse id is in the catalog but client/server wiring is not finished.</summary>
public sealed class NotImplementedAction : L2Action
{
    readonly int _actionId;

    public NotImplementedAction(int actionId) => _actionId = actionId;

    public override void UseAction()
    {
        if (PlayerStatePacketServices.PlayerState != null &&
            PlayerStatePacketServices.PlayerState.State == PlayerState.DEAD)
            return;

        string name = InterludeActionCatalog.TryGet(_actionId, out var e) ? e.CmdName : "?";
        Debug.LogError(
            "[Action] not implemented: id=" + _actionId + " name='" + name +
            "'. RequestActionUse has no client handler (see InterludeActionCatalog wire kind).");

        // Still forward so zone can reject with action_unsupported where applicable.
        InterludeRequestActionUse.TrySend(_actionId);
    }
}
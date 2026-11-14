/// <summary>Placeholder until the action's Net packet / animation is wired (icons show in ActionWnd now).</summary>
public sealed class StubAction : L2Action
{
    readonly int _actionId;

    public StubAction(int actionId) => _actionId = actionId;

    public override void UseAction()
    {
        if (PlayerStatePacketServices.PlayerState != null &&
            PlayerStatePacketServices.PlayerState.State == PlayerState.DEAD)
            return;

        NetSession session = NetSession.TryGetBound();
        if (session == null)
            return;

        int targetId = SelectedTargetId();
        switch ((ActionType)_actionId)
        {
            case ActionType.Trade:
                if (targetId > 0)
                    _ = session.SendTradeRequest(targetId);
                return;
            case ActionType.PartyInvite:
                if (targetId > 0)
                    _ = session.SendPartyInviteRequest(targetId, 0);
                return;
            case ActionType.PartyLeave:
                _ = session.SendPartyLeaveRequest();
                return;
            case ActionType.PartyKick:
                if (targetId > 0)
                    _ = session.SendPartyKickRequest(targetId);
                return;
            case ActionType.Pickup:
                int groundId = NearestGroundItemId();
                if (groundId > 0)
                    _ = session.SendPickupRequest(groundId);
                return;
            case ActionType.Assist:
                AssistTarget();
                return;
        }
    }

    static int SelectedTargetId()
    {
        TargetManager targets = ControllerPacketServices.Targets != null
            ? ControllerPacketServices.Targets
            : ActionPacketServices.Targets;
        if (targets == null)
            return 0;
        TargetData target = targets.Target != null ? targets.Target : targets.AttackTarget;
        if (target != null && target.Identity != null)
            return target.Identity.Id;
        return 0;
    }

    static void AssistTarget()
    {
        TargetManager targets = ControllerPacketServices.Targets != null
            ? ControllerPacketServices.Targets
            : ActionPacketServices.Targets;
        if (targets == null || targets.Target == null)
            return;
        Entity current = targets.Target.GetEntity();
        if (current == null || current.TargetId <= 0)
            return;
        PartyTargetAssistServices.TrySelectPartyMember(current.TargetId);
    }

    static int NearestGroundItemId()
    {
        PlayerEntity self = DomainPacketServices.LocalPlayer;
        World world = ControllerPacketServices.GameWorld;
        if (self == null || world == null)
            return 0;

        // Interlude pickup is about 200 world units. Unity scale is ~0.0189, so ~4m.
        const float maxSqr = 16f;
        int bestId = 0;
        float best = maxSqr;
        world.ForEachEntity(entity =>
        {
            ItemEntity item = entity as ItemEntity;
            if (item == null || item.Identity == null)
                return;
            float sqr = (item.transform.position - self.transform.position).sqrMagnitude;
            if (sqr >= best)
                return;
            best = sqr;
            bestId = item.Identity.Id;
        });
        return bestId;
    }
}

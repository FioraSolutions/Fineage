using UnityEngine;

/// <summary>
/// Select a party member as current target + attack target (assist feel) without new opcodes.
/// </summary>
public static class PartyTargetAssistServices
{
    /// <summary>Interlude friend/party target color hint (5).</summary>
    public const string PartyTargetColor = "#a2fbab";

    public static TargetManager ResolveTargets()
    {
        TargetManager injected = ActionPacketServices.Targets;
        if (injected != null)
            return injected;
        return TargetPacketServices.Targets;
    }

    public static bool HasSession()
    {
        return ZoneCommands.IsConnected;
    }

    /// <summary>Target a party member by object id and mark attack target for assist-style play.</summary>
    public static bool TrySelectPartyMember(int objectId)
    {
        if (objectId <= 0)
            return false;

        TargetManager targets = ResolveTargets();
        if (targets == null)
            return false;

        targets.NextTargetById(objectId, PartyTargetColor);
        if (!targets.HasTarget())
            return false;

        targets.SetAttackTarget();

        Entity entity = targets.Target != null ? targets.Target.GetEntity() : null;
        if (entity != null && entity.Identity != null && HasSession())
        {
            var l2jpos = entity.Identity.GetL2jPos();
            ZoneCommands.Send(new ClickActionCommand(
                entity.Identity.Id, (int)l2jpos.x, (int)l2jpos.y, (int)l2jpos.z, 0));
        }

        return true;
    }
}

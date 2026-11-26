using UnityEngine;

/// <summary>
/// Interlude level-up pillar/glow (e_u004_a), levelup.ogg, and optional social gesture — shared by
/// level-up, class change, and subclass celebration packets.
/// </summary>
public static class LevelUpCelebrationPresentation
{
    public static bool TryPlay(Entity entity, InterludePresentationKind kind, string triggerLabel)
    {
        if (kind != InterludePresentationKind.LevelUpCelebration)
            return false;

        if (entity == null)
        {
            Debug.LogError(
                "[LevelUpCelebration] dropped: null entity trigger=" + triggerLabel);
            return false;
        }

        EventBus bus = ControllerPacketServices.Bus;
        if (bus == null)
        {
            Debug.LogError(
                "[LevelUpCelebration] dropped: EventBus null trigger=" + triggerLabel +
                " entity=" + entity.name);
            return false;
        }

        int objectId = entity.Identity != null ? entity.Identity.Id : 0;
        bus.LevelUp(entity, objectId);
        return true;
    }
}
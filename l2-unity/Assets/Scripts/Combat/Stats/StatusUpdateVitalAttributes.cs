using System.Collections.Generic;

/// <summary>
/// Applies Interlude StatusUpdate HP/MP ids (0x09–0x0c) from zone regen and combat fan-out.
/// </summary>
public static class StatusUpdateVitalAttributes
{
    public static bool TryApply(Status status, Stats stats, int attributeId, int value)
    {
        if (status == null || stats == null)
            return false;

        switch ((StatusUpdate.AttributeType)attributeId)
        {
            case StatusUpdate.AttributeType.CUR_HP:
                status.SetHp(value);
                return true;
            case StatusUpdate.AttributeType.MAX_HP:
                stats.MaxHp = value;
                return true;
            case StatusUpdate.AttributeType.CUR_MP:
                status.SetMp(value);
                return true;
            case StatusUpdate.AttributeType.MAX_MP:
                stats.MaxMp = value;
                return true;
            default:
                return false;
        }
    }

    public static bool ApplyAll(Status status, Stats stats, IList<StatusUpdate.Attribute> attributes)
    {
        if (status == null || stats == null || attributes == null || attributes.Count == 0)
            return false;

        bool any = false;
        for (int i = 0; i < attributes.Count; i++)
        {
            if (TryApply(status, stats, attributes[i].id, attributes[i].value))
                any = true;
        }

        ReconcilePlayerCaps(status, stats);
        return any;
    }

    public static void ReconcilePlayerCaps(Status status, Stats stats)
    {
        if (stats is PlayerStats playerStats && status is PlayerStatus playerStatus)
            PlayerResourceCaps.EnsureMaxAtLeastCurrent(playerStats, playerStatus);
    }
}

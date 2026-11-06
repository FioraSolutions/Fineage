using System;
using System.Collections.Generic;

/// <summary>
/// Client mirror of the zone's auto-shot state (Mobius <c>RequestAutoSoulShot</c> / <c>ExAutoSoulShot</c>).
///
/// <para>
/// In the original client a soulshot / spiritshot on the shortcut bar is not a "use one" button: clicking
/// it (or pressing its key) arms automatic use, and clicking again disarms it. The zone answers with
/// <c>AutoSoulShot</c> and then recharges the weapon after every swing. This keeps the requested state so
/// a second click knows whether it is turning the shot on or off, and so the slot can show that it is
/// armed. The zone also clears a shot by itself (weapon swap, empty stack) and announces that through
/// the same opcode.
/// </para>
/// </summary>
public static class NetAutoShot
{
    /// <summary>Classic <c>etcitem_type</c> for both soulshots and spiritshots.</summary>
    const string ShotEtcType = "soul_shot";

    static readonly object Gate = new object();
    static readonly HashSet<int> Enabled = new HashSet<int>();

    public static bool IsEnabled(int itemId)
    {
        lock (Gate)
            return Enabled.Contains(itemId);
    }

    public static IReadOnlyCollection<int> EnabledItems
    {
        get
        {
            lock (Gate)
                return new List<int>(Enabled);
        }
    }

    public static void Reset()
    {
        lock (Gate)
            Enabled.Clear();
    }

    /// <summary>Applies the zone's confirmation. Returns true when the state actually changed.</summary>
    public static bool Apply(int itemId, bool enabled)
    {
        if (itemId <= 0)
            return false;
        lock (Gate)
        {
            if (enabled)
                return Enabled.Add(itemId);
            return Enabled.Remove(itemId);
        }
    }

    /// <summary>
    /// A shot is an etc item whose <c>etcitem_type</c> is <c>soul_shot</c>. The client data file only
    /// carries that one token: spiritshot and blessed spiritshot rows share it, and the zone picks the
    /// charge kind from its own template (<c>isSoulshot</c> / <c>isSpiritshot</c>).
    /// </summary>
    public static bool IsShotItem(int itemId)
    {
        if (itemId <= 0)
            return false;
        EtcItemgrpTable table = TablePacketServices.EtcItemGrps;
        // Before the data tables are read the group map is null; a click that early is not a shot.
        if (table == null || table.EtcItemGrps == null)
            return false;
        EtcItemgrp grp = table.GetEtcItem(itemId);
        if (grp == null)
            return false;
        return string.Equals(grp.EtcItemType, ShotEtcType, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Click / key entry point: flips the shot and sends the request. Returns the new intent.</summary>
    public static bool Toggle(int itemId)
    {
        if (!IsShotItem(itemId))
            return false;
        bool enable = !IsEnabled(itemId);
        ZoneCommands.Send(new RequestAutoSoulShotCommand(itemId, enable));
        return enable;
    }
}

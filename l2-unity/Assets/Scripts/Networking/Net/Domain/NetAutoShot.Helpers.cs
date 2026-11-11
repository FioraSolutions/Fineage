using System;
using System.Collections.Generic;

public static partial class NetAutoShot
{
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

    public static bool Toggle(int itemId)
    {
        if (!IsShotItem(itemId))
            return false;
        bool enable = !IsEnabled(itemId);
        ZoneCommands.Send(new RequestAutoSoulShotCommand(itemId, enable));
        return enable;
    }
}

using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public static partial class EntitySpawnShared
{
    static void ReportUnresolved(int npcId, string reason)
    {
        if (!ReportedUnresolvedNpcIds.Add(npcId))
            return;

        if (_unresolvedReportsSuppressed)
            return;
        if (ReportedUnresolvedNpcIds.Count > MaxUnresolvedReports)
        {
            _unresolvedReportsSuppressed = true;
            FineageLog.Warn(
                FineageLogCategory.World,
                "NpcInfo unresolved reports capped at " + MaxUnresolvedReports
                + " distinct npcIds; further ids are not logged.");
            return;
        }

        FineageLog.Warn(
            FineageLogCategory.World,
            "NpcInfo npcId=" + npcId + " not rendered: " + reason);
    }

    static void ReportSubstituted(int npcId, string wanted, string used)
    {
        string key = wanted + " -> " + used;
        if (!ReportedSubstitutedMeshes.Add(key))
            return;
        // Art-pack gap (prefab absent under Resources/Data/Animations/...), not a broken texture.
        // Once per wanted→used pair per play session so enter-world knownlist does not flood the console.
        FineageLog.Info(
            FineageLogCategory.World,
            "NpcInfo npcId=" + npcId + " art pack missing mesh '" + wanted
            + "'; stand-in '" + used + "'");
    }
}

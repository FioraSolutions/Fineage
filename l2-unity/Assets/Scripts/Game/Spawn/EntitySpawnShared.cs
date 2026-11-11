using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class EntitySpawnShared
{
    /// <summary>npcIds already reported as unresolvable, so a repeated NpcInfo does not spam logs.</summary>
    static readonly HashSet<int> ReportedUnresolvedNpcIds = new HashSet<int>();
    const int MaxUnresolvedReports = 20;
    static bool _unresolvedReportsSuppressed;


    /// <summary>
    /// One line per npcId when the client cannot build a spawn request. Without this the NPC
    /// silently disappears, which looks identical to the server never sending NpcInfo.
    /// </summary>

    /// <summary>
    /// Reported once per substituted mesh so a missing asset pack variant is visible in the
    /// log instead of being silently papered over.
    /// </summary>
}

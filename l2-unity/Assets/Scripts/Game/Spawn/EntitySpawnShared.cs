using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public static partial class EntitySpawnShared
{
    /// <summary>
    /// npcIds already reported as unresolvable: one line per distinct npcId, never capped (the set is bounded by
    /// the NPC template count), so the complete list of NPCs the client cannot render is in the log.
    /// </summary>
    static readonly HashSet<int> ReportedUnresolvedNpcIds = new HashSet<int>();


    /// <summary>
    /// One line per npcId when the client cannot build a spawn request. Without this the NPC
    /// silently disappears, which looks identical to the server never sending NpcInfo.
    /// </summary>

    /// <summary>
    /// Reported once per substituted mesh so a missing asset pack variant is visible in the
    /// log instead of being silently papered over.
    /// </summary>
}

using System.Collections.Generic;
using UnityEngine;

public static partial class MagicShotCoordinator
{
    public static void CompleteCast(int objectId, MagicCastData castData)
    {
        if (objectId <= 0 || castData == null)
        {
            return;
        }

        int castStartMs = Mathf.RoundToInt(castData.StartTime * 1000f);
        lock (Sync)
        {
            if (SessionsByObjectId.TryGetValue(objectId, out ShotSession existingSession) &&
                existingSession.CastStartMs == castStartMs)
            {
                SessionsByObjectId.Remove(objectId);
            }
        }
    }

    private static void CleanupStaleSessions(float now)
    {
        if (SessionsByObjectId.Count == 0)
        {
            return;
        }

        List<int> staleObjectIds = null;
        foreach (KeyValuePair<int, ShotSession> pair in SessionsByObjectId)
        {
            if (now - pair.Value.LastUpdatedTime > StaleSessionLifetimeSeconds)
            {
                if (staleObjectIds == null)
                {
                    staleObjectIds = new List<int>();
                }

                staleObjectIds.Add(pair.Key);
            }
        }

        if (staleObjectIds == null)
        {
            return;
        }

        for (int i = 0; i < staleObjectIds.Count; i++)
        {
            SessionsByObjectId.Remove(staleObjectIds[i]);
        }
    }
}

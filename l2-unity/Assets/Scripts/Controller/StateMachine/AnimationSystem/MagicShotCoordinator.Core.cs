using System.Collections.Generic;
using UnityEngine;

public static partial class MagicShotCoordinator
{
    public static bool TryStartShot(int objectId, MagicCastData castData, string source, out string message)
    {
        if (objectId <= 0)
        {
            message = $"[MagicShotCoordinator] ALLOW source={source} objectId={objectId} reason=invalidObjectId";
            return true;
        }

        if (castData == null)
        {
            message = $"[MagicShotCoordinator] ALLOW source={source} objectId={objectId} reason=castDataNull";
            return true;
        }

        int castStartMs = Mathf.RoundToInt(castData.StartTime * 1000f);
        float now = Time.time;

        lock (Sync)
        {
            CleanupStaleSessions(now);

            if (SessionsByObjectId.TryGetValue(objectId, out ShotSession existingSession))
            {
                if (existingSession.CastStartMs == castStartMs)
                {
                    message =
                        $"[MagicShotCoordinator] BLOCK source={source} objectId={objectId} castStartMs={castStartMs} reason=alreadyStarted";
                    return false;
                }
            }

            SessionsByObjectId[objectId] = new ShotSession
            {
                CastStartMs = castStartMs,
                LastUpdatedTime = now
            };
        }

        message = $"[MagicShotCoordinator] ALLOW source={source} objectId={objectId} castStartMs={castStartMs}";
        return true;
    }
}

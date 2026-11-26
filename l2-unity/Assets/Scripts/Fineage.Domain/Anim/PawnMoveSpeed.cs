using UnityEngine;

/// <summary>
/// One apply for the wire's live run/walk speed.
/// <see cref="Stats.BaseRunSpeed"/> is the template GroundSpeed the clip was authored at.
/// A later packet on the same template (Dash, weight, walk/run) updates the live speed only,
/// so locomotion playback stays current/authored. A class change is a new template.
/// A missing speed is left unset; callers log it. Nothing here invents 120.
/// </summary>
public static class PawnMoveSpeed
{
    public static void Apply(Stats stats, float liveRun, float liveWalk, bool templateChanged)
    {
        if (stats == null)
            return;

        if (liveRun > 0.001f)
        {
            if (templateChanged || stats.BaseRunSpeed <= 0)
                stats.BaseRunSpeed = Mathf.RoundToInt(liveRun);
            stats.RunRealSpeed = liveRun;
            stats.Speed = Mathf.RoundToInt(liveRun);
        }

        if (liveWalk > 0.001f)
        {
            if (templateChanged || stats.BaseWalkingSpeed <= 0)
                stats.BaseWalkingSpeed = Mathf.RoundToInt(liveWalk);
            stats.WalkRealSpeed = liveWalk;
        }
    }
}

using System;
using System.Threading.Tasks;
using UnityEngine;

public partial class AnimationManager
{
    public void OnAnimationFinished(string name, int objectId)
    {
        TryCompleteAwaitFromPhaseFinish(objectId, name, "clip_complete");
    }

    public void NotifyMagicPhaseFinished(int objectId, string phaseName)
    {
        MagicGaitTrace.EventId(objectId, "NotifyMagicPhaseFinished", "phase=" + phaseName);
        TryCompleteAwaitFromPhaseFinish(objectId, phaseName, "smb_phase");
    }

    private bool TryCompleteAwaitFromPhaseFinish(int objectId, string name, string source)
    {
        // Debug.Log(
            // $"[AnimFinishedEvent] objectId={objectId} finishedName='{name}' source={source} now={Time.time:F3}");

        // SpAtk / jatk / Cast / MagicShot awaits are completed by SMB NotifyMagicPhaseFinished.
        // Early clip OnAnimationComplete must not finish the skill runner mid-dual.
        if (string.Equals(source, "clip_complete", StringComparison.Ordinal) &&
            IsSmbOwnedCombatOrMagicPhase(name))
        {
            // Debug.Log(
                // $"[AnimFinishedEvent] IGNORE clip_complete objectId={objectId} finishedName='{name}' " +
                // $"(SMB owns phase finish)");
            return false;
        }

        if (_expectedFinishNameByObjectId.TryGetValue(objectId, out string expectedName))
        {
            if (string.Equals(source, "clip_complete", StringComparison.Ordinal) &&
                IsSmbOwnedCombatOrMagicPhase(expectedName))
            {
                // Debug.Log(
                    // $"[AnimFinishedEvent] IGNORE clip_complete objectId={objectId} " +
                    // $"finishedName='{name}' expected='{expectedName}' (SMB owns phase finish)");
                return false;
            }

            if (!string.Equals(name, expectedName, StringComparison.Ordinal) &&
                !string.Equals(SkillAnimationDatabase.ResolveOverrideCompletionEventName(name), expectedName, StringComparison.Ordinal))
            {
                // Debug.Log(
                    // $"[AnimFinishedEvent] IGNORE objectId={objectId} finishedName='{name}' expected='{expectedName}' " +
                    // $"source={source} now={Time.time:F3}");
                return false;
            }
        }

        if (_tcsMap.TryGetValue(objectId, out var tcs))
        {
            _tcsMap.Remove(objectId);
            tcs.TrySetResult(true);
            return true;
        }

        return false;
    }

    private static bool IsSmbOwnedCombatOrMagicPhase(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }

        return name.StartsWith("SpAtk", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith("jatk", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith("CastMid", StringComparison.Ordinal)
            || name.StartsWith("CastEnd", StringComparison.Ordinal)
            || name.StartsWith("MagicShot", StringComparison.Ordinal);
    }

}

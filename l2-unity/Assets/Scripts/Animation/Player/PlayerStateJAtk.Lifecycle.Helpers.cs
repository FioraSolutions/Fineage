using UnityEngine;

public partial class PlayerStateJAtk : StateMachineBehaviour
{
    private void LogAnimMilestones(float elapsedSec)
    {
        float elapsedMs = TimeUtils.ConvertSecToMs(elapsedSec);
        float wallNorm = _fullCycleMs > 0f ? elapsedMs / _fullCycleMs : 0f;
        TryLogMilestone(0, 0.25f, elapsedMs, wallNorm);
        TryLogMilestone(1, 0.40f, elapsedMs, wallNorm);
        TryLogMilestone(2, 0.50f, elapsedMs, wallNorm);
        TryLogMilestone(3, 0.75f, elapsedMs, wallNorm);
    }

    private void TryLogMilestone(int bit, float animThreshold, float elapsedMs, float wallNorm)
    {
        int flag = 1 << bit;
        if ((_milestoneMask & flag) != 0) return;
        if (_lastAnimNormalized < animThreshold) return;

        _milestoneMask |= flag;
        // Debug.Log(
            // $"[ATK_HIT_CHAIN] AnimMilestone anim={parameterName} animNorm>={animThreshold:F2} " +
            // $"animNorm={_lastAnimNormalized:F3} wallElapsedMs={elapsedMs:F1} wallNorm={wallNorm:F3} " +
            // $"frame={Time.frameCount}");
    }
}

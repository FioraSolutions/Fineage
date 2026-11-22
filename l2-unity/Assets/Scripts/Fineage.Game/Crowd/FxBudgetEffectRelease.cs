using UnityEngine;

/// <summary>Releases <see cref="FxBudgetRegistry"/> slot when a gated skill FX is destroyed.</summary>
public sealed class FxBudgetEffectRelease : MonoBehaviour
{
    CrowdDetailTier _tier;

    public static void AttachIfNeeded(GameObject fxRoot, int casterEntityId, bool highPriority)
    {
        if (casterEntityId <= 0 || highPriority)
            return;

        CrowdDetailTier tier = CrowdTierPacketServices.GetTier(casterEntityId);
        if (tier <= CrowdDetailTier.T1Near)
            return;

        var release = fxRoot.GetComponent<FxBudgetEffectRelease>();
        if (release == null)
            release = fxRoot.AddComponent<FxBudgetEffectRelease>();
        release._tier = tier;
    }

    void OnDestroy() => FxBudgetRegistry.Release(_tier);
}
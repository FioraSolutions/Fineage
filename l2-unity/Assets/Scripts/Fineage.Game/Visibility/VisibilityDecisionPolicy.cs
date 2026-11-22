using System;
using System.Collections.Generic;

/// <summary>Pure visibility decisions for EditMode tests (no Unity scene).</summary>
public static class VisibilityDecisionPolicy
{
    public static CrowdDetailTier TierForDistanceSq(
        float distSq,
        float tier1MaxSq,
        float tier2MaxSq,
        float tier3MaxSq,
        float pawnDrawMaxSq)
    {
        if (distSq > pawnDrawMaxSq)
            return CrowdDetailTier.TCulled;
        if (distSq <= tier1MaxSq)
            return CrowdDetailTier.T1Near;
        if (distSq <= tier2MaxSq)
            return CrowdDetailTier.T2Mid;
        if (distSq <= tier3MaxSq)
            return CrowdDetailTier.T3Far;
        return CrowdDetailTier.TCulled;
    }

    /// <summary>
    /// Enforces RENDERCHARACTERCOUNT: keep nearest spawned pawns, cull the rest.
    /// </summary>
    public static void ApplyRenderCharacterCap(
        int[] entityIds,
        float[] distSq,
        CrowdDetailTier[] tiers,
        int maxCharacters,
        int localPlayerIndex)
    {
        if (maxCharacters <= 0 || entityIds == null || entityIds.Length == 0)
            return;

        var scratch = new List<(int index, float d)>(entityIds.Length);
        for (int i = 0; i < entityIds.Length; i++)
        {
            if (i == localPlayerIndex)
                continue;
            if (tiers[i] == CrowdDetailTier.TCulled)
                continue;
            scratch.Add((i, distSq[i]));
        }

        if (scratch.Count <= maxCharacters)
            return;

        scratch.Sort((a, b) => a.d.CompareTo(b.d));
        for (int k = maxCharacters; k < scratch.Count; k++)
            tiers[scratch[k].index] = CrowdDetailTier.TCulled;
    }
}
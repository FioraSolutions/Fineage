using System;

/// <summary>Tracks active skill FX for roadmap step 17 tier budgets.</summary>
public static class FxBudgetRegistry
{
    static int _t2Active;
    static int _t3Active;

    public static int ActiveT2 => _t2Active;
    public static int ActiveT3 => _t3Active;

    public static bool TryAcquire(CrowdDetailTier tier, CrowdLodProfile profile)
    {
        if (profile == null)
            return true;

        if (tier == CrowdDetailTier.T2Mid)
        {
            if (_t2Active >= profile.maxConcurrentSkillFxT2)
                return false;
            _t2Active++;
            return true;
        }

        if (tier >= CrowdDetailTier.T3Far)
        {
            if (_t3Active >= profile.maxConcurrentSkillFxT3)
                return false;
            _t3Active++;
            return true;
        }

        return true;
    }

    public static void Release(CrowdDetailTier tier)
    {
        if (tier == CrowdDetailTier.T2Mid)
            _t2Active = Math.Max(0, _t2Active - 1);
        else if (tier >= CrowdDetailTier.T3Far)
            _t3Active = Math.Max(0, _t3Active - 1);
    }
}
/// <summary>Skill FX spawn policy by crowd tier (roadmap §7.2 step 17).</summary>
public static class FxBudgetPolicy
{
    public static bool ShouldSpawnSkillFx(int casterEntityId, bool casterIsHighPriority)
    {
        if (casterEntityId <= 0 || casterIsHighPriority)
            return true;

        CrowdDetailTier tier = CrowdTierPacketServices.GetTier(casterEntityId);
        if (tier <= CrowdDetailTier.T1Near)
            return true;

        CrowdLodProfile profile = CrowdLodProfileCatalog.LoadActive();
        using (FineagePerfProfiling.FxBudget.Auto())
            return FxBudgetRegistry.TryAcquire(tier, profile);
    }
}
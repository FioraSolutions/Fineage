using NUnit.Framework;
using UnityEngine;

public sealed class FxBudgetRegistryEditModeTests
{
    [SetUp]
    public void Reset() => FxBudgetRegistryTestsReset.ReleaseAll();

    [Test]
    public void T1_casts_do_not_consume_budget()
    {
        var profile = ScriptableObject.CreateInstance<CrowdLodProfile>();
        profile.maxConcurrentSkillFxT2 = 1;
        profile.maxConcurrentSkillFxT3 = 1;
        Assert.True(FxBudgetRegistry.TryAcquire(CrowdDetailTier.T1Near, profile));
        Assert.True(FxBudgetRegistry.TryAcquire(CrowdDetailTier.T1Near, profile));
        Assert.AreEqual(0, FxBudgetRegistry.ActiveT2);
        Assert.AreEqual(0, FxBudgetRegistry.ActiveT3);
    }

    [Test]
    public void T3_caps_concurrent_other_casts()
    {
        var profile = ScriptableObject.CreateInstance<CrowdLodProfile>();
        profile.maxConcurrentSkillFxT3 = 2;
        Assert.True(FxBudgetRegistry.TryAcquire(CrowdDetailTier.T3Far, profile));
        Assert.True(FxBudgetRegistry.TryAcquire(CrowdDetailTier.T3Far, profile));
        Assert.False(FxBudgetRegistry.TryAcquire(CrowdDetailTier.T3Far, profile));
        FxBudgetRegistry.Release(CrowdDetailTier.T3Far);
        Assert.True(FxBudgetRegistry.TryAcquire(CrowdDetailTier.T3Far, profile));
    }
}

/// <summary>Test-only reset for static registry counters.</summary>
static class FxBudgetRegistryTestsReset
{
    public static void ReleaseAll()
    {
        while (FxBudgetRegistry.ActiveT2 > 0)
            FxBudgetRegistry.Release(CrowdDetailTier.T2Mid);
        while (FxBudgetRegistry.ActiveT3 > 0)
            FxBudgetRegistry.Release(CrowdDetailTier.T3Far);
    }
}
using NUnit.Framework;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public sealed class CrowdTierJobsEditModeTests
{
    [Test]
    public void AssignCrowdTierJob_maps_squared_distances()
    {
        var positions = new NativeArray<float3>(3, Allocator.Temp);
        var tiers = new NativeArray<byte>(3, Allocator.Temp);
        positions[0] = float3.zero;
        positions[1] = new float3(30f, 0f, 0f);
        positions[2] = new float3(100f, 0f, 0f);

        var job = new AssignCrowdTierJob
        {
            Positions = positions,
            Observer = float3.zero,
            Tier1MaxSq = 25f * 25f,
            Tier2MaxSq = 60f * 60f,
            Tier3MaxSq = 120f * 120f,
            Tiers = tiers,
        };

        job.Execute(0);
        job.Execute(1);
        job.Execute(2);

        Assert.AreEqual((byte)CrowdDetailTier.T1Near, tiers[0]);
        Assert.AreEqual((byte)CrowdDetailTier.T2Mid, tiers[1]);
        Assert.AreEqual((byte)CrowdDetailTier.T3Far, tiers[2]);

        positions.Dispose();
        tiers.Dispose();
    }

    [Test]
    public void CrowdLodProfile_throttles_animator_speed_by_tier()
    {
        var profile = UnityEngine.ScriptableObject.CreateInstance<CrowdLodProfile>();
        Assert.AreEqual(0.5f, profile.AnimatorSpeedForTier(CrowdDetailTier.T2Mid));
        Assert.AreEqual(AnimatorCullingMode.CullCompletely, profile.AnimatorCullingForTier(CrowdDetailTier.T3Far));
        UnityEngine.Object.DestroyImmediate(profile);
    }

    [Test]
    public void CrowdLodProfile_hides_nameplate_at_configured_tier()
    {
        var profile = UnityEngine.ScriptableObject.CreateInstance<CrowdLodProfile>();
        profile.hideNameplateFromTier = CrowdDetailTier.T3Far;
        Assert.IsTrue(profile.ShouldShowNameplate(CrowdDetailTier.T2Mid));
        Assert.IsFalse(profile.ShouldShowNameplate(CrowdDetailTier.T3Far));
        UnityEngine.Object.DestroyImmediate(profile);
    }
}
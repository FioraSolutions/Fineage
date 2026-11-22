using NUnit.Framework;

public sealed class VisibilityManagerEditModeTests
{
    [Test]
    public void TierForDistanceSq_culls_beyond_pawn_draw_and_tier3()
    {
        float t1 = 25f * 25f;
        float t2 = 60f * 60f;
        float t3 = 120f * 120f;
        float pawn = 36f * 36f;

        Assert.AreEqual(CrowdDetailTier.T1Near,
            VisibilityDecisionPolicy.TierForDistanceSq(10f * 10f, t1, t2, t3, pawn));
        Assert.AreEqual(CrowdDetailTier.T2Mid,
            VisibilityDecisionPolicy.TierForDistanceSq(30f * 30f, t1, t2, t3, pawn));
        Assert.AreEqual(CrowdDetailTier.TCulled,
            VisibilityDecisionPolicy.TierForDistanceSq(50f * 50f, t1, t2, t3, pawn));
        Assert.AreEqual(CrowdDetailTier.TCulled,
            VisibilityDecisionPolicy.TierForDistanceSq(200f * 200f, t1, t2, t3, pawn));
    }

    [Test]
    public void ApplyRenderCharacterCap_culls_farthest_over_budget()
    {
        var ids = new[] { 1, 2, 3, 4 };
        var distSq = new[] { 100f, 400f, 900f, 1600f };
        var tiers = new[]
        {
            CrowdDetailTier.T1Near,
            CrowdDetailTier.T1Near,
            CrowdDetailTier.T1Near,
            CrowdDetailTier.T1Near,
        };

        VisibilityDecisionPolicy.ApplyRenderCharacterCap(ids, distSq, tiers, maxCharacters: 2, localPlayerIndex: -1);

        Assert.AreEqual(CrowdDetailTier.T1Near, tiers[0]);
        Assert.AreEqual(CrowdDetailTier.T1Near, tiers[1]);
        Assert.AreEqual(CrowdDetailTier.TCulled, tiers[2]);
        Assert.AreEqual(CrowdDetailTier.TCulled, tiers[3]);
    }
}
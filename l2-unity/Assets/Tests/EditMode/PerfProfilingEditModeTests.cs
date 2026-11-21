using NUnit.Framework;

public sealed class PerfProfilingEditModeTests
{
    [Test]
    public void Roadmap_marker_names_match_perf_contract()
    {
        CollectionAssert.AreEqual(
            new[]
            {
                "Fineage.Net.Read",
                "Fineage.Net.Decode",
                "Fineage.Net.Queue",
                "Fineage.Net.Drain",
                "Fineage.Net.Apply",
                "Fineage.Move.Step",
                "Fineage.Nameplates.Update",
                "Fineage.ActorLights.Push",
                "Fineage.Spawn",
                "Fineage.Tile.Load",
                "Fineage.Crowd.Tier",
            },
            FineagePerfProfiling.RoadmapMarkerNames);

        Assert.AreEqual(NetReceiveProfiling.ReadName, FineagePerfProfiling.RoadmapMarkerNames[0]);
        Assert.AreEqual(FineagePerfProfiling.MoveStepName, FineagePerfProfiling.RoadmapMarkerNames[5]);
        Assert.AreEqual(FineagePerfProfiling.CrowdTierName, FineagePerfProfiling.RoadmapMarkerNames[10]);
    }

    [Test]
    public void Overlay_disabled_without_env_or_flag()
    {
        Assert.IsFalse(FineagePerfOverlaySettings.Enabled);
    }
}
using NUnit.Framework;
using Unity.Profiling;

public sealed class PerfProfilingEditModeTests
{
    [Test]
    public void Roadmap_markers_use_expected_names()
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

        Assert.AreEqual("Fineage.Net.Read", NetReceiveProfiling.Read.ToString());
        Assert.AreEqual("Fineage.Move.Step", FineagePerfProfiling.MoveStep.ToString());
        Assert.AreEqual("Fineage.Crowd.Tier", FineagePerfProfiling.CrowdTier.ToString());
    }

    [Test]
    public void Overlay_disabled_without_env_or_flag()
    {
        Assert.IsFalse(FineagePerfOverlaySettings.Enabled);
    }
}
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

/// <summary>Roadmap step 2: offline pump bench prints p50/p95/p99 for CI logs.</summary>
[Category("Slow")]
public sealed class CrowdReplayBenchEditModeTests
{
    [Test]
    public void Synthetic_pump_bench_prints_percentiles_for_100_300_1000()
    {
        IReadOnlyDictionary<int, FineageFrameTimePercentiles> tiers =
            CrowdNetPumpBench.RunSyntheticPumpTiers(CrowdNetPumpBench.DefaultPawnTiers);

        var report = new StringBuilder();
        report.AppendLine("FineageCrowdReplayBench (synthetic NetMainThreadPump MoveState coalesce):");
        foreach (KeyValuePair<int, FineageFrameTimePercentiles> kv in tiers)
        {
            report.Append("  pawns=").Append(kv.Key).Append(' ').AppendLine(kv.Value.ToString());
        }

        TestContext.WriteLine(report.ToString());

        Assert.AreEqual(3, tiers.Count);
        Assert.Greater(tiers[100].SampleCount, 0);
        Assert.GreaterOrEqual(tiers[1000].P50Ms, 0d);
    }
}
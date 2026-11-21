using System;
using System.Collections.Generic;

/// <summary>Percentile summary for offline crowd/net benches (roadmap step 2).</summary>
public readonly struct FineageFrameTimePercentiles
{
    public readonly double P50Ms;
    public readonly double P95Ms;
    public readonly double P99Ms;
    public readonly int SampleCount;

    public FineageFrameTimePercentiles(double p50, double p95, double p99, int sampleCount)
    {
        P50Ms = p50;
        P95Ms = p95;
        P99Ms = p99;
        SampleCount = sampleCount;
    }

    public static FineageFrameTimePercentiles FromFrameTimesMs(List<double> frameMs)
    {
        if (frameMs == null || frameMs.Count == 0)
            return new FineageFrameTimePercentiles(0, 0, 0, 0);

        frameMs.Sort();
        return new FineageFrameTimePercentiles(
            Percentile(frameMs, 0.50),
            Percentile(frameMs, 0.95),
            Percentile(frameMs, 0.99),
            frameMs.Count);
    }

    static double Percentile(List<double> sorted, double p)
    {
        if (sorted.Count == 0)
            return 0;
        double rank = p * (sorted.Count - 1);
        int lo = (int)Math.Floor(rank);
        int hi = (int)Math.Ceiling(rank);
        if (lo == hi)
            return sorted[lo];
        double frac = rank - lo;
        return sorted[lo] * (1d - frac) + sorted[hi] * frac;
    }

    public override string ToString() =>
        $"p50={P50Ms:0.###}ms p95={P95Ms:0.###}ms p99={P99Ms:0.###}ms n={SampleCount}";
}
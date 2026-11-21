using System;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// Synthetic siege load: posts MoveState coalesce traffic through <see cref="NetMainThreadPump"/>
/// and measures per-frame drain wall time (EditMode / batchmode friendly; full pawn replay is follow-up).
/// </summary>
public static class CrowdNetPumpBench
{
    public static readonly int[] DefaultPawnTiers = { 100, 300, 1000 };

    public const int SimulatedFrames = 120;

    public static IReadOnlyDictionary<int, FineageFrameTimePercentiles> RunSyntheticPumpTiers(int[] pawnTiers)
    {
        if (pawnTiers == null || pawnTiers.Length == 0)
            pawnTiers = DefaultPawnTiers;

        var results = new Dictionary<int, FineageFrameTimePercentiles>(pawnTiers.Length);
        for (int t = 0; t < pawnTiers.Length; t++)
        {
            int pawnCount = pawnTiers[t];
            results[pawnCount] = RunTier(pawnCount);
        }

        return results;
    }

    static FineageFrameTimePercentiles RunTier(int pawnCount)
    {
        var pump = new NetMainThreadPump();
        var frameMs = new List<double>(SimulatedFrames);

        for (int frame = 0; frame < SimulatedFrames; frame++)
        {
            for (int pawn = 0; pawn < pawnCount; pawn++)
            {
                var body = new MoveStateMsg { EntityValue = pawn + 1, DestX = frame, DestY = pawn };
                var anchor = new ZoneApplyPacket { Opcode = NetOpcode.MoveState, Body = body };
                pump.Post(() => { }, NetOpcode.MoveState, body, anchor);
            }

            long start = Stopwatch.GetTimestamp();
            pump.DrainFrame();
            while (pump.Depth > 0)
                pump.DrainFrame();
            double ms = (Stopwatch.GetTimestamp() - start) * 1000.0 / Stopwatch.Frequency;
            frameMs.Add(ms);
        }

        return FineageFrameTimePercentiles.FromFrameTimesMs(frameMs);
    }
}
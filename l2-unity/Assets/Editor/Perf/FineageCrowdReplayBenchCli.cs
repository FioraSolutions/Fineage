using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Unity -batchmode -quit -projectPath l2-unity -executeMethod FineageCrowdReplayBenchCli.RunCli
/// </summary>
public static class FineageCrowdReplayBenchCli
{
    public static void RunCli()
    {
        IReadOnlyDictionary<int, FineageFrameTimePercentiles> tiers =
            CrowdNetPumpBench.RunSyntheticPumpTiers(CrowdNetPumpBench.DefaultPawnTiers);

        foreach (KeyValuePair<int, FineageFrameTimePercentiles> kv in tiers)
            Debug.Log($"[CrowdReplayBench] pawns={kv.Key} {kv.Value}");

        EditorApplication.Exit(0);
    }
}
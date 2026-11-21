using Unity.Profiling;

/// <summary>
/// Shared Profiler markers for client performance roadmap step 1 (audit §7.2).
/// Net markers live in <see cref="NetReceiveProfiling"/>.
/// </summary>
public static class FineagePerfProfiling
{
    public static readonly ProfilerMarker MoveStep = new ProfilerMarker("Fineage.Move.Step");
    public static readonly ProfilerMarker NameplatesUpdate = new ProfilerMarker("Fineage.Nameplates.Update");
    public static readonly ProfilerMarker ActorLightsPush = new ProfilerMarker("Fineage.ActorLights.Push");
    public static readonly ProfilerMarker Spawn = new ProfilerMarker("Fineage.Spawn");
    public static readonly ProfilerMarker TileLoad = new ProfilerMarker("Fineage.Tile.Load");
    public static readonly ProfilerMarker CrowdTier = new ProfilerMarker("Fineage.Crowd.Tier");

    public static readonly string[] RoadmapMarkerNames =
    {
        "Fineage.Net.Read",
        "Fineage.Net.Decode",
        "Fineage.Net.Queue",
        "Fineage.Net.Drain",
        "Fineage.Net.Apply",
        MoveStep.name,
        NameplatesUpdate.name,
        ActorLightsPush.name,
        Spawn.name,
        TileLoad.name,
        CrowdTier.name,
    };
}
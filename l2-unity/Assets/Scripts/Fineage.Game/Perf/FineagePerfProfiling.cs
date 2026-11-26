using Unity.Profiling;

/// <summary>
/// Shared Profiler markers for client performance roadmap step 1 (audit §7.2).
/// Net markers live in <see cref="NetReceiveProfiling"/>.
/// </summary>
public static class FineagePerfProfiling
{
    public const string MoveStepName = "Fineage.Move.Step";
    public const string NameplatesUpdateName = "Fineage.Nameplates.Update";
    public const string ActorLightsPushName = "Fineage.ActorLights.Push";
    public const string SpawnName = "Fineage.Spawn";
    public const string TileLoadName = "Fineage.Tile.Load";
    public const string CrowdTierName = "Fineage.Crowd.Tier";
    public const string FxBudgetName = "Fineage.Fx.Budget";
    public const string GravityStepName = "Fineage.Gravity.Step";
    public const string MoveBurstPlanName = "Fineage.Move.BurstPlan";
    public const string MoveBurstRotateName = "Fineage.Move.BurstRotate";
    public const string GravityRaycastBatchName = "Fineage.Gravity.RaycastBatch";
    public const string MoveNpcStepName = "Fineage.Move.NpcStep";
    public const string MovePawnYawStepName = "Fineage.Move.PawnYaw";
    public const string DecoInstancedCullName = "Fineage.Deco.InstancedCull";

    public static readonly ProfilerMarker MoveStep = new ProfilerMarker(MoveStepName);
    public static readonly ProfilerMarker NameplatesUpdate = new ProfilerMarker(NameplatesUpdateName);
    public static readonly ProfilerMarker ActorLightsPush = new ProfilerMarker(ActorLightsPushName);
    public static readonly ProfilerMarker Spawn = new ProfilerMarker(SpawnName);
    public static readonly ProfilerMarker TileLoad = new ProfilerMarker(TileLoadName);
    public static readonly ProfilerMarker CrowdTier = new ProfilerMarker(CrowdTierName);
    public static readonly ProfilerMarker FxBudget = new ProfilerMarker(FxBudgetName);
    public static readonly ProfilerMarker GravityStep = new ProfilerMarker(GravityStepName);
    public static readonly ProfilerMarker MoveBurstPlan = new ProfilerMarker(MoveBurstPlanName);
    public static readonly ProfilerMarker MoveBurstRotate = new ProfilerMarker(MoveBurstRotateName);
    public static readonly ProfilerMarker GravityRaycastBatch = new ProfilerMarker(GravityRaycastBatchName);
    public static readonly ProfilerMarker MoveNpcStep = new ProfilerMarker(MoveNpcStepName);
    public static readonly ProfilerMarker MovePawnYawStep = new ProfilerMarker(MovePawnYawStepName);
    public static readonly ProfilerMarker DecoInstancedCull = new ProfilerMarker(DecoInstancedCullName);

    public static readonly string[] RoadmapMarkerNames =
    {
        NetReceiveProfiling.ReadName,
        NetReceiveProfiling.DecodeName,
        NetReceiveProfiling.QueueName,
        NetReceiveProfiling.DrainName,
        NetReceiveProfiling.ApplyName,
        MoveStepName,
        NameplatesUpdateName,
        ActorLightsPushName,
        SpawnName,
        TileLoadName,
        CrowdTierName,
        FxBudgetName,
        GravityStepName,
        MoveBurstPlanName,
        MoveBurstRotateName,
        GravityRaycastBatchName,
        MoveNpcStepName,
        MovePawnYawStepName,
        DecoInstancedCullName,
    };
}
using NUnit.Framework;
using Unity.Collections;
using Unity.Mathematics;

public sealed class MoveBurstJobsEditModeTests
{
  [Test]
  public void RemoteMovePlanBurstJob_clamps_last_step_to_target()
  {
    var positions = new NativeArray<float3>(1, Allocator.Temp);
    var targets = new NativeArray<float3>(1, Allocator.Temp);
    var stop = new NativeArray<float>(1, Allocator.Temp);
    var speeds = new NativeArray<float>(1, Allocator.Temp);
    var results = new NativeArray<RemoteMovePlanResult>(1, Allocator.Temp);

    positions[0] = float3.zero;
    targets[0] = new float3(2f, 0f, 0f);
    stop[0] = 0.1f;
    speeds[0] = 10f;

    var job = new RemoteMovePlanBurstJob
    {
        Positions = positions,
        Targets = targets,
        StopDistances = stop,
        Speeds = speeds,
        DeltaTime = 0.5f,
        Results = results,
    };
    job.Execute(0);

    Assert.AreEqual(RemoteMovePlanResult.ActionMove, results[0].Action);
    Assert.AreEqual(1f, results[0].DirX, 0.001f);
    Assert.AreEqual(4f, results[0].EffectiveSpeed, 0.001f);

    positions.Dispose();
    targets.Dispose();
    stop.Dispose();
    speeds.Dispose();
    results.Dispose();
  }

  [Test]
  public void RemoteMovePlanBurstJob_marks_arrival_inside_stop_radius()
  {
    var positions = new NativeArray<float3>(1, Allocator.Temp);
    var targets = new NativeArray<float3>(1, Allocator.Temp);
    var stop = new NativeArray<float>(1, Allocator.Temp);
    var speeds = new NativeArray<float>(1, Allocator.Temp);
    var results = new NativeArray<RemoteMovePlanResult>(1, Allocator.Temp);

    positions[0] = new float3(9.95f, 0f, 0f);
    targets[0] = new float3(10f, 0f, 0f);
    stop[0] = 0.25f;
    speeds[0] = 5f;

    var job = new RemoteMovePlanBurstJob
    {
        Positions = positions,
        Targets = targets,
        StopDistances = stop,
        Speeds = speeds,
        DeltaTime = 0.1f,
        Results = results,
    };
    job.Execute(0);

    Assert.AreEqual(RemoteMovePlanResult.ActionFinished, results[0].Action);

    positions.Dispose();
    targets.Dispose();
    stop.Dispose();
    speeds.Dispose();
    results.Dispose();
  }

  [Test]
  public void PawnRotateStepBurstJob_detects_facing_threshold()
  {
    var monster = new NativeArray<float3>(1, Allocator.Temp);
    var target = new NativeArray<float3>(1, Allocator.Temp);
    var current = new NativeArray<quaternion>(1, Allocator.Temp);
    var thresholds = new NativeArray<float>(1, Allocator.Temp);
    var results = new NativeArray<PawnRotateStepResult>(1, Allocator.Temp);

    monster[0] = float3.zero;
    target[0] = new float3(0f, 0f, 10f);
    current[0] = quaternion.LookRotation(new float3(0f, 0f, 1f), math.up());
    thresholds[0] = 15f;

    var job = new PawnRotateStepBurstJob
    {
        MonsterPositions = monster,
        TargetPositions = target,
        CurrentRotations = current,
        AngleThresholds = thresholds,
        SlerpFactor = 1f,
        Results = results,
    };
    job.Execute(0);

    Assert.IsTrue(results[0].HasStep);
    Assert.IsTrue(results[0].Facing);

    monster.Dispose();
    target.Dispose();
    current.Dispose();
    thresholds.Dispose();
    results.Dispose();
  }
}
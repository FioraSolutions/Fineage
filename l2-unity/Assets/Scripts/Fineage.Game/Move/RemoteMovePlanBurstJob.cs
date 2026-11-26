using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

/// <summary>
/// Horizontal remote move plan (non-CharInfo path) for step 11 — Burst-friendly math
/// matching <see cref="MoveAllCharacters"/> MoveToPoint direction/speed clamp.
/// </summary>
[BurstCompile]
public struct RemoteMovePlanBurstJob : IJobParallelFor
{
  public const float TinyDistSq = 0.0001f;

  [ReadOnly] public NativeArray<float3> Positions;
  [ReadOnly] public NativeArray<float3> Targets;
  [ReadOnly] public NativeArray<float> StopDistances;
  [ReadOnly] public NativeArray<float> Speeds;
  public float DeltaTime;
  public NativeArray<RemoteMovePlanResult> Results;

  public void Execute(int index)
  {
    float3 before = Positions[index];
    float3 target = Targets[index];
    float stopDistance = StopDistances[index];
    float speed = Speeds[index];

    float dx = target.x - before.x;
    float dz = target.z - before.z;
    float distSq = dx * dx + dz * dz;
    float stopSq = stopDistance * stopDistance;

    if (distSq < stopSq)
    {
      Results[index] = RemoteMovePlanResult.Finished();
      return;
    }

    if (distSq <= TinyDistSq)
    {
      Results[index] = RemoteMovePlanResult.NoOp();
      return;
    }

    float dist = math.sqrt(distSq);
    float invDist = 1f / dist;
    float dirX = dx * invDist;
    float dirZ = dz * invDist;

    float stepLength = speed * DeltaTime;
    float effectiveSpeed = speed;
    if (stepLength >= dist)
      effectiveSpeed = dist / DeltaTime;

    Results[index] = RemoteMovePlanResult.Move(dirX, dirZ, effectiveSpeed);
  }
}

public struct RemoteMovePlanResult
{
  public byte Action;
  public float DirX;
  public float DirZ;
  public float EffectiveSpeed;

  public const byte ActionMove = 1;
  public const byte ActionFinished = 2;
  public const byte ActionNoOp = 3;

  public static RemoteMovePlanResult Finished() => new RemoteMovePlanResult { Action = ActionFinished };

  public static RemoteMovePlanResult NoOp() => new RemoteMovePlanResult { Action = ActionNoOp };

  public static RemoteMovePlanResult Move(float dirX, float dirZ, float effectiveSpeed) =>
      new RemoteMovePlanResult
      {
          Action = ActionMove,
          DirX = dirX,
          DirZ = dirZ,
          EffectiveSpeed = effectiveSpeed,
      };
}
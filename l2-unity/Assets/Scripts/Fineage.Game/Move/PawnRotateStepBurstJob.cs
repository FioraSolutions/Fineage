using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

/// <summary>
/// Batch rotate step toward a world target (MoveAllCharacters rotate dict path).
/// </summary>
[BurstCompile]
public struct PawnRotateStepBurstJob : IJobParallelFor
{
  [ReadOnly] public NativeArray<float3> MonsterPositions;
  [ReadOnly] public NativeArray<float3> TargetPositions;
  [ReadOnly] public NativeArray<quaternion> CurrentRotations;
  [ReadOnly] public NativeArray<float> AngleThresholds;
  public float SlerpFactor;
  public NativeArray<PawnRotateStepResult> Results;

  public void Execute(int index)
  {
    float3 monster = MonsterPositions[index];
    float3 target = TargetPositions[index];
    quaternion current = CurrentRotations[index];
    float threshold = AngleThresholds[index];

    float3 toTarget = target - monster;
    toTarget.y = 0f;
    float lenSq = math.lengthsq(toTarget);
    if (lenSq < 0.0001f)
    {
      Results[index] = PawnRotateStepResult.Skip();
      return;
    }

    float3 dir = toTarget * math.rsqrt(lenSq);
    float3 forward = math.mul(current, new float3(0f, 0f, 1f));
    forward.y = 0f;
    float fLenSq = math.lengthsq(forward);
    if (fLenSq > 0.0001f)
      forward *= math.rsqrt(fLenSq);
    else
      forward = new float3(0f, 0f, 1f);

    float cos = math.clamp(math.dot(forward, dir), -1f, 1f);
    float angleDeg = math.degrees(math.acos(cos));
    bool facing = angleDeg < threshold;

    quaternion look = quaternion.LookRotationSafe(dir, math.up());
    float t = math.saturate(SlerpFactor);
    quaternion next = math.slerp(current, look, t);

    Results[index] = new PawnRotateStepResult
    {
        HasStep = true,
        Facing = facing,
        Rotation = next,
    };
  }
}

public struct PawnRotateStepResult
{
  public bool HasStep;
  public bool Facing;
  public quaternion Rotation;

  public static PawnRotateStepResult Skip() => default;
}
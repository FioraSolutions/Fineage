using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct AssignCrowdTierJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<float3> Positions;
    public float3 Observer;
    public float Tier1MaxSq;
    public float Tier2MaxSq;
    public float Tier3MaxSq;
    public float PawnDrawMaxSq;
    public NativeArray<byte> Tiers;

    public void Execute(int index)
    {
        float3 delta = Positions[index] - Observer;
        delta.y = 0f;
        float distSq = math.lengthsq(delta);

        if (PawnDrawMaxSq > 0f && distSq > PawnDrawMaxSq)
        {
            Tiers[index] = (byte)CrowdDetailTier.TCulled;
            return;
        }

        byte tier = (byte)CrowdDetailTier.T3Far;
        if (distSq <= Tier1MaxSq)
            tier = (byte)CrowdDetailTier.T1Near;
        else if (distSq <= Tier2MaxSq)
            tier = (byte)CrowdDetailTier.T2Mid;
        else if (distSq <= Tier3MaxSq)
            tier = (byte)CrowdDetailTier.T3Far;
        else
            tier = (byte)CrowdDetailTier.TCulled;
        Tiers[index] = tier;
    }
}
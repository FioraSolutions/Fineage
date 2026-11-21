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
    public NativeArray<byte> Tiers;

    public void Execute(int index)
    {
        float distSq = math.distancesq(Positions[index], Observer);
        byte tier = (byte)CrowdDetailTier.T3Far;
        if (distSq <= Tier1MaxSq)
            tier = (byte)CrowdDetailTier.T1Near;
        else if (distSq <= Tier2MaxSq)
            tier = (byte)CrowdDetailTier.T2Mid;
        else if (distSq <= Tier3MaxSq)
            tier = (byte)CrowdDetailTier.T3Far;
        Tiers[index] = tier;
    }
}
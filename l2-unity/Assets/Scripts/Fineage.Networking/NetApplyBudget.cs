/// <summary>Shared main-thread zone-apply budget (roadmap step 9). Count and time ceilings both apply.</summary>
public static class NetApplyBudget
{
    /// <summary>Soft ceiling for non-preserve apply work per frame (audit §1.1).</summary>
    public const double MaxNormalApplyMillisecondsPerFrame = 4.0;
}
using UnityEngine;

[CreateAssetMenu(fileName = "CrowdLodProfile", menuName = "Fineage/Crowd LOD Profile")]
public sealed class CrowdLodProfile : ScriptableObject
{
    [Tooltip("Within this distance (meters): T0 for local player party; others start at T1.")]
    public float tier1MaxDistanceMeters = 25f;

    [Tooltip("Beyond tier1 and within this distance: T2.")]
    public float tier2MaxDistanceMeters = 60f;

    [Tooltip("Beyond tier2 and within this distance: T3.")]
    public float tier3MaxDistanceMeters = 120f;

    [Tooltip("Nameplates for entities at or above this tier are hidden (step 13 hooks nameplates/FX).")]
    public CrowdDetailTier hideNameplateFromTier = CrowdDetailTier.T3Far;

    [Header("Animator (step 14)")]
    public float tier0AnimatorSpeed = 1f;
    public float tier1AnimatorSpeed = 1f;
    public float tier2AnimatorSpeed = 0.5f;
    public float tier3AnimatorSpeed = 0.25f;
    public CrowdDetailTier animatorCullCompletelyFromTier = CrowdDetailTier.T3Far;

    public float Tier1MaxSq => tier1MaxDistanceMeters * tier1MaxDistanceMeters;
    public float Tier2MaxSq => tier2MaxDistanceMeters * tier2MaxDistanceMeters;
    public float Tier3MaxSq => tier3MaxDistanceMeters * tier3MaxDistanceMeters;

    public bool ShouldShowNameplate(CrowdDetailTier tier) => tier < hideNameplateFromTier;

    public float AnimatorSpeedForTier(CrowdDetailTier tier)
    {
        switch (tier)
        {
            case CrowdDetailTier.T0Hero: return tier0AnimatorSpeed;
            case CrowdDetailTier.T1Near: return tier1AnimatorSpeed;
            case CrowdDetailTier.T2Mid: return tier2AnimatorSpeed;
            default: return tier3AnimatorSpeed;
        }
    }

    public AnimatorCullingMode AnimatorCullingForTier(CrowdDetailTier tier)
    {
        return tier >= animatorCullCompletelyFromTier
            ? AnimatorCullingMode.CullCompletely
            : AnimatorCullingMode.CullUpdateTransforms;
    }
}
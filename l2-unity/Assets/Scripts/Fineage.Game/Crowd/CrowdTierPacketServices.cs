public static class CrowdTierPacketServices
{
    public static CrowdDetailTier GetTier(int entityId) => VisibilityTierRegistry.GetTier(entityId);

    public static bool ShouldShowNameplate(int entityId) => VisibilityTierRegistry.ShouldShowNameplate(entityId);
}
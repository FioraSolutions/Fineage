using UnityEngine;

public static class CrowdTierPacketServices
{
    public static CrowdTierSystem System { get; private set; }

    public static void Bind(CrowdTierSystem system) => System = system;

    public static CrowdDetailTier GetTier(int entityId) =>
        System != null ? System.GetTier(entityId) : CrowdDetailTier.T1Near;

    public static bool ShouldShowNameplate(int entityId) =>
        System == null || System.ShouldShowNameplate(entityId);
}
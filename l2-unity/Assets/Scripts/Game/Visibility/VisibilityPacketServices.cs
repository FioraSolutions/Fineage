using UnityEngine;

public static class VisibilityPacketServices
{
    static VisibilityManager _manager;

    public static IVisibilityRoster Manager => _manager;

    public static void Bind(VisibilityManager manager) => _manager = manager;

    public static CrowdDetailTier GetTier(int entityId) =>
        _manager != null ? _manager.GetTier(entityId) : CrowdDetailTier.T1Near;

    public static bool ShouldShowNameplate(int entityId) =>
        _manager == null || _manager.ShouldShowNameplate(entityId);

    public static bool ShouldPlayWorldAudio(int entityId, Vector3 position) =>
        _manager == null || _manager.ShouldPlayWorldAudio(entityId, position);

    public static bool ShouldPlayNpcVoice(int entityId, NpcVoicePriority priority) =>
        _manager == null || _manager.ShouldPlayNpcVoice(entityId, priority);

    public static void NotifyNpcVoiceEnded(int entityId) => _manager?.NotifyNpcVoiceEnded(entityId);

    public static float NameplateViewDistanceMeters =>
        _manager != null ? _manager.NameplateViewDistanceMeters : InterludeVisibilityOptions.NameplateViewDistanceMeters;
}
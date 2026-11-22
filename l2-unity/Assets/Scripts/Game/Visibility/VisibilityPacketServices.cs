using UnityEngine;

public static class VisibilityPacketServices
{
    public static VisibilityManager Manager { get; private set; }

    public static void Bind(VisibilityManager manager) => Manager = manager;

    public static CrowdDetailTier GetTier(int entityId) =>
        Manager != null ? Manager.GetTier(entityId) : CrowdDetailTier.T1Near;

    public static bool ShouldShowNameplate(int entityId) =>
        Manager == null || Manager.ShouldShowNameplate(entityId);

    public static bool ShouldPlayWorldAudio(int entityId, Vector3 position) =>
        Manager == null || Manager.ShouldPlayWorldAudio(entityId, position);

    public static bool ShouldPlayNpcVoice(int entityId, NpcVoicePriority priority) =>
        Manager == null || Manager.ShouldPlayNpcVoice(entityId, priority);

    public static void NotifyNpcVoiceEnded(int entityId) => Manager?.NotifyNpcVoiceEnded(entityId);

    public static float NameplateViewDistanceMeters =>
        Manager != null ? Manager.NameplateViewDistanceMeters : InterludeVisibilityOptions.NameplateViewDistanceMeters;
}
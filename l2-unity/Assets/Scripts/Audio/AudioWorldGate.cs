using UnityEngine;

/// <summary>
/// Client hear range for world SFX — Interlude-style ~28m, not global idle/combat spam.
/// </summary>
public static class AudioWorldGate
{
    public static bool TryGetListenerPosition(out Vector3 position)
    {
        ThirdPersonListener listener = AudioPacketServices.Listener;
        if (listener != null && listener.Player != null)
        {
            position = listener.Player.transform.position;
            return true;
        }

        PlayerController player = ControllerPacketServices.Player;
        if (player != null)
        {
            position = player.transform.position;
            return true;
        }

        Entity local = DomainPacketServices.LocalPlayer;
        if (local != null && local.transform != null)
        {
            position = local.transform.position;
            return true;
        }

        Camera cam = Camera.main;
        if (cam != null)
        {
            position = cam.transform.position;
            return true;
        }

        position = default;
        return false;
    }

    public static float DistanceToListener(Vector3 worldPosition)
    {
        if (!TryGetListenerPosition(out Vector3 listener))
            return float.MaxValue;

        Vector3 d = worldPosition - listener;
        d.y = 0f;
        return d.magnitude;
    }

    public static bool IsWithinHearRange(Vector3 worldPosition) =>
        DistanceToListener(worldPosition) <= AudioWorldConstants.HearMaxMeters;

    public static bool ShouldPlayWorldSound(Vector3 worldPosition)
    {
        if (!TryGetListenerPosition(out _))
            return true;

        return IsWithinHearRange(worldPosition);
    }
}
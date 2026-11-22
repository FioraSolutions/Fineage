/// <summary>
/// UE2 PlaySound audibility (UDN Sound Reference, engine v2226, clarified 2003-11-10).
/// Authored Radius is the inner plateau in L2 unreal units. The sound is cut off at Radius * 100.
/// Falloff: volume_drop_db = 20 * log10(1 + Rolloff * (distance - Radius) / Radius),
/// which is attenuation = 1 / (1 + Rolloff * (distance - Radius) / Radius).
/// Rolloff is ALAudioSubsystem.Rolloff from the UE2 reference ini (0.5). L2's l2.ini is not in the dump.
/// </summary>
public static class Ue2SoundAudibility
{
    public const float OuterRadiusScale = 100f;
    public const float AlAudioDefaultRolloff = 0.5f;

    /// <summary>UE2 ESoundSlot. SLOT_None mixes. Other slots hold one sound per actor.</summary>
    public const int SlotNone = 0;
    public const int SlotMisc = 1;
    public const int SlotPain = 2;
    public const int SlotInteract = 3;
    public const int SlotAmbient = 4;
    public const int SlotTalk = 5;
    public const int SlotInterface = 6;

    public static bool HasRadius(float innerRadiusUu) =>
        !float.IsNaN(innerRadiusUu) && !float.IsInfinity(innerRadiusUu) && innerRadiusUu > 0f;

    public static string FormatMissingRadius(string id) =>
        "AudioService: missing audible radius for \"" + (id ?? "")
        + "\" (no AnimNotify Radius, SoundRadius, or emitter radius).";

    public static float InnerRadiusMeters(float innerRadiusUu) =>
        innerRadiusUu * VectorUtils.L2UuToUnity;

    public static float OuterRadiusMeters(float innerRadiusUu) =>
        InnerRadiusMeters(innerRadiusUu) * OuterRadiusScale;

    public static bool IsAudible(float distanceMeters, float innerRadiusUu)
    {
        if (!HasRadius(innerRadiusUu) || distanceMeters < 0f)
            return false;
        return distanceMeters < OuterRadiusMeters(innerRadiusUu);
    }

    public static float Attenuation(float distanceMeters, float innerRadiusUu)
    {
        if (!HasRadius(innerRadiusUu))
            return 0f;
        float inner = InnerRadiusMeters(innerRadiusUu);
        if (distanceMeters <= inner)
            return 1f;
        float outer = inner * OuterRadiusScale;
        if (distanceMeters >= outer)
            return 0f;
        float drop = 1f + AlAudioDefaultRolloff * (distanceMeters - inner) / inner;
        return 1f / drop;
    }

    /// <summary>UE2 voice priority: louder and closer wins. Equal priority does not steal.</summary>
    public static float Priority(float volumeScale, float distanceMeters, float innerRadiusUu) =>
        volumeScale * Attenuation(distanceMeters, innerRadiusUu);

    public static int SlotForNotifyClass(string className)
    {
        if (className == "AnimNotify_AttackVoice")
            return SlotTalk;
        return SlotMisc;
    }
}

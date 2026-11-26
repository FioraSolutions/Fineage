/// <summary>
/// UE2 PlaySound and Actor.SoundRadius (UDN Sound Reference, engine v2226, 2003-11-10).
/// The authored Radius / SoundRadius is the inner plateau, and it is 1/100 of the
/// cutoff in unreal units: the sound is inaudible at Radius * 100.
/// Ambient SoundRadius uses that same scale ("the radius is 1/100 of the radius of
/// the sound in Unreal Units"); only the edge transition differs from PlaySound.
/// Falloff: volume_drop_db = 20 * log10(1 + Rolloff * (distance - Radius) / Radius),
/// which is attenuation = 1 / (1 + Rolloff * (distance - Radius) / Radius).
/// Rolloff is ALAudioSubsystem.Rolloff from the UE2 reference ini (0.5). L2's l2.ini is not in the dump.
/// The Interlude client ships ALAudio (UALAudioSubsystem). Engine.u SoundRadius is a
/// FloatProperty, so this is the UE2 float radius, not the UE1 Galaxy byte radius
/// whose world radius was (SoundRadius + 1) * 25.
/// </summary>
public static class Ue2SoundAudibility
{
    public const float OuterRadiusScale = 100f;
    public const float AlAudioDefaultRolloff = 0.5f;

    /// <summary>
    /// UE2 Actor.uc defaultproperties SoundRadius=32. AmbientSound does not override it
    /// (UE2Runtime AmbientSound defaults change SoundVolume only). Map actors that omit
    /// SoundRadius inherit this. The ambient table stores 0 for that omission.
    /// L2 Engine.u class-default object is not in the catalog.
    /// </summary>
    public const float ActorDefaultSoundRadiusUu = 32f;

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

    /// <summary>
    /// Actor/AmbientSound SoundRadius. An omitted property is stored as 0 and uses
    /// <see cref="ActorDefaultSoundRadiusUu"/>. A positive authored value is kept.
    /// </summary>
    public static float ResolveActorSoundRadiusUu(float authoredUu) =>
        HasRadius(authoredUu) ? authoredUu : ActorDefaultSoundRadiusUu;

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

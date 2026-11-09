using UnityEngine;

[CreateAssetMenu(fileName = "LightEffectSetting", menuName = "VFX/Settings/LightEffect")]
public class LightEffectSetting : ScriptableObject
{
    [Header("Look")]
    public Color color = Color.white;
    [Tooltip("Peak Light.intensity")]
    public float intensity = 0.6f;
    public float durationSeconds = 0.4f;
    public float rangeMeters = 2f;
    public float spotAngle = 71.5f;
    public float innerSpotAngle = 32.5f;

    [Header("Attach")]
    [Tooltip(
        "Combat subject to light.\n" +
        "Caster = attacker (player).\n" +
        "Target = victim (NPC) — soulshot flash on dog.\n" +
        "HitPoint = SetImpactHit world point only.\n" +
        "VFX spawning on the NPC does NOT mean Attach=Caster.")]
    public LightAttachSubject attachSubject = LightAttachSubject.Target;

    [Tooltip("Place on attacker-facing side of capsule (Location - dir * radius * scale).")]
    public bool useFaceOffset = true;
    public float faceOffsetRadiusScale = 2f;
}

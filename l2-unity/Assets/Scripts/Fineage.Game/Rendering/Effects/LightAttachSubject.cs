/// <summary>
/// Who to illuminate — combat roles, NOT "where the VFX prefab sits".
/// Example: player hits a dog → player = Caster, dog = Target.
/// Soulshot impact FX also appears on the dog, but that does not make the dog Caster.
/// </summary>
public enum LightAttachSubject
{
    /// <summary>Attacker / skill user (e.g. player).</summary>
    Caster = 0,

    /// <summary>Victim being hit (e.g. NPC/dog). Use this for shot_N_atk hit flash.</summary>
    Target = 1,

    /// <summary>Raw impact point from SetImpactHit (HitPointProxy path), no Entity required.</summary>
    HitPoint = 2
}

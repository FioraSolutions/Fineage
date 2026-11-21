/// <summary>
/// Skill DAT accessors for abnormal tooltips; wired from Assembly-CSharp at HUD load (#1598–#1602).
/// </summary>
public static class AbnormalEffectTooltipTables
{
    public static ISkillgrpLookup SkillGrps { get; set; }

    public static ISkillNameTable SkillNames { get; set; }
}
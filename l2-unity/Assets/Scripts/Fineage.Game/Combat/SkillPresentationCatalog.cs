using System.Collections.Generic;

/// <summary>Per-skill presentation flags (audit batch 8); replace scattered SkillId literals.</summary>
public static class SkillPresentationCatalog
{
    public const int WindStrikeId = 4247;
    public const int TwistDoomId = 2046;

    static readonly HashSet<int> WindStrikeGatedSkills = new HashSet<int> { WindStrikeId };

    public static bool IsWindStrike(int skillId) => WindStrikeGatedSkills.Contains(skillId);

    public static bool IsTwistDoomPhysical(int skillId) => skillId == TwistDoomId;
}

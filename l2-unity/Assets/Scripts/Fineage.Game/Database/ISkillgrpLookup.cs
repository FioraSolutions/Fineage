/// <summary>
/// Cycle-safe skillgrp DAT lookup for Di/UI (#131).
/// </summary>
public interface ISkillgrpLookup
{
    Skillgrp GetSkill(int id, int level);
}

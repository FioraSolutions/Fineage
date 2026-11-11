/// <summary>
/// Cycle-safe skill name DAT lookup for Di/UI (#131).
/// </summary>
public interface ISkillNameTable
{
    SkillNameData GetName(int id, int level);
}

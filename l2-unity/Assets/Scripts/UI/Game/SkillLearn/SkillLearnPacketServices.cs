/// <summary>
/// Skill learn UI access to game services with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 21).
/// </summary>
public static class SkillLearnPacketServices
{
    private static SkillLearnWindow _skillLearn;
    private static DescriptionSkillWindow _skillDesc;
    private static SkillListWindow _skillList;

    public static void Bind(
        SkillLearnWindow skillLearn = null,
        DescriptionSkillWindow skillDesc = null,
        SkillListWindow skillList = null)
    {
        if (skillLearn != null)
            _skillLearn = skillLearn;
        if (skillDesc != null)
            _skillDesc = skillDesc;
        if (skillList != null)
            _skillList = skillList;
    }

    public static SkillLearnWindow SkillLearn =>
        _skillLearn != null ? _skillLearn : IncomingPacketActions.SkillLearn;

    public static DescriptionSkillWindow SkillDesc =>
        _skillDesc != null ? _skillDesc : IncomingPacketActions.SkillDesc;

    public static SkillListWindow SkillList =>
        _skillList != null ? _skillList : IncomingPacketActions.SkillList;
}

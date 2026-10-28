using System;

/// <summary>
/// Skill learn/list UI apply hub (#142 batch slice 161; UI hub slice 167).
/// </summary>
public static class SkillApplyIncomingPacketServices
{
    public static SkillLearnWindow SkillLearn => UiApplyIncomingPacketServices.SkillLearn;

    public static DescriptionSkillWindow SkillDesc => UiApplyIncomingPacketServices.SkillDesc;

    public static SkillListWindow SkillList => UiApplyIncomingPacketServices.SkillList;

    public static void Queue(Action action) => UiApplyIncomingPacketServices.Queue(action);
}

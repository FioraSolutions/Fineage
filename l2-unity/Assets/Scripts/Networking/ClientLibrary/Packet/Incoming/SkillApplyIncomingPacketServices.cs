using System;

/// <summary>
/// Skill learn/list UI apply hub (#142 batch slice 161; locator slice 170).
/// </summary>
public static class SkillApplyIncomingPacketServices
{
    public static SkillLearnWindow SkillLearn => LocatorApplyIncomingPacketServices.SkillLearn;

    public static DescriptionSkillWindow SkillDesc => LocatorApplyIncomingPacketServices.SkillDesc;

    public static SkillListWindow SkillList => LocatorApplyIncomingPacketServices.SkillList;

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);
}

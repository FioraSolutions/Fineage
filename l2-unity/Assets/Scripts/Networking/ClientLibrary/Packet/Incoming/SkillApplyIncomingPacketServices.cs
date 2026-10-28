using System;

/// <summary>
/// Skill learn/list UI apply hub (#142 batch slice 161; locator slice 170).
/// </summary>
public static class SkillApplyIncomingPacketServices
{
    public static SkillLearnWindow SkillLearn => IncomingPacketLocatorApplyIncomingPacketServices.SkillLearn;

    public static DescriptionSkillWindow SkillDesc => IncomingPacketLocatorApplyIncomingPacketServices.SkillDesc;

    public static SkillListWindow SkillList => IncomingPacketLocatorApplyIncomingPacketServices.SkillList;

    public static void Queue(Action action) =>
        IncomingPacketQueueApplyIncomingPacketServices.Queue(action);
}

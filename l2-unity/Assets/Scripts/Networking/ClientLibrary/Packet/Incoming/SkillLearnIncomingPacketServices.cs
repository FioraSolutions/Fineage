using System;

/// <summary>
/// Skill learn / skill list UI incoming handler access via UiIncomingPacketServices (#142 sub-slice 69).
/// </summary>
public static class SkillLearnIncomingPacketServices
{
    public static SkillLearnWindow SkillLearn => UiIncomingPacketServices.SkillLearn;

    public static DescriptionSkillWindow SkillDesc => UiIncomingPacketServices.SkillDesc;

    public static SkillListWindow SkillList => UiIncomingPacketServices.SkillList;

    public static void Queue(Action action) => UiIncomingPacketServices.Queue(action);

    public static void ApplyAcquireSkillInfo(AcquireSkillInfoDto packet) =>
        Queue(() =>
        {
            SkillLearn.HideWindow();
            SkillDesc.AddData(packet);
            SkillDesc.ShowWindow();
        });

    public static void ApplyAcquireSkillList(AcquireSkillListDto packet) =>
        Queue(() =>
        {
            SkillLearn.AddData(packet.AcquireList);
            SkillLearn.ShowWindow();
        });

    public static void ApplySkillList(SkillListDto packet) =>
        Queue(() => SkillList.UpdateSkillList(packet.Skills));

    public static void ApplySkillCoolTime(SkillCoolTimeDto packet)
    {
    }
}

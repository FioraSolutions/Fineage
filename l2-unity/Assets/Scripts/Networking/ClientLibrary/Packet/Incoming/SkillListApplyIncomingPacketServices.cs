using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Skill learn wire packet handler entry (#142 batch slice 139).
/// Splits apply out of <see cref="SkillLearnIncomingPacketServices"/> (sub-slice 69).
/// Apply facade (#142 batch slice 154; locator queue slice 259). Re-homes from <see cref="SkillListIncomingPacketServices"/>.
/// </summary>
public static class SkillListApplyIncomingPacketServices
{
    public static SkillListWindow SkillList => LocatorApplyIncomingPacketServices.SkillList;

    public static void Queue(System.Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplySkillList(SkillListDto packet) =>
        Queue(() =>
        {
            SkillListWindow window = SkillLearnPacketServices.SkillList;
            if (window == null)
                window = SkillList;
            if (window == null)
            {
                int n = packet?.Skills != null ? packet.Skills.Count : 0;
                Debug.LogError(
                    "[SkillList] SkillListApplyIncomingPacketServices: window null (count=" + n + ").");
                SkillListWindow.StashOrphanedSkillList(packet);
                return;
            }

            window.UpdateSkillList(packet.Skills);
        });
}

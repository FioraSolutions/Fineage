using VContainer;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Skill list and magic skill use applies, constructed by the container instead of reached
/// through static facades (#142 slice 3). <see cref="SkillListWindow"/> arrives through
/// <see cref="ILateBound{T}"/> (#142 slice 4).
/// </summary>
public sealed class SkillApplyService
{
    private readonly ILateBound<SkillListWindow> _skillList;

    [Inject]
    public SkillApplyService(ILateBound<SkillListWindow> skillList)
    {
        _skillList = skillList;
    }

    public void ApplySkillList(SkillListDto packet) =>
        QueueApplyIncomingPacketServices.Queue(() =>
        {
            SkillListWindow window = ResolveSkillListWindow();
            if (window == null)
            {
                int n = packet?.Skills != null ? packet.Skills.Count : 0;
                Debug.LogError(
                    "[SkillList] ApplySkillList dropped: SkillListWindow not in scene yet (count=" + n + ").");
                SkillListWindow.StashOrphanedSkillList(packet);
                return;
            }

            window.UpdateSkillList(packet.Skills);
        });

    SkillListWindow ResolveSkillListWindow()
    {
        SkillListWindow window = _skillList != null ? _skillList.Value : null;
        if (window != null)
            return window;
        window = SkillLearnPacketServices.SkillList;
        if (window != null)
            return window;
        return SkillListApplyIncomingPacketServices.SkillList;
    }

    public void ApplyMagicSkillUse(MagicSkillUseDto packet)
    {
        // Temporary: skill cast/VFX hides CharInfo spawn cost in packetApply.txt.
        // Packet is still read, so the stream stays aligned.
        if (packet == null)
            return;

        QueueApplyIncomingPacketServices.QueueWorld(apply => apply.MagicSkillUse(packet));
    }
}

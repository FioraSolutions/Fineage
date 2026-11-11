using UnityEngine;

public partial class SkillInstance : AbstractSkill
{
    public double GetReuseTime()
    {
        return TablePacketServices.SkillGrps.GetSkill(SkillID, Level).ReuseDelay;
    }

    public double GetHitTime()
    {
        return TablePacketServices.SkillGrps.GetSkill(SkillID, Level).HitTime;
    }

    public bool IsMagic()
    {
        var skill = TablePacketServices.SkillGrps.GetSkill(SkillID, Level);
        if (skill == null)
            return false;
        return skill.IsMagic == 1;
    }

    public string Icon()
    {
        return TablePacketServices.SkillGrps.GetSkill(SkillID, Level)?.Icon ?? "";
    }

    public string GetName()
    {
        return TablePacketServices.SkillNames.GetName(SkillID, Level)?.Name ?? "";
    }

    public string GetDescription()
    {
        return TablePacketServices.SkillNames.GetName(SkillID, Level)?.Desc ?? "";
    }
}

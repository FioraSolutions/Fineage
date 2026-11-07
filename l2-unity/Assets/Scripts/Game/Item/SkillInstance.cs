using UnityEngine;

public class SkillInstance : AbstractSkill
{
    public SkillInstance(int pId, int pLevel, bool pPassive, bool pDisabled)
    {
        SkillID = pId;
        Level = pLevel;
        IsPassive = pPassive;
        IsDisabled = pDisabled;
    }

    public void RefreshData(int pId, int pLevel, bool pPassive, bool pDisabled)
    {
        SkillID = pId;
        Level = pLevel;
        IsPassive = pPassive;
        IsDisabled = pDisabled;
    }

    public string GetTypeName()
    {
        return IsPassive ? "Passive Skill" : "Active Skill";
    }

    public int GetHp()
    {
        return TablePacketServices.SkillGrps.GetSkill(SkillID, Level).HpConsume;
    }

    public int GetMp()
    {
        return TablePacketServices.SkillGrps.GetSkill(SkillID, Level).MpConsume;
    }

    public int GetRange()
    {
        return TablePacketServices.SkillGrps.GetSkill(SkillID, Level).CastRange;
    }
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
        return SkillNameTable.Instance.GetName(SkillID, Level)?.Name ?? "";
    }

    public string GetDescription()
    {
        return SkillNameTable.Instance.GetName(SkillID, Level)?.Desc ?? "";
    }


}

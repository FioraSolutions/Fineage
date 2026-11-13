
using UnityEngine;

public partial class SkillInstance : AbstractSkill
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
        SkillPassiveRules.Refresh(this);
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
}

using System;

public partial class L2GameUI
{
    public bool AreWindowsReady()
    {
        return UILoaded
            && ClanPacketServices.Clan != null && ClanPacketServices.Clan.IsReady
            && SkillLearnPacketServices.SkillList != null && SkillLearnPacketServices.SkillList.IsReady;
    }
}

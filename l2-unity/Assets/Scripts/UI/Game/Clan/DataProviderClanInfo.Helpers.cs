using Org.BouncyCastle.Bcpg;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class DataProviderClanInfo : AbstractDataFunction
{
    public void SetMemberInfo(VisualElement container, PledgeReceiveMemberInfoDto packet ,  PledgeShowMemberListAllDto packetAll)
    {
        Label titleName = container.Q<Label>("titleLabel");
        AddElementIfNotEmpty(titleName, titleName, packet.Name);

        Label titlePlayerName = container.Q<Label>("titlePlayerLabel");
        AddElementIfNotEmpty(titlePlayerName, titlePlayerName, packet.Title);

        string memberRank = packetAll.SubPledgeLeaderName == packet.Name? "Leader" : "Member";
 

        Label rankLabel = container.Q<Label>("rankLabel");
        AddElementIfNotEmpty(rankLabel, rankLabel, memberRank);


        Label statuslabel = container.Q<Label>("statusLabel");
        AddElementIfNotEmpty(statuslabel, statuslabel, "Main Clan - " + packet.SubPledgeName);
    }

    public void SetSubjectClanInfo(VisualElement container , PledgeShowMemberListAllDto packetAll)
    {
        Label titleName = container.Q<Label>("titleLabel");
        AddElementIfNotEmpty(titleName, titleName, packetAll.PledgeName);
    }
}

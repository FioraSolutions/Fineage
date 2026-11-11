using Org.BouncyCastle.Bcpg;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class DataProviderClanInfo : AbstractDataFunction
{
    public void SetMasterClanInfo(VisualElement container, PledgeShowMemberListAllDto packet)
    {
        if (container != null)
        {
            Label leaderLabel = container.Q<Label>("labelLeaderName");
            AddElementIfNotEmpty(leaderLabel, leaderLabel, packet.SubPledgeLeaderName);

            Label titleLabel = container.Q<Label>("labelClanName");
            AddElementIfNotEmpty(titleLabel, titleLabel, packet.PledgeName);


            Label lvllabel = container.Q<Label>("lvlName");
            AddElementIfNotEmpty(lvllabel, lvllabel, packet.Level.ToString());

            int countOnline = 0;
            int allOnline = 0;

            if (packet.Members != null)
            {
                 countOnline = packet.Members.Count(x => x.Online > 0);
                 allOnline = packet.Members.Count;
            }

            Label onlineLabel = container.Q<Label>("labelCountOnline");
            AddElementIfNotEmpty(onlineLabel, onlineLabel, "(" + countOnline + "/"+ allOnline + ")");

            var icon = container.Q<VisualElement>("ImageOnline");

            AddResourcesIconIfPresent(
                icon,
                icon,
                countOnline == 0 ? IMG_OFFLINE : IMG_ONLINE);


        }
    }

    public void UpdateClanInfo(VisualElement container, PledgeInfoDto packet)
    {
        Label titleLabel = container.Q<Label>("labelClanName");
        AddElementIfNotEmpty(titleLabel, titleLabel, packet.ClanName);
    }
}

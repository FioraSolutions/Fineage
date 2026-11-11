using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class MasterClan
{
    private void EnsureMinimumRowCount(List<string> names, List<string> levels,
    List<string> roles, List<string> activities, int minimumCount)
    {
        int currentCount = names.Count;
        if (currentCount >= minimumCount) return;

        int emptySlots = minimumCount - currentCount;
        for (int i = 0; i < emptySlots; i++)
        {
            names.Add("");
            levels.Add("");
            roles.Add("");
            activities.Add("");
        }
    }

    private void SetOfflineMemberColors(List<ClanMember> members, List<TableColumn> columns)
    {
        UserInfoDto usrInfo = StorageNpc.getInstance().GetFirstUser();
        string selfName = usrInfo != null && usrInfo.PlayerInfoInterlude.Identity != null
            ? usrInfo.PlayerInfoInterlude.Identity.Name
            : null;
        for (int i=0; i < members.Count; i++)
        {
            ClanMember member = members[i];
            if(selfName != null && selfName != member.MemberName)
            {
                 columns[0].SetColor(i + member.MemberName, DEFAULT_OFFLINE_COLOR);
                 columns[1].SetColor(i + member.Level.ToString(), DEFAULT_OFFLINE_COLOR);
            }
            //if (member.Online == 0)
            //{
            // columns[0].SetColor(i + member.MemberName, "#B3B2B2");
            // columns[1].SetColor(i + member.Level.ToString(), "#B3B2B2");
            //}

        }
    }

    private void SetOnline(ClanMember member, List<string> activity)
    {
        if (member.Online == 0)
        {
            activity.Add(MEMBER_OFFLINE);

        }
        else
        {
            activity.Add(MEMBER_ONLINE);
        }
    }

    public string GetOnline(int online)
    {
        return (online >= 1) ? MEMBER_ONLINE : MEMBER_OFFLINE;
    }
}

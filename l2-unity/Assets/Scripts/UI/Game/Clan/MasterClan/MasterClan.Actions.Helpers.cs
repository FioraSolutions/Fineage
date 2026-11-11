using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class MasterClan
{
    private string  GetOnlineHexColors(int online)
    {
        return (online >= 1) ? "" : "#B3B2B2";
    }

    private List<TableColumn> GetColumns(List<string> namesList , List<string> lvlList, List<string> roleList, List<string> activity)
    {
        var name = new TableColumn(false, "Name", 100, namesList, 13);
        var lvl = new TableColumn(true, "Lv.", 0, lvlList, 0);
        var role = new TableColumn(true, "Role", 15, roleList, 0, true);
        var act = new TableColumn(true, "Activity.", 20, activity, 0, true);

        return new List<TableColumn> { name, lvl, role, act };
    }

    public void DeleteAllMember(ICreatorTables creatorTableWindows)
    {
        creatorTableWindows.ClearTable();
    }

    public void UpdateMemberData(ClanMember clanMember, PledgeShowMemberListAllDto packet , ICreatorTables creatorTableWindows)
    {
        if (packet != null && clanMember != null)
        {

            var memberToUpdate = packet.Members.FirstOrDefault(m => m.MemberName == clanMember.MemberName);

            if (memberToUpdate != null)
            {

                memberToUpdate.MemberName = clanMember.MemberName;
                memberToUpdate.Level = clanMember.Level;
                memberToUpdate.ClassId = clanMember.ClassId;
                memberToUpdate.Sex = clanMember.Sex;
                memberToUpdate.Race = clanMember.Race;
                memberToUpdate.Online = clanMember.Online;
                //memberToUpdate. = packetUpdate.PledgeType;
                //memberToUpdate.HasSponsor = packetUpdate.HasSponsor;

                UpdateMembersTable(clanMember, creatorTableWindows);

               // CreateMembersTable(packet.Members, creatorTableWindows);
            }
        }
    }

    public void SelectMember(int selectIndex, string select_text)
    {

        if (string.IsNullOrEmpty(select_text))
        {
            // Debug.LogWarning("Input string is null or empty");
            return;
        }

        string[] row_text = select_text.Split("___");


        if (row_text.Length < 2)
        {
            // Debug.LogWarning($"Invalid input format. Expected at least 2 parts separated by '___', but got {row_text.Length} parts");
            return;
        }


        if (string.IsNullOrEmpty(row_text[1]))
        {
            // Debug.LogWarning("Second part of the split string is empty");
            return;
        }


        _table_select_name_member = row_text[1];
    }
}

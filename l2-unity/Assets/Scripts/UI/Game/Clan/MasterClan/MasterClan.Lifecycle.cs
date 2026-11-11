using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class MasterClan
{
    public string GetSelectMemberName()
    {
        return _table_select_name_member;
    }

    public void ForEachClan( ICreatorTables _creatorTableWindows)
    {
        List<TableColumn> listTableColumn = GetEmptyData();
        _creatorTableWindows.CreateTable(listTableColumn);
    }

    public List<TableColumn> GetEmptyData()
    {
        var namesList = new List<string>();
        var conditionsList = new List<string>();
        var levelList = new List<string>();
        var activity = new List<string>();


        for (int i = 0; i < 7; i++)
        {
            namesList.Add("");
            conditionsList.Add("");
            levelList.Add("");
            activity.Add("");
        }

        return GetColumns(namesList, conditionsList, levelList, activity);
    }

    public List<string> SetDropdownList(DropdownField dropdown , string clanName)
    {
        if (string.IsNullOrEmpty(clanName))
        {
            // UI Toolkit throws ArgumentNullException("value") if choices is null.
            // Login calls this with an empty pledge name before the socket opens.
            var empty = new List<string>();
            dropdown.choices = empty;
            dropdown.SetValueWithoutNotify(string.Empty);
            return empty;
        }
        else
        {
            var _listDropDown = new List<string> { "Main Clan - " + clanName };
            dropdown.value = "Main Clan - " + clanName;
            dropdown.choices = _listDropDown;
            return _listDropDown;
        }

    }

    public void UpdateMembersTable(ClanMember clanMember, ICreatorTables _creatorTableWindows)
    {
        _creatorTableWindows.UpdateRowByName(clanMember.MemberName , "" , new string[4] { clanMember.MemberName, clanMember.Level.ToString(), ROLE_CREATE, GetOnline(clanMember.Online) });
    }

    public void CreateMembersTable(List<ClanMember> listMemebers, ICreatorTables _creatorTableWindows)
    {

        var namesList = new List<string>();
        var lvlList = new List<string>();
        var roleList = new List<string>();
        var activity = new List<string>();


        PopulateMemberData(listMemebers, namesList, lvlList, roleList, activity);
        EnsureMinimumRowCount(namesList, lvlList, roleList, activity, 7);


        var tableColumns = GetColumns(namesList, lvlList, roleList, activity);
        SetOfflineMemberColors(listMemebers, tableColumns);

        _creatorTableWindows.UpdateTableData(tableColumns);
    }

    public void DeleteMemeberTable(string memberName , PledgeShowMemberListAllDto packet , ICreatorTables _creatorTableWindows)
    {
        if(packet != null)
        {
            var deletemember = packet.Members.FirstOrDefault(m => m.MemberName == memberName);
            packet.Members?.Remove(deletemember);
            _creatorTableWindows.RemoveByName(memberName);
        }

    }

    public void AddMemberData(ClanMember clanMember, PledgeShowMemberListAllDto packet, ICreatorTables _creatorTableWindows)
    {
        packet.Members.Add(clanMember);
        _creatorTableWindows.AddRow(new string[4] { clanMember.MemberName , clanMember.Level.ToString() , ROLE_CREATE, GetOnline(clanMember.Online) });
    }

    private void PopulateMemberData(List<ClanMember> members, List<string> names,
    List<string> levels, List<string> roles, List<string> activities)
    {
        foreach (var member in members)
        {
            names.Add(member.MemberName);
            levels.Add(member.Level.ToString());
            roles.Add(ROLE_CREATE);
            SetOnline(member, activities);
        }
    }
}

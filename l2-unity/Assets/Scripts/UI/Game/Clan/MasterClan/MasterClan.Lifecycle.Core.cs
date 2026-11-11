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
}

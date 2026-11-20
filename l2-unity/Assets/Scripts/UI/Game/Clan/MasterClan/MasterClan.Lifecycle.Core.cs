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

    public List<string> SetDropdownList(DropdownField dropdown, NetClanBook book, IList<int> pledgeTypesOut)
    {
        pledgeTypesOut.Clear();
        if (dropdown == null)
            return new List<string>();

        if (book == null || !book.HasClan || string.IsNullOrEmpty(book.Name))
        {
            var empty = new List<string>();
            dropdown.choices = empty;
            dropdown.SetValueWithoutNotify(string.Empty);
            return empty;
        }

        var labels = new List<string>();
        labels.Add("Main Clan - " + book.Name);
        pledgeTypesOut.Add(0);
        NetSubPledge[] subs = book.SubPledges;
        for (int i = 0; i < subs.Length; i++)
        {
            string label = SubUnitLabel(subs[i]) + " - " + (string.IsNullOrEmpty(subs[i].Name) ? book.Name : subs[i].Name);
            labels.Add(label);
            pledgeTypesOut.Add(subs[i].Type);
        }

        dropdown.choices = labels;
        dropdown.SetValueWithoutNotify(labels[0]);
        return labels;
    }

    static string SubUnitLabel(NetSubPledge sub)
    {
        if (sub.Type == -1 || sub.Type == 100)
            return "Academy";
        if (sub.Type >= 1001)
            return "Order of Knights";
        if (sub.Type == 200 || sub.Type == 201)
            return "Royal Guard";
        return "Sub-unit";
    }
}

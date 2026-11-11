using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CreatorTabsWindows : AbstractCreator, ICreator
{
    public CreatorTabsWindows()
    {
        EventSwitchOut += OnSwitchEventOut;
    }

    public void AddData(List<ItemInstance> allItems)
    {
        throw new System.NotImplementedException();
    }

    public void AddOtherData(List<OtherModel> allItems)
    {
        throw new System.NotImplementedException();
    }

    public ItemInstance GetActiveByPosition(int position)
    {
        throw new System.NotImplementedException();
    }

    public void InsertTablesIntoContent(ICreatorTables creatorTable, List<TableColumn> dataColumn , bool useAllTabs)
    {
        VisualElement element = GetActiveContent();

        _useAllTabs = useAllTabs;
        _creatorTable = creatorTable;
        _dataColumn = dataColumn;

        if (element != null)
        {
            _creatorTable.InitTable(element);
            _creatorTable.CreateTable(dataColumn);
        }
        else
        {
            // Debug.LogError("CreatorTabsWindows >>> InsertTablesIntoContent Not Found Content Element!!!");
        }

    }
}

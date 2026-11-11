using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CreatorTabsWindows : AbstractCreator, ICreator
{
    public void SetClickActiveTab(int position)
    {
        throw new System.NotImplementedException();
    }

    public void OnSwitchEventOut(ITab tab, bool isTrade)
    {
        if (_useAllTabs)
        {
           OnEventSwitchTab(FindTabByName(tab.GetTabName()));

           VisualElement element = GetActiveContent();

          _creatorTable.DestroyTable();
          element.Clear();
          _creatorTable.InitTable(element);
          _creatorTable.CreateTable(_dataColumn);
        }

    }

    public void OnEventSwitchTab(int tabIndex)
    {
     
        if (EventSwitchTabByIndexOfTab != null)
        {
            EventSwitchTabByIndexOfTab(tabIndex);
        }
    }

    public void RefreshDataColumns(List<TableColumn> dataColumns)
    {
        _dataColumn = dataColumns;
    }

    public void InsertFooterIntoContent(VisualElement footerElement , VisualElement root)
    {
        if(root != null && footerElement != null)
        {
            root.Clear();
            root.Add(footerElement);
        }
    }
}

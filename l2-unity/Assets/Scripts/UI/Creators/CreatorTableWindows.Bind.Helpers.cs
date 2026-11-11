using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CreatorTableWindows
{
    public void ClearTable()
    {
        if (_table == null)
        {
            return;
        }

        VisualElement containerBodyList = _table.Q<VisualElement>(_tableBodyListName);
        RemoveFromHierarchy(containerBodyList);
    }

    public bool HasTable(VisualElement element)
    {
        if (element == null)
            return false;

      
        bool hasTableBody = element.Q<VisualElement>("table_body") != null;
        bool hasTableHeader = element.Q<VisualElement>("table_header") != null;

        bool hasTableClasses = element.ClassListContains("l2-table") ||
                              element.ClassListContains("table-container");

        return hasTableBody && hasTableHeader || hasTableClasses;
    }

    private void RemoveFromHierarchy(VisualElement containerBodyList)
    {
        foreach (var column in containerBodyList.Children().ToList())
        {

            foreach (var row in column.Children().ToList())
            {
                row.RemoveFromHierarchy();
            }
        }
    }

    private VisualElement GetNewRow()
    {
            return  ToolTipsUtils.CloneOne(_templateRowBlack);
    }
}

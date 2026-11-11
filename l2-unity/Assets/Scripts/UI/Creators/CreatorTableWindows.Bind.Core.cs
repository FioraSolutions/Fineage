using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CreatorTableWindows
{
    public void AddRow(string[] allList)
    {
        if (!IsTableInitialized()) return;

        _functions.AddRow(allList , _columnsList , _listView);
    }

    public void UpdateRowByName(string rowName , string hex_color , string[] allList)
    {
        if (!IsTableInitialized()) return;

        _functions.UpdateRowByName(rowName, hex_color , allList, _columnsList, _listView);
    }

    private bool IsTableInitialized()
    {
        if (_listView == null || _columnsList == null || _columnsList.Count == 0)
        {
            // Debug.LogError("ListView or columns not initialized");
            return false;
        }
        return true;
    }

    public void DestroyTable()
    {
        if (_table != null)
        {

            _listView.makeItem -= MakeItem;
            _listView.bindItem -= BindListViewItems;
            _listView.selectedIndicesChanged -= SelectRow;
            //_listView.itemsSource.Clear();
            _lastSelectIndex = -1;
            _currentSelectIndex = -1;
            _lastSelectElement = null;

            _table = null;
            _tableHeHeaderItem = null;
            containerHeader = null;
            _lastSelectIndex = -1;
        }
    }
}

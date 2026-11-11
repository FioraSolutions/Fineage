using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CreatorTableWindows
{
    public void InitTable(VisualElement content)
    {
        _content = content;
        _defaultCursor = IconPacketServices.Icons.LoadCursorByName("Default");
        _functions = new TableFunction();
    }

    public void LoadAsset(Func<string, VisualTreeAsset> loaderFunc)
    {
        _templateTable = loaderFunc(_templateNameTable);
        _templateHeader = loaderFunc(_templateHeaderItem);
        _templateInside = loaderFunc(_templateColumnInside);
        _templateRows = loaderFunc(_templateRowsName);
        _templateRowBlack = loaderFunc(_templateRowBlackName);

    }

    public void CreateTable(List<TableColumn> columnsList)
    {
        
        if (_templateTable == null || _templateHeader == null)
        {
            // Debug.LogError("CreatorTableWindows not found TemplateTable or TemplateHeader");
            return;
        }


        _lastSelectIndex = -1;
        _currentSelectIndex = -1;
        _lastSelectElement = null;

        _index_row = 0;
        _table = ToolTipsUtils.CloneOne(_templateTable);

        _listView = _table.Q<ListView>(_tableListViewName);
        _listView.fixedItemHeight = 18;
        _columnsList = columnsList;

        _innerScrollView = _listView.Q<ScrollView>();

        var arr_column0 = _columnsList[0].ListData;
        _listView.itemsSource = arr_column0;


        _listView.makeItem = MakeItem;
        _listView.bindItem = BindListViewItems;
        _listView.selectedIndicesChanged += SelectRow;

        _tableHeHeaderItem = CreateHeaderItems(columnsList);
         VisualElement containerHeaderList = _table.Q<VisualElement>(_tableHeaderListName);
        InjectHeaderList(_tableHeHeaderItem, containerHeaderList);
        IfSize0HideListView(columnsList[0].ListData.Count, _listView);
        _content.Add(_table);
    }
}

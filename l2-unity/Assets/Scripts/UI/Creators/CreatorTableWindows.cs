

using System;
using System.Collections.Generic;

using System.Linq;
using UnityEngine;

using UnityEngine.UIElements;


public partial class CreatorTableWindows : ICreatorTables
{
    private VisualElement _content;
    private const string _templateNameTable = "Data/UI/_Elements/Template/TableList";
    private const string _templateHeaderItem = "Data/UI/_Elements/Template/TableHeaderItem";
    private const string _templateColumnInside = "Data/UI/_Elements/Template/TableHeaderName";
    private const string _templateRowsName = "Data/UI/_Elements/Template/TableRows";
    private const string _templateRowBlackName = "Data/UI/_Elements/Template/TableRowBlack";



    private const string _tableListViewName = "table_listView";
    private const string _tableBodyListName = "table_body";
    private const string _tableHeaderListName = "table_header";
    private const string _tableHeaderLabelName = "columnLabel";
    private const string _tableHeaderBorder = "columnBorder";


    private VisualTreeAsset _templateTable;
    private VisualTreeAsset _templateHeader;
    private VisualTreeAsset _templateInside;
    private VisualTreeAsset _templateRows;

    private VisualTreeAsset _templateRowBlack;


    private int _lastSelectIndex = -1;
    private int _currentSelectIndex = -1;
    private VisualElement _lastSelectElement;

    private VisualElement _table;
    private List<VisualElement> _tableHeHeaderItem;
    private VisualElement containerHeader;

    private ListView _listView;
    private ScrollView _innerScrollView;
    private List<TableColumn> _columnsList;
    private Texture2D _defaultCursor;
    public int _index_row = 0;
    public event Action<int , string> OnRowClicked;
    private TableFunction _functions;
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

  

    private void IfSize0HideListView(int size , VisualElement table)
    {
        if(size == 0)
        {
            table.style.display = DisplayStyle.None;

        }
        else
        {
            table.style.display = DisplayStyle.Flex;
        }
    }

   
    private void InjectHeaderList(List<VisualElement> _tableHeHeaderItem , VisualElement containerHeaderList)
    {
        foreach (var header in _tableHeHeaderItem)
        {
            containerHeaderList.Add(header);
        }
    }

    public void UpdateTableData(List<TableColumn> columnsList)
    {
        if (_table == null)
        {
            // Debug.LogError("Table is not created. Call CreateTable first.");
            return;
        }

        if (_templateTable == null || _templateHeader == null)
        {
            // Debug.LogError("CreatorTableWindows not found TemplateTable or TemplateHeader");
            return;
        }

        _lastSelectIndex = -1;
        _currentSelectIndex = -1;
        _lastSelectElement = null;
        _columnsList.Clear();
        _columnsList = columnsList;
         var arr_column0 = _columnsList[0].ListData;
        _listView.itemsSource = arr_column0;
        _listView.Rebuild();

        _listView.selectedIndex = -1;

        var headerBorder = containerHeader.Q<VisualElement>(_tableHeaderBorder);
        if(headerBorder != null && _innerScrollView != null) OnHeaderGeometryChanged(headerBorder, _innerScrollView);
        IfSize0HideListView(columnsList[0].ListData.Count, _listView);

    }




    public void RemoveByName(string rowName)
    {
        if (!IsTableInitialized()) return;

        _functions.RemoveByName(rowName, _columnsList, _listView, ref _currentSelectIndex, OnRowClicked);
    }

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

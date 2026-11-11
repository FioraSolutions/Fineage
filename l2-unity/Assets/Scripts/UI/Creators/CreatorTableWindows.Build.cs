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
}

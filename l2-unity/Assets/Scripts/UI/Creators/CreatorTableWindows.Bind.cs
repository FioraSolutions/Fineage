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

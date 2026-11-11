public partial class TableFunction
{
    public void ResetSelectInner(VisualElement highlightLast, VisualElement highlightLastTile)
    {
        if (highlightLast != null) highlightLast.style.display = DisplayStyle.None;
        if (highlightLastTile != null) highlightLastTile.style.display = DisplayStyle.None;
    }

    public void AddRow(string[] allListData , List<TableColumn> columnsList , ListView listView)
    {
        AddRowData(allListData, columnsList);
        RefreshTable(listView);
    }

    private void AddRowData(string[] allListData, List<TableColumn> columnsList)
    {
        if (columnsList == null || columnsList.Count == 0) return;

        int targetIndex = FindFirstEmptyIndex(columnsList[0]);
        UpdateColumnsWithData(allListData, columnsList, targetIndex);
    }

    private int FindFirstEmptyIndex(TableColumn column)
    {
        return column.ListData.FindIndex(string.IsNullOrEmpty);
    }

    private void UpdateColumnsWithData(string[] allListData, List<TableColumn> columnsList, int targetIndex)
    {
        for (int i = 0; i < columnsList.Count; i++)
        {
            var column = columnsList[i];
            var data = GetColumnData(allListData, i);
            UpdateColumnData(column, data, targetIndex);
        }
    }
}

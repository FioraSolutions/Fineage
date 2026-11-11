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

    private string GetColumnData(string[] allListData, int columnIndex)
    {
        return ArrayUtils.IsValidIndexArray(allListData, columnIndex) ?
            allListData[columnIndex] : "";
    }

    private void UpdateColumnData(TableColumn column, string data, int targetIndex)
    {
        if (targetIndex != -1)
        {
            column.ListData[targetIndex] = data;
        }
        else
        {
            column.ListData.Add(data);
        }
    }

    public void UpdateRowByName(string rowName, string hex_color , string[] newRowData, List<TableColumn> columnsList, ListView listView)
    {
        if (columnsList == null || columnsList.Count == 0 || newRowData == null)
        {
            // Debug.LogError("Invalid input parameters");
            return;
        }

        int rowIndex = FindRowIndex(rowName, columnsList[0].ListData);
        if (rowIndex == -1)
        {
            // Debug.LogError($"Row with name '{rowName}' not found");
            return;
        }

        UpdateRowData(rowIndex, newRowData, columnsList);
        UpdateHexColorData(rowIndex, columnsList, newRowData , hex_color);
        RefreshTable(listView);
    }

    private void UpdateRowData(int rowIndex, string[] newRowData, List<TableColumn> columnsList)
    {
        for (int i = 0; i < columnsList.Count; i++)
        {
            var column = columnsList[i];
            string data = GetColumnData(newRowData, i);

            if (rowIndex < column.ListData.Count)
            {
                column.ListData[rowIndex] = data;
            }
        }
    }

    private void UpdateHexColorData(int rowIndex , List<TableColumn> columnList, string[] newRowData , string hex_color)
    {
        if (!string.IsNullOrEmpty(hex_color))
        {
            for (int i = 0; i < columnList.Count; i++)
            {

                var column = columnList[i];
                string ids = rowIndex + newRowData[i];
                column.SetColor(ids, hex_color);
            }
        }

    }
}

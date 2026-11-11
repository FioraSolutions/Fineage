public partial class TableFunction
{
    public void RemoveByName(string rowName ,  List<TableColumn> columnsList , ListView listView , ref int currentSelectIndex , Action<int, string> OnRowClicked)
    {

        var listData = columnsList[0].ListData;
        int rowIndex = FindRowIndex(rowName, columnsList[0].ListData);

        if (rowIndex == -1) return;

        RemoveRowData(rowIndex , columnsList);
        AddEmptyRow(columnsList);
        RefreshTable(listView);
        UpdateSelectionAfterRemoval(rowIndex , ref currentSelectIndex);
        bool hasContent = listData.Any(str => !string.IsNullOrEmpty(str));
        ResetSelectIndexIfListData0(hasContent , listData, ref currentSelectIndex);
        ResetSelectElement(hasContent, listView);
        TriggerRowClickEvent(rowIndex , listData , OnRowClicked);
    }

    private int FindRowIndex(string rowName , List<string> listData)
    {
        var firstColumnData = listData;
        return firstColumnData.IndexOf(rowName);
    }

    private void RemoveRowData(int rowIndex , List<TableColumn> columnsList)
    {
        foreach (var column in columnsList)
        {
            column.ListData.RemoveAt(rowIndex);
        }
    }

    private void AddEmptyRow(List<TableColumn> columnsList)
    {
        foreach (var column in columnsList)
        {
            column.ListData.Add("");
        }
    }

    private void RefreshTable(ListView listView)
    {
        listView.RefreshItems();
    }
}

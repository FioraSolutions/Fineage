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

    private void UpdateSelectionAfterRemoval(int removedRowIndex , ref int currentSelectIndex)
    {
        if (currentSelectIndex > removedRowIndex)
        {
            currentSelectIndex--;
        }
    }

    private void ResetSelectIndexIfListData0(bool hasContent , List<string> listData , ref int currentSelectIndex)
    {
        if (!hasContent)
        {
            currentSelectIndex = -1;
        }
    }

    private void TriggerRowClickEvent(int rowIndex , List<string> listData , Action<int, string> OnRowClicked)
    {
        string text = listData[rowIndex];
        string selectText = "___" + text;
        OnRowClicked?.Invoke(rowIndex, selectText);
    }

    private void ResetSelectElement(bool hasContent , ListView listView)
    {
        if (!hasContent)
        {
            ResetSelectElement(listView);
        }
    }

    private void ResetSelectElement(ListView listView)
    {
        var selectedVisualElement = listView.Q<VisualElement>(className: "unity-collection-view__item--selected");

        for (int i = 0; i < selectedVisualElement.childCount; i++)
        {
            VisualElement row1 = selectedVisualElement[i];
            if (row1.childCount > 0)
            {
                VisualElement highlightLast = row1.Q<VisualElement>("highlight");
                VisualElement highlightLastTile = row1.Q<VisualElement>("highlightTile");
                ResetSelectInner(highlightLast, highlightLastTile);
            }
        }
    }
}

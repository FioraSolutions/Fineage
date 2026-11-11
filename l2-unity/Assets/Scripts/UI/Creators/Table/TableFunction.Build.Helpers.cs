using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using UnityEngine;

public partial class TableFunction
{
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

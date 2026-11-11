using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CreatorTableWindows
{
    private void EventClickOut(string select_text , int currentSelectIndex)
    {
        OnRowClicked?.Invoke(currentSelectIndex, select_text);
    }

    private void ShowSelectHighLightInner(VisualElement highlight, VisualElement highlightTile, int lastChild, int i)
    {
        if(i == lastChild)
        {
            highlightTile.style.display = DisplayStyle.Flex;
        }
        else
        {
            highlight.style.display = DisplayStyle.Flex;
        }

    }

    private void ShowSelectHighLight(VisualElement row1 , int childCount , int i)
    {
       // row1.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0));
        VisualElement highlight = row1.Q<VisualElement>(i == childCount ? "highlightTile" : "highlight");
        highlight.style.display = DisplayStyle.Flex;
    }

    public void UpClickMouse(MouseUpEvent evt , VisualElement element)
    {
        element.RemoveFromClassList("l2-table-header-name-bg-click");
        element.AddToClassList("l2-table-header-name-bg");
    }

    public void SetSelectRow(int rowId)
    {
        if(rowId == -1)
        {
            //if(_selectListColumn != null)
            //{
             //   ResetSelectList(_selectListColumn, rowId);
             //   _lastElementIndex = rowId;
            //}

        }
    }
}

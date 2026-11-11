using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CreatorTableWindows
{
    public void ClickMouse(MouseDownEvent evt , VisualElement element)
    {
        element.RemoveFromClassList("l2-table-header-name-bg");
        element.AddToClassList("l2-table-header-name-bg-click");
    }

    public void SelectRow(IEnumerable<int> selectedItems)
    {
        var indexRow = selectedItems.FirstOrDefault();


        var selectedVisualElement = _listView.Q<VisualElement>(className: "unity-collection-view__item--selected");

        if(ClickRow(selectedVisualElement , indexRow))
        {
            _currentSelectIndex = indexRow;
        }
    }

    public bool ClickRow(VisualElement _selectListColumn , int currentSelectIndex)
    {
        UnityEngine.Cursor.SetCursor(_defaultCursor, Vector2.zero, UnityEngine.CursorMode.Auto);
        if (_selectListColumn != null)
        {
            return SetSelectElement(_selectListColumn ,  currentSelectIndex);
        }
        return false;
    }

    private bool  SetSelectElement(VisualElement _selectListColumn , int  currentSelectIndex)
    {
        bool isSelect = false;
        string select_text = "";
        for (int i = 0; i < _selectListColumn.childCount; i++)
        {
            VisualElement row1 = _selectListColumn[i];

            if (row1.childCount > 0)
            {

                Label labeltext = row1.Q<Label>("labelText");
                VisualElement image = row1.Q<VisualElement>("image");

                var bg = image.style.backgroundImage.value;

                if (bg.IsEmpty())
                {
                    if (labeltext == null | string.IsNullOrEmpty(labeltext.text)) break;
                }

                select_text += "___"+labeltext.text;

                if (_lastSelectElement != null && currentSelectIndex != _lastSelectIndex)
                {
                    VisualElement last_row1 = _lastSelectElement[i];
                    VisualElement highlightLast = last_row1.Q<VisualElement>("highlight");
                    VisualElement highlightLastTile = last_row1.Q<VisualElement>("highlightTile");
                    _functions.ResetSelectInner(highlightLast, highlightLastTile);
                }



                ShowSelectHighLight(row1, _selectListColumn.childCount - 1, i);
                isSelect = true;
            }
        }


        if (_lastSelectElement != _selectListColumn & isSelect == true)
        {
            _lastSelectElement = _selectListColumn;
            _lastSelectIndex = currentSelectIndex;
        }

        EventClickOut(select_text, currentSelectIndex);

        return isSelect;
    }
}

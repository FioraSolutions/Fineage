using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CreatorTableWindows
{
    public VisualElement MakeItem()
    {

            VisualElement tablesContainer = new VisualElement();

            tablesContainer.style.flexDirection = FlexDirection.Row;
            tablesContainer.style.flexGrow = 1;
            tablesContainer.style.flexShrink = 1;

            for (int i=0; i < _columnsList.Count; i++)
            {
                var column = _columnsList[i];
                var listRows = ToolTipsUtils.CloneOne(_templateRows);
                VisualElement element = CreateRow("" , 1);
                listRows.Add(element);
                tablesContainer.Add(listRows);

                AlignItem(listRows, column);
                listRows.name = "body_column_" + i;


                //listRows.RegisterCallback<MouseDownEvent>(evt => UnityEngine.Cursor.SetCursor(_defaultCursor, Vector2.zero, UnityEngine.CursorMode.Auto));
                //listRows.RegisterCallback<MouseUpEvent>(evt => UnityEngine.Cursor.SetCursor(_defaultCursor, Vector2.zero, UnityEngine.CursorMode.Auto));
                //listRows.RegisterCallback<PointerMoveEvent>(evt => UnityEngine.Cursor.SetCursor(_defaultCursor, Vector2.zero, UnityEngine.CursorMode.Auto));
                //listRows.RegisterCallback<PointerDownEvent>(evt => UnityEngine.Cursor.SetCursor(_defaultCursor, Vector2.zero, UnityEngine.CursorMode.Auto));

                listRows.RegisterCallback<MouseDownEvent>(ResetCursorOnMouseDown);
                listRows.RegisterCallback<MouseUpEvent>(ResetCursorOnMouseUp);
                listRows.RegisterCallback<PointerMoveEvent>(ResetCursorOnPointerMove);
                listRows.RegisterCallback<PointerDownEvent>(ResetCursorOnPointerDown);
            }

        _index_row++;

        return tablesContainer;

   }

    private void ResetCursorOnMouseDown(MouseDownEvent evt) =>
        UnityEngine.Cursor.SetCursor(_defaultCursor, Vector2.zero, UnityEngine.CursorMode.Auto);
    private void ResetCursorOnMouseUp(MouseUpEvent evt) =>
        UnityEngine.Cursor.SetCursor(_defaultCursor, Vector2.zero, UnityEngine.CursorMode.Auto);
    private void ResetCursorOnPointerMove(PointerMoveEvent evt) =>
        UnityEngine.Cursor.SetCursor(_defaultCursor, Vector2.zero, UnityEngine.CursorMode.Auto);
    private void ResetCursorOnPointerDown(PointerDownEvent evt) =>
        UnityEngine.Cursor.SetCursor(_defaultCursor, Vector2.zero, UnityEngine.CursorMode.Auto);

    private void UnregisterRowCallbacks(VisualElement listRows)
    {
        listRows.UnregisterCallback<MouseDownEvent>(ResetCursorOnMouseDown);
        listRows.UnregisterCallback<MouseUpEvent>(ResetCursorOnMouseUp);
        listRows.UnregisterCallback<PointerMoveEvent>(ResetCursorOnPointerMove);
        listRows.UnregisterCallback<PointerDownEvent>(ResetCursorOnPointerDown);
    }

    private void BindListViewItems(VisualElement ve , int i)
    {

        var row = (VisualElement)ve;

        for (int n = 0; n < _columnsList.Count; n++)
        {
            var column = _columnsList[n];
            VisualElement row_list1 = row[n];
            VisualElement in_1 = row_list1[0];
            //structur template TableRowBlack - inside 0 > content_highlight & 1 > label_text
            VisualElement content_highlight = in_1[0];
            VisualElement highlightLast = content_highlight[0];
            VisualElement highlightTile = content_highlight[1];

            Label label = (Label)in_1[1];
            VisualElement image = in_1[2];

            if (column.ListData.Count > 0)
            {
                SetTextOrImage(label, image, column, i);
                ToggleHideTextOrImage(label, image, column);
                SetColorRowsWhiteOrBlack(row_list1, i);
                ReturnSetSelectIfChangePosition(highlightLast, highlightTile, n, i);
                ResetLastSelectElementIfChangePosition(highlightLast, highlightTile, n, i);
                //i - unique index row
                //column.ListData[i] - text in cell
                SetOtherColorLabel(column, label, i+column.ListData[i]);
            }

        }

        if (_lastSelectIndex == _currentSelectIndex & i == _currentSelectIndex)
        {
            _lastSelectElement = row;
           // Debug.Log("LastIdex select update link" + _currentSelectIndex + " last index " + _lastSelectIndex);
        }

    }

    private void ToggleHideTextOrImage(Label textLabel , VisualElement image, TableColumn column)
    {

        if (column.IsImage)
        {
            textLabel.style.display = DisplayStyle.None;
            image.style.display = DisplayStyle.Flex;
        }
        else
        {
            textLabel.style.display = DisplayStyle.Flex;
            image.style.display = DisplayStyle.None;
        }
    }

    private void SetTextOrImage(Label textLabel, VisualElement image, TableColumn column , int i)
    {
        if (column.IsImage)
        {
            string img_source = column.ListData[i];

            textLabel.text = "";
            if (!L2SlotIconView.TrySetResourcesBackground(image, img_source, useNativeSize: true))
            {
                L2SlotIconView.Clear(image);
                image.style.width = 0;
                image.style.height = 0;
            }

        }
        else
        {
            textLabel.text = column.ListData[i];
            image.style.backgroundImage = null;
            image.style.width = 0;
            image.style.height = 0;
        }
    }

    private void SetColorRowsWhiteOrBlack(VisualElement listRow, int index)
    {
       

        if (index % 2 == 0)
        {
            // Color #393835 with 79% opacity
            listRow.style.backgroundColor = new Color(0.22f, 0.22f, 0.21f, 0.79f);
        }
        else
        {
            // Fully transparent
            listRow.style.backgroundColor = new Color(0f, 0f, 0f, 0f);
        }
    }

    private void SetOtherColorLabel(TableColumn column, VisualElement row, string text)
    {
        if (column.IsImage)
        {
            return;
        }

        if (column.IsSetColor())
        {
            Color color = column.GetColor(text);
            row.style.color = color != Color.white ? color : Color.white;
        }
        else
        {
            if(row.style.color != Color.white)
            {
                row.style.color = Color.white;
            }

        }
    }

    private void ReturnSetSelectIfChangePosition(VisualElement highlightLast , VisualElement highlightTile , int n , int i)
    {
        if (_currentSelectIndex == i)
        {
            ShowSelectHighLightInner(highlightLast, highlightTile, _columnsList.Count - 1, n);
        }
        else
        {
            _functions.ResetSelectInner(highlightLast, highlightTile);
        }
    }

    private void ResetLastSelectElementIfChangePosition(VisualElement highlightLast, VisualElement highlightTile, int n, int i)
    {
        if (_lastSelectIndex == i && _lastSelectIndex != _currentSelectIndex)
        {
            _functions.ResetSelectInner(highlightLast, highlightTile);
        }

    }

}

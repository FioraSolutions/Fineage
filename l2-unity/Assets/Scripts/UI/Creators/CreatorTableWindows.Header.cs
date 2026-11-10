using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CreatorTableWindows
{
    public List<VisualElement> CreateHeaderItems(List<TableColumn> tablesColumns)
    {
        if (_templateHeader == null)
        {
            // Debug.LogError("CreatorTableWindows not found TemplateHeader");
            return new List<VisualElement>();
        }
        //Create Header Box (Only BorderBox)
        containerHeader = ToolTipsUtils.CloneOne(_templateHeader);
        var headerBorder = containerHeader.Q<VisualElement>(_tableHeaderBorder);

        headerBorder.RegisterCallback<GeometryChangedEvent>(evt => OnHeaderGeometryChanged( headerBorder , _innerScrollView));

        return tablesColumns.Select(headerName =>
        {
            //Create Items Inside Header
            var rootContainerHeader = ToolTipsUtils.CloneOne(_templateInside);
            Label tableHeaderLabel = rootContainerHeader.Q<Label>(_tableHeaderLabelName);
            VisualElement containerBgElement = rootContainerHeader.Q<VisualElement>("bgElement");

            SetManualWidth(containerBgElement, headerName.ManualWidth);
            CreateHeader(tableHeaderLabel, containerBgElement, headerName);
            headerBorder.Add(rootContainerHeader);

            return containerHeader;
        }).ToList();
    }

    private void CreateHeader(Label tableHeaderLabel , VisualElement bgElement , TableColumn headerName)
    {
        bgElement.RegisterCallback<MouseDownEvent>(evt => ClickMouse(evt, bgElement));
        bgElement.RegisterCallback<MouseUpEvent>(evt => UpClickMouse(evt, bgElement));
        tableHeaderLabel.text = headerName.NameColumn;
    }

    private void AlignItem(VisualElement listRows, TableColumn column)
    {
        var rows = listRows.Children().ToList();
        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];

            if (column.AlignTextCenter)
            {
                row.style.alignItems = new StyleEnum<Align>(Align.Center);
                // row.style.justifyContent = new StyleEnum<Justify>(Justify.Center);
                if (column.IsImage) row.style.justifyContent = new StyleEnum<Justify>(Justify.Center);
            }
            else
            {
                var label = row.Q<Label>("labelText");
                label.style.paddingLeft = column.LeftIndent;
            }

            if(column.MaxTextSize != 0)
            {
                var label = row.Q<Label>("labelText");
                MaxLabelSize(label, column.MaxTextSize);
            }


        }
    }

    public void MaxLabelSize(Label label , int maxSize)
    {
        label.style.maxWidth = maxSize;
    }

    public VisualElement CreateRow(string text , int index)
    {
        VisualElement row;
        row = GetNewRow();
        Label labelText = row.Q<Label>("labelText");
        CleapText(labelText);

        if (labelText != null)
        {
            labelText.text = text;
        }

        return row;
    }

    public List<VisualElement> CreateRows(TableColumn column)
    {
        List < VisualElement > rows = new List<VisualElement>();

        for (int i = 0; i < column.ListData.Count; i++)
        {
            VisualElement row;
            string text = column.ListData[i];
            row = GetNewRow();
            Label labelText = row.Q<Label>("labelText");
            CleapText(labelText);

            if (labelText !=  null)
            {
                labelText.text = text;
            }

            rows.Add(row);
        }

        return rows;
    }

    private void CleapText(Label rowLabel)
    {
        //rowLabel.style.width = 200;                   // ôèêñ. øèðèíà
        rowLabel.style.whiteSpace = WhiteSpace.NoWrap; // çàïðåòèòü ïåðåíîñ
        rowLabel.style.textOverflow = TextOverflow.Clip; // îáðåçàòü
    }

}

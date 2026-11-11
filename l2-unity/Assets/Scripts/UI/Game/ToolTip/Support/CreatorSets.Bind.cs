using UnityEngine;
using UnityEngine.UIElements;

public partial class CreatorSets
{
    private TemplateContainer CreateVisualElement(int i, Abstractgrp grpItem, string name, VisualTreeAsset setsTemplate , bool isEquip)
    {
        var template = setsTemplate.CloneTree();
        CreateHeaderif0Count(i, template);
        SetDataIcon(grpItem, template);
        SetDataLabel(name, template , isEquip);

        return template;
    }

    private void AddCountPiece(string i , TemplateContainer template)
    {
        Label labelName = template.Q<Label>("labelSetName");
        labelName.text = i;
    }

    private void AddTextEffect(string text, TemplateContainer template)
    {
        Label labelText = template.Q<Label>("labelSetText");
        labelText.text = text;
    }

    private void CreateHeaderif0Count(int i , TemplateContainer template)
    {
        if (i == 0)
        {
            Label labelHeader = template.Q<Label>("labelSetHeader");
            labelHeader.text = "<Set Effect>";
        }
    }

    private void SetDataIcon(Abstractgrp grpItem , TemplateContainer template)
    {
        VisualElement iconSet = template.Q<VisualElement>("iconSet");
        if (iconSet != null)
            L2SlotIconView.SetNamedIcon(iconSet, grpItem.Icon);
    }

    private void SetDataLabel(string name, TemplateContainer template , bool isEquip)
    {
        Label labelSet = template.Q<Label>("labelNameSet");
        if (labelSet != null)
        {
            SetColor(labelSet, isEquip);
            labelSet.text = name;
        }

    }

    private void SetColor(Label labelSet, bool isEquip)
    {
        if (isEquip)
        {
            Color color;
            ColorUtility.TryParseHtmlString("#d9d488", out color);
            labelSet.style.color = color;
        }
        else
        {
            Color color;
            ColorUtility.TryParseHtmlString("#61612E", out color);
            labelSet.style.color = color;
        }
    }
}

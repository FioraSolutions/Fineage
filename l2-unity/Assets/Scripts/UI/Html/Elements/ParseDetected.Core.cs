using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class ParseDetected : IParse
{
    public List<IElementsUI> GetElements()
    {
        return _elements.Values.ToList();
    }

    public void Parse(string text)
    {

    }

    public void ParseNode(HtmlNode node)
    {
        try
        {
            switch (node.Name)
            {
                case "#text":
                    AddText(node);
                    break;
                case "br":
                case "br1":
                    AddBr(node);
                    break;
                case "a":
                    AddHref(node);
                    break;
                case "font":
                    AddFont(node);
                    break;
                case "edit":
                    AddEdit(node);
                    break;
                case "button":
                    AddButton(node);
                    break;
                default:
                    break;
            }
        }
        catch (Exception)
        {
            // Debug.LogWarning("HtmlMessage->ParseNode: error " + ex.ToString());
        }
    }

    private void AddButton(HtmlNode node)
    {
        if (node.HasAttributes)
        {
            var attributes = node.Attributes;

            var valueAttr = attributes.FirstOrDefault(attr => attr.Name.Equals("value"));
            var actionAttr = attributes.FirstOrDefault(attr => attr.Name.Equals("action"));
            var widthAttr = attributes.FirstOrDefault(attr => attr.Name.Equals("width"));
            var heightAttr = attributes.FirstOrDefault(attr => attr.Name.Equals("height"));
            var backAttr = attributes.FirstOrDefault(attr => attr.Name.Equals("back"));
            var foreAttr = attributes.FirstOrDefault(attr => attr.Name.Equals("fore"));

            string value = valueAttr?.Value ?? "Button";
            string action = actionAttr?.Value ?? "";
            string width = widthAttr?.Value ?? "75";
            string height = heightAttr?.Value ?? "21";
            string back = backAttr?.Value ?? "";
            string fore = foreAttr?.Value ?? "";

            string key = "button_" + value + "_" + _count++;

            if (!_elements.ContainsKey(key))
            {
                _elements.Add(key, new ParseButton(value, action, width, height, back, fore));
            }
        }
    }

    private void AddText(HtmlNode node)
    {
        if (!_elements.ContainsKey(node.InnerText))
        {
            _elements.Add(node.InnerText, new ParseLabel(node.InnerText));
        }
    }
}

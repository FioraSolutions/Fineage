using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class ParseDetected : IParse
{
    private void AddBr(HtmlNode node)
    {
       _elements.Add("br" + _count++, new ParseBr());
    }

    private void AddHref(HtmlNode node)
    {
        var href = node.Attributes.FirstOrDefault();
        var name = node.InnerText;

        if (!_elements.ContainsKey(name))
        {
            _elements.Add(name, new ParseHref(name, href.Value));
        }
        else
        {
            AddDictNotUniq(name, name, href.Value);
        }
    }

    private void AddFont(HtmlNode node)
    {
        var fontColor = node.Attributes.FirstOrDefault();
        string color = fontColor.Value;
        string text = node.InnerText;
        ParseFontColor parseFontColor = new ParseFontColor(color, text);

        if (!AddHrefColor(text, parseFontColor, _elements))
        {
            _elements.Add(text, parseFontColor);
        }
    }

    private bool AddHrefColor(string textId , ParseFontColor color , Dictionary<string, IElementsUI> dict)
    {
        if (dict.ContainsKey(textId))
        {
           IElementsUI element =  dict[textId];
            
            if(element != null && element.GetType() == typeof(ParseHref))
            {
                var href = (ParseHref)element;
                href.SetColor(color);
                return true;
            }
        }
        return false;
    }

    private void AddEdit(HtmlNode node)
    {
        string text = node.Name;
        var elementNameVar = "";
        if (node.HasAttributes)
        {
            var attr = node.Attributes;
            elementNameVar = attr.Where(p => p.Name.Equals("var"))
                .FirstOrDefault().Value;
        }
        _elements.Add(text , new ParseEdit(elementNameVar));
    }

    private void AddDictNotUniq(string nameName, string name, string hrefValue)
    {
        _elements.Remove(name);
        _elements.Add(nameName, new ParseHref(name, hrefValue));
    }
}

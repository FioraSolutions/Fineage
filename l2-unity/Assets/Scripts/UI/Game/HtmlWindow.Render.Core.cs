using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using VContainer;

public partial class HtmlWindow
{
    public void InjectToWindow(string html)
    {
        _normalHtml = html ?? "";

        ApplyNormalWindowMode();
        RenderHtml(_normalHtml, "");
    }

    public void ShowNpcDialog(int npcObjectId, int npcId, string html)
    {
        NetNpcSession.OpenDialog(npcObjectId);
        string titled = html ?? "";
        if (string.IsNullOrEmpty(ExtractTitle(titled)))
        {
            string name = NetNpcSession.NameOf(npcObjectId);
            if (string.IsNullOrEmpty(name))
                name = "NPC";
            titled = "<title>" + name + "</title>" + titled;
        }

        _ = npcId;
        InjectToWindow(titled);
        Show();
    }

    private void RenderHtml(string html, string defaultTitle)
    {
        if (_content == null)
            return;

        _content.Clear();

        string title = ExtractTitle(html);
        SetWindowTitle(!string.IsNullOrEmpty(title) ? title : defaultTitle);

        L2HtmlRenderer renderer = new L2HtmlRenderer(_content, OnHtmlAction);
        renderer.Render(html);
    }
}

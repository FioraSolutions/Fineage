using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SystemMsgWindow : L2Window
{
    public void DockAboveChat(VisualElement chatWindow)
    {
        _chatAnchor = chatWindow;
        if (_windowEle != null)
            BindDock(chatWindow);
    }

    void BindDock(VisualElement chat)
    {
        if (chat == null || _windowEle == null)
            return;
        if (!_docked)
        {
            _docked = true;
            chat.RegisterCallback<GeometryChangedEvent>(_ => SyncDock());
            _windowEle.RegisterCallback<GeometryChangedEvent>(_ => SyncDock());
        }
        SyncDock();
        // One-frame defer: ChatWnd SnapSize / tab layout often finishes after first bind.
        _windowEle.schedule.Execute(SyncDock).ExecuteLater(0);
        _windowEle.schedule.Execute(SyncDock).ExecuteLater(1);
    }

    public void SyncDock()
    {
        if (_windowEle == null || _chatAnchor == null || _windowEle.parent == null)
            return;

        Rect chat = _chatAnchor.worldBound;
        Rect parent = _windowEle.parent.worldBound;
        float height = _windowEle.resolvedStyle.height;
        if (height <= 0f)
            height = DefaultHeight;

        // Prefer layout width — worldBound can lag one frame behind style.width after resize.
        float width = _chatAnchor.resolvedStyle.width;
        if (width <= 1f)
            width = chat.width;

        _windowEle.style.left = chat.x - parent.x;
        _windowEle.style.width = width;
        _windowEle.style.minWidth = width;
        _windowEle.style.maxWidth = width;
        _windowEle.style.top = chat.y - parent.y - height - GapAboveChat;
        _windowEle.style.bottom = StyleKeyword.Null;
        _windowEle.style.right = StyleKeyword.Null;
    }

    public void AppendLine(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;
        if (!_ready || _content == null)
        {
            _pending.Add(text);
            return;
        }
        AppendLineNow(text);
    }

    void AppendLineNow(string text)
    {
        string line = text.IndexOf("<color=", StringComparison.Ordinal) >= 0
            ? text
            : NetChat.FormatSystem(text);

        Label label;
        if (_lines.Count >= MaxLines)
        {
            label = _lines[0];
            _lines.RemoveAt(0);
            label.RemoveFromHierarchy();
        }
        else
        {
            label = new Label();
            label.AddToClassList("system-msg-line");
            label.enableRichText = true;
        }

        label.text = line;
        _content.Add(label);
        _lines.Add(label);
        ScheduleScrollDown();
    }

    void ScheduleScrollDown()
    {
        if (_list == null)
            return;
        // UI Toolkit schedule — safe when this MonoBehaviour GO is briefly inactive.
        _list.schedule.Execute(() =>
        {
            if (_list != null)
                _list.verticalScroller.value = _list.verticalScroller.highValue;
        });
    }
}

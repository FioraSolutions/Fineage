using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SystemMsgWindow : L2Window
{
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

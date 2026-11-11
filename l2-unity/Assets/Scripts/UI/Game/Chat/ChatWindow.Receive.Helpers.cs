using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ChatWindow
{
    public void AppendSystemLine(string text)
    {
        // Interlude: system lines go to SystemMsgWnd, not ChatWnd tabs.
        SystemMsgWindow.Ensure().AppendLine(text);
    }

    internal void ScrollDown(Scroller scroller)
    {
        StartCoroutine(ScrollDownWithDelay(scroller));
    }

    IEnumerator ScrollDownWithDelay(Scroller scroller)
    {
        yield return new WaitForEndOfFrame();
        scroller.value = scroller.highValue > 0 ? scroller.highValue : 0;
    }
}

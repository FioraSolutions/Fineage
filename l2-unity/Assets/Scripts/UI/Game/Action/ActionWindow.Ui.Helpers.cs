using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class ActionWindow : L2PopupWindow
{
    public void EmbedInto(VisualElement host)
    {
        if (host == null || _windowEle == null)
            return;
        // Hide chrome of the standalone popup; park the body under MainWnd.
        _windowEle.style.display = DisplayStyle.None;
        VisualElement body = _windowEle.Q("content") ?? _basicContainer?.parent ?? _windowEle;
        if (body.parent != host)
        {
            body.RemoveFromHierarchy();
            host.Add(body);
            body.style.position = Position.Relative;
            body.style.left = 0;
            body.style.top = 0;
            body.style.width = Length.Percent(100);
            body.style.height = Length.Percent(100);
        }
        host.style.display = DisplayStyle.Flex;
        AudioPacketServices.Audio.PlayUISound("window_open");
    }

    private void AddSlot(int position, VisualElement container)
    {
        VisualElement slotElement = _slotTemplate.Instantiate()[0];
        container.Add(slotElement);

        ActionSlot slot = new ActionSlot(slotElement, position, SlotType.Action);
        _slots.Add(slot);
    }

    public override void ShowWindow()
    {
        base.ShowWindow();
        AudioPacketServices.Audio.PlayUISound("window_open");
        SystemUIPacketServices.Ui.WindowOpened(this);
    }

    public override void HideWindow()
    {
        base.HideWindow();
        AudioPacketServices.Audio.PlayUISound("window_close");
        SystemUIPacketServices.Ui.WindowClosed(this);
    }
}

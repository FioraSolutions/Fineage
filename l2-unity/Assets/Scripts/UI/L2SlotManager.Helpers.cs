using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class L2SlotManager : L2PopupWindow
{
    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();
        _dragSlotData = new L2Slot(_windowEle);
        _windowEle.usageHints = UsageHints.None;
        _windowEle.pickingMode = PickingMode.Ignore;
        _windowEle.style.position = Position.Absolute;
        _windowEle.Query<VisualElement>().ForEach(e => e.pickingMode = PickingMode.Ignore);
        // Start hidden — ShowWindow only flips opacity/display during an active drag.
        HideWindow();
    }

    public void SetHoverSlot(L2Slot slot)
    {
        _hoverSlot = slot;
    }

    public void SetDraggedSlot(L2Slot slot)
    {
        _draggedSlot = slot;

        _dragSlotData.Icon = slot.Icon;
        _dragSlotData.Id = slot.Id;

        RefreshImage(slot);
    }

    private void RefreshImage(L2Slot slot)
    {
        if (slot.GetType() == typeof(GearSlot))
        {
            StyleBackground background = slot.SlotElement.style.backgroundImage;
            _dragSlotData.SlotBg.style.backgroundImage = background;
        }
        else
        {
            if (slot.SlotBg != null)
            {
                StyleBackground background = slot.SlotBg.style.backgroundImage;
                _dragSlotData.SlotBg.style.backgroundImage = background;
            }
        }
    }

    internal void DragSlotAt(Vector2 panelPos, Vector2 grabOffset)
    {
        ShowWindow();
        _windowEle.style.right = StyleKeyword.Null;
        _windowEle.style.bottom = StyleKeyword.Null;

        Vector2 local = panelPos;
        VisualElement parent = _windowEle.parent;
        if (parent != null)
            local = parent.WorldToLocal(panelPos);

        _windowEle.style.left = local.x - grabOffset.x;
        _windowEle.style.top = local.y - grabOffset.y;
        _windowEle.BringToFront();
    }

    public void ReleaseDrag()
    {

        HideWindow();

        if (!IsValidDrag() || IsSameSlot())
        {
            ResetSlots();
            return;
        }

        HandleDragRelease();
        ResetSlots();
    }

    public void CancelDrag()
    {
        HideWindow();
        ResetSlots();
    }
}

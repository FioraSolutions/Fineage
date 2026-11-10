using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class L2SlotManager : L2PopupWindow
{

    [SerializeField] private L2Slot _draggedSlot;
    [SerializeField] private L2Slot _hoverSlot;
    private L2Slot _dragSlotData;

    private static L2SlotManager _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static L2SlotManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            WireHudCorePacketServices();
            WireGameWindowPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireHudCorePacketServices();
        WireGameWindowPacketServices();
    }

    
    private void WireHudCorePacketServices()
    {
        HudCorePacketServices.Bind(slots: this);
    }

    private void WireGameWindowPacketServices()
    {
        GameWindowPacketServices.Bind();
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Template/DraggedSlot");
    }

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

    /// <summary>
    /// Places the floating drag icon so <paramref name="grabOffset"/> (local to the source slot)
    /// stays under the pointer. <paramref name="panelPos"/> is UI Toolkit panel space.
    /// </summary>
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

    /// <summary>Abort an in-progress or primed slot drag without applying a drop.</summary>
    public void CancelDrag()
    {
        HideWindow();
        ResetSlots();
    }

}

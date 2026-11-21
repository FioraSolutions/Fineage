using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class ActionWindow : L2PopupWindow
{
    private void WireHudCorePacketServices()
    {
        HudCorePacketServices.Bind(action: this);
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/ActionWindow");
        _slotTemplate = LoadAsset("Data/UI/_Elements/Template/ActionSlot");
    }

    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);

        VisualElement dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);

        Label _windowName = (Label)GetElementById("windows-name-label");
        _windowName.text = "Actions";

        _basicContainer = GetElementById("BasicSlots");
        _partyContainer = GetElementById("PartySlots");
        _socialContainer = GetElementById("SocialSlots");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        // Center window
        _windowEle.style.left = new Length(50, LengthUnit.Percent);
        _windowEle.style.top = new Length(50, LengthUnit.Percent);
        _windowEle.style.translate = new StyleTranslate(new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent)));

        _slots = new List<ActionSlot>();

        int position = 0;
        // interface.xdat ActionWnd: Basic 6x3, Party 6x2, Social 6x3. No Token row.
        for (int i = 0; i < SLOTS_PER_ROW * 3; i++)
        {
            AddSlot(position++, _basicContainer);
        }

        for (int i = 0; i < SLOTS_PER_ROW * 2; i++)
        {
            AddSlot(position++, _partyContainer);
        }

        for (int i = 0; i < SLOTS_PER_ROW * 3; i++)
        {
            AddSlot(position++, _socialContainer);
        }

        // Interlude ActionWnd order from InterludeActionCatalog (ActionName + interface.xdat ActionWnd).
        AssignSection(InterludeActionSection.Basic, _basicContainer);
        AssignSection(InterludeActionSection.Party, _partyContainer);
        AssignSection(InterludeActionSection.Social, _socialContainer);
        WireActionAvailability();
    }

    void WireActionAvailability()
    {
        _availabilityChanged = OnActionAvailabilityChanged;
        ActionAvailabilityPacketServices.Availability.Changed += _availabilityChanged;
        RefreshActionAvailability();
    }

    System.Action _availabilityChanged;

    void OnActionAvailabilityChanged() => RefreshActionAvailability();

    void OnDisable()
    {
        if (_availabilityChanged != null)
            ActionAvailabilityPacketServices.Availability.Changed -= _availabilityChanged;
    }
}

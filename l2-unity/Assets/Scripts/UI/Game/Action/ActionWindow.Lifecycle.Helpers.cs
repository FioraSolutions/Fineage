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

        // Interlude ActionWnd Basic / Party / Social.
        AssignBasic(
            ActionType.Sit, ActionType.WalkRun, ActionType.Attack, ActionType.Trade,
            ActionType.NextTarget, ActionType.Pickup, ActionType.Assist,
            ActionType.PrivateStoreSell, ActionType.PrivateStoreBuy,
            ActionType.DwarvenManufacture, ActionType.Mount, ActionType.Recommend);
        AssignParty(
            ActionType.PartyInvite, ActionType.PartyLeave, ActionType.PartyKick,
            ActionType.PartyMatching);
        AssignSocial(
            ActionType.SocialGreeting, ActionType.SocialVictory, ActionType.SocialAdvance,
            ActionType.SocialYes, ActionType.SocialNo, ActionType.SocialBow,
            ActionType.SocialUnaware, ActionType.SocialWaiting, ActionType.SocialLaugh,
            ActionType.SocialApplaud, ActionType.SocialDance, ActionType.SocialSorrow,
            ActionType.SocialCharm, ActionType.SocialShyness);
    }
}

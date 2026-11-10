using System;
using UnityEngine.UIElements;

public class ActionSlot : L2DraggableSlot
{
    public int ActionId { get; private set; }
    public ActionData Action { get; private set; }

    public ActionSlot(VisualElement slotElement, int position, SlotType slotType) : base(position, slotElement, slotType, true, false)
    {
        _slotElement = slotElement;
        _position = position;
    }

    protected override void HandleLeftClick()
    {
        ShortcutPacketServices.Actions.UseAction((ActionType)ActionId);
    }

    protected override void HandleRightClick()
    {
    }

    protected override void HandleMiddleClick()
    {
    }

    public void AssignAction(ActionType actionType)
    {
        ButtonClickSoundManipulator _buttonClickSoundManipulator = new ButtonClickSoundManipulator(_slotElement);
        _slotDragManipulator.enabled = true;
        _slotElement.RemoveFromClassList("empty");

        ActionId = (int)actionType;
        Action = TablePacketServices.ActionNames.GetAction(actionType);
        L2SlotIconView.SetNamedIcon(_slotBg, ConvertIcon(actionType));

        if (_tooltipManipulator != null)
        {
            _tooltipManipulator.SetText(Action != null ? Action.Name : "Ride Strider");
        }
    }

    private string ConvertIcon(ActionType actionType)
    {
        // Interlude ActionWnd icon stems (button_img/actionNNN).
        switch (actionType)
        {
            case ActionType.Sit: return "action001";
            case ActionType.WalkRun: return "action002";
            case ActionType.Attack: return "action003";
            case ActionType.Trade: return "action010";
            case ActionType.NextTarget: return "action005";
            case ActionType.Pickup: return "action007";
            case ActionType.Assist: return "action008";
            case ActionType.PartyInvite: return "action011";
            case ActionType.PartyLeave: return "action012";
            case ActionType.PartyKick: return "action013";
            case ActionType.PrivateStoreSell: return "action028";
            case ActionType.PartyMatching: return "action010";
            case ActionType.SocialGreeting: return "action015";
            case ActionType.SocialVictory: return "action016";
            case ActionType.SocialAdvance: return "action017";
            case ActionType.SocialYes: return "action018";
            case ActionType.SocialNo: return "action019";
            case ActionType.SocialBow: return "action020";
            case ActionType.SocialUnaware: return "action021";
            case ActionType.SocialWaiting: return "action022";
            case ActionType.SocialLaugh: return "action024";
            case ActionType.SocialApplaud: return "action029";
            case ActionType.SocialDance: return "action030";
            case ActionType.SocialSorrow: return "action031";
            case ActionType.SocialCharm: return "action032";
            case ActionType.SocialShyness: return "action033";
            case ActionType.PrivateStoreBuy: return "action034";
            case ActionType.DwarvenManufacture: return "action040";
            case ActionType.Mount: return "etc_pet_ride_01";
            case ActionType.Recommend: return "action041";
        }

        ActionData named = TablePacketServices.ActionNames.GetAction(actionType);
        return named != null ? named._icon : "action001";
    }
}
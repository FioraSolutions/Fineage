using System;
using UnityEngine.UIElements;

public partial class ActionSlot : L2DraggableSlot
{
    protected override void HandleMiddleClick()
    {
    }

    public void AssignAction(ActionType actionType)
    {
        ButtonClickSoundManipulator _buttonClickSoundManipulator = new ButtonClickSoundManipulator(_slotElement);
        _slotDragManipulator.enabled = true;
        _slotElement.RemoveFromClassList("empty");

        ActionId = (int)actionType;
        Action = TablePacketServices.ActionNames != null
            ? TablePacketServices.ActionNames.GetAction(actionType)
            : null;
        L2SlotIconView.SetNamedIcon(_slotBg, ConvertIcon(actionType));

        string tip = HoverText(actionType);
        if (_tooltipManipulator == null && _slotElement != null)
        {
            _tooltipManipulator = new TooltipManipulator(_slotElement, tip);
            _slotElement.AddManipulator(_tooltipManipulator);
        }
        else if (_tooltipManipulator != null)
        {
            _tooltipManipulator.SetText(tip);
        }
    }

    static string HoverText(ActionType actionType)
    {
        ActionData named = TablePacketServices.ActionNames != null
            ? TablePacketServices.ActionNames.GetAction(actionType)
            : null;
        if (named != null && !string.IsNullOrEmpty(named.Name))
        {
            if (string.IsNullOrEmpty(named.Descripion))
                return named.Name;
            return named.Name + "\n" + named.Descripion;
        }

        switch (actionType)
        {
            case ActionType.Sit: return "Sit/Stand\nToggle Sit/Stand. (/sit, /stand)";
            case ActionType.WalkRun: return "Walk/Run\nToggle Walk/Run. (/walk, /run)";
            case ActionType.Attack: return "Attack\nAttacks the selected target. (/attack)";
            case ActionType.Trade: return "Trade\nRequest a trade from the selected target. (/trade)";
            case ActionType.NextTarget: return "Next Target\nSelect the nearest attackable target. (/targetnext)";
            case ActionType.Pickup: return "Pick Up\nPick up items scattered nearby. (/pickup)";
            case ActionType.Assist: return "Assist\nSelect the target of the currently selected character. (/assist)";
            case ActionType.PartyInvite: return "Invite\nInvite the selected person to your party. (/invite)";
            case ActionType.PartyLeave: return "Leave Party\nLeave the party. (/leave)";
            case ActionType.PartyKick: return "Dismiss Party Member\nDismiss the selected member from your party. (/dismiss)";
            case ActionType.PartyMatching: return "Party Matching\nOpen the party matching window. (/partymatching)";
            case ActionType.PartyLeader: return "Change Party Leader\nThe selected party member becomes the leader. (/changepartyleader)";
            case ActionType.PrivateStoreSell: return "Private Store - Sell\nSet up a private store to sell items. (/vendor)";
            case ActionType.PrivateStoreBuy: return "Private Store - Buy\nSet up a private store to buy items. (/buy)";
            case ActionType.DwarvenManufacture: return "Dwarven Workshop\nSet up a workshop that crafts from Dwarven recipes. (/dwarvenmanufacture)";
            case ActionType.Mount: return "Mount/Dismount\nMount or dismount a strider you already have summoned. Does not summon one. (/mount, /dismount)";
            case ActionType.Recommend: return "Recommend\nRaise the selected character's evaluation. (/evaluate)";
            case ActionType.SocialGreeting: return "Greeting\nEmote: hello. (/socialhello)";
            case ActionType.SocialVictory: return "Victory\nEmote: victory. (/socialvictory)";
            case ActionType.SocialAdvance: return "Advance\nEmote: charge. (/socialcharge)";
            case ActionType.SocialYes: return "Yes\nEmote: yes. (/socialyes)";
            case ActionType.SocialNo: return "No\nEmote: no. (/socialno)";
            case ActionType.SocialBow: return "Bow\nEmote: bow. (/socialbow)";
            case ActionType.SocialUnaware: return "Unaware\nEmote: unaware. (/socialunaware)";
            case ActionType.SocialWaiting: return "Standby\nEmote: waiting. (/socialwaiting)";
            case ActionType.SocialLaugh: return "Laugh\nEmote: laugh. (/sociallaugh)";
            case ActionType.SocialApplaud: return "Applaud\nEmote: applause. (/socialapplause)";
            case ActionType.SocialDance: return "Dance\nEmote: dance. (/socialdance)";
            case ActionType.SocialSorrow: return "Sorrow\nEmote: sorrow. (/socialsad)";
            case ActionType.SocialCharm: return "Charm\nEmote: charm. (/charm)";
            case ActionType.SocialShyness: return "Shyness\nEmote: shyness.";
        }

        return actionType.ToString();
    }

    private string ConvertIcon(ActionType actionType)
    {
        // Interlude ActionWnd icon stems in SysTextures/Icon.
        switch (actionType)
        {
            case ActionType.Sit: return "action001";
            case ActionType.WalkRun: return "action002";
            case ActionType.Attack: return "action003";
            case ActionType.Trade: return "action010";
            case ActionType.NextTarget: return "action005";
            case ActionType.Pickup: return "action006";
            case ActionType.Assist: return "action008";
            case ActionType.PartyInvite: return "action011";
            case ActionType.PartyLeave: return "action012";
            case ActionType.PartyKick: return "action013";
            case ActionType.PrivateStoreSell: return "action028";
            case ActionType.PartyMatching: return "action014";
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
            case ActionType.PartyLeader: return "action050";
        }

        ActionData named = TablePacketServices.ActionNames != null
            ? TablePacketServices.ActionNames.GetAction(actionType)
            : null;
        return named != null && !string.IsNullOrEmpty(named._icon) ? named._icon : "action001";
    }
}

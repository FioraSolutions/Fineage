using System;
using UnityEngine.UIElements;

public partial class ActionSlot : L2DraggableSlot
{
    protected override void HandleMiddleClick()
    {
    }

    public void ClearAction()
    {
        ActionId = 0;
        Action = null;
        _slotDragManipulator.enabled = false;
        if (_slotElement != null)
        {
            _slotElement.AddToClassList("empty");
            _slotElement.style.display = DisplayStyle.None;
        }
        if (_slotBg != null)
            _slotBg.style.backgroundImage = StyleKeyword.Null;
        if (_tooltipManipulator != null)
        {
            _slotElement.RemoveManipulator(_tooltipManipulator);
            _tooltipManipulator = null;
        }
    }

    public void AssignAction(ActionType actionType)
    {
        ButtonClickSoundManipulator _buttonClickSoundManipulator = new ButtonClickSoundManipulator(_slotElement);
        _slotDragManipulator.enabled = true;
        if (_slotElement != null)
            _slotElement.style.display = DisplayStyle.Flex;
        _slotElement.RemoveFromClassList("empty");

        ActionId = (int)actionType;
        Action = TablePacketServices.ActionNames != null
            ? TablePacketServices.ActionNames.GetAction(actionType)
            : null;
        if (InterludeActionIcons.TryResolveResourcesPath((int)actionType, out string resPath))
            L2SlotIconView.TrySetResourcesBackground(_slotBg, resPath);

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
            case ActionType.Mount:
                return "Mount/Dismount";
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

}

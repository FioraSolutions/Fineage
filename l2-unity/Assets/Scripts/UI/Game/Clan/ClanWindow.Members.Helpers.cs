using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ClanWindow
{
    private void SetDisabledButton(string leaderName, List<ClanMember> Members)
    {
        string userName = GetSelfName();
        if (userName == null || _labelEditAuth == null)
        {
            return;
        }

        ResetAllButtons();

        if (IsLeader(leaderName))
            EnabledLeaderButtons();
        else
        {
            if (Members.FirstOrDefault(m => m.MemberName == userName) == null)
                EnabledUnkButtons();
            else
                EnabledUserButtons();
        }
    }

    private void ResetAllButtons()
    {
        _labelEditAuth.AddToClassList(USS_STYLE_DISABLED);
        _labelMemberInfo.AddToClassList(USS_STYLE_DISABLED);
        _labelPrivileges.AddToClassList(USS_STYLE_DISABLED);
        _labelClanInfo.AddToClassList(USS_STYLE_DISABLED);
        _labelPenalty.AddToClassList(USS_STYLE_DISABLED);
        _labelLeave.AddToClassList(USS_STYLE_DISABLED);
        _labelClanEntry.AddToClassList(USS_STYLE_DISABLED);
        _labelInvite.AddToClassList(USS_STYLE_DISABLED);
    }

    private void EnabledLeaderButtons()
    {
        _labelEditAuth.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelMemberInfo.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelPrivileges.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelClanInfo.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelPenalty.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelLeave.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelInvite.RemoveFromClassList(USS_STYLE_DISABLED);


        _labelEditAuth.AddToClassList(USS_STYLE_YELLOW);
        _labelMemberInfo.AddToClassList(USS_STYLE_YELLOW);
        _labelPrivileges.AddToClassList(USS_STYLE_YELLOW);
        _labelClanInfo.AddToClassList(USS_STYLE_YELLOW);
        _labelPenalty.AddToClassList(USS_STYLE_YELLOW);
        _labelLeave.AddToClassList(USS_STYLE_YELLOW);
        _labelInvite.AddToClassList(USS_STYLE_YELLOW);

    }

    private void EnabledUserButtons()
    {
        _labelMemberInfo.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelPrivileges.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelPenalty.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelClanInfo.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelLeave.RemoveFromClassList(USS_STYLE_DISABLED);

        _labelMemberInfo.AddToClassList(USS_STYLE_YELLOW);
        _labelPrivileges.AddToClassList(USS_STYLE_YELLOW);
        _labelPenalty.AddToClassList(USS_STYLE_YELLOW);
        _labelClanInfo.AddToClassList(USS_STYLE_YELLOW);
        _labelLeave.AddToClassList(USS_STYLE_YELLOW);
    }

    private void EnabledUnkButtons()
    {

        _labelPenalty.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelLeave.RemoveFromClassList(USS_STYLE_DISABLED);

        _labelPenalty.AddToClassList(USS_STYLE_YELLOW);
        _labelLeave.AddToClassList(USS_STYLE_YELLOW);

    }

    private void OkLeave()
    {
        ZoneCommands.Send(new RequestWithdrawPledgeCommand());
        CancelEvent();
    }

    private void OkInvite()
    {
        TargetManager targets = TargetPacketServices.Targets;
        if (targets != null && targets.HasTarget() && targets.Target.GetEntity() != null)
        {
            int id = targets.Target != null ? targets.Target.GetEntity().Identity.Id : 0;

            ZoneCommands.Send(new RequestJoinPledgeCommand(id));
        }

        CancelEvent();
    }

    private void OnРЎancel()
    {
        CancelEvent();
    }

    private void CancelEvent()
    {
        SystemMessagePacketServices.Window.OnButtonOk -= OkLeave;
        SystemMessagePacketServices.Window.OnButtonClosed -= OnРЎancel;
    }
}

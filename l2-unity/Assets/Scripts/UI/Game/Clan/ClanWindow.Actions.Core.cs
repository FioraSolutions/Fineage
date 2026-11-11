using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ClanWindow
{
    private void OnClickShowMember(ClickEvent evt)
    {
    }

    private void OnClickPrivileges(ClickEvent evt)
    {
    }

    private void OnClickEditAuthButton(ClickEvent evt)
    {
        //if (!string.IsNullOrEmpty(_masterClan.GetSelectMemberName()))
        //{
        //    ZoneCommands.Send(//        new RequestPledgePowerGradeList());
        //}

    }

    private void OnClickLeave(ClickEvent evt)
    {
        if(_packet != null && !IsLeader(_packet.SubPledgeLeaderName))
        {
            SystemMessagePacketServices.Window.OnButtonOk += OkLeave;
            SystemMessagePacketServices.Window.OnButtonClosed += OnРЎancel;
            SystemMessagePacketServices.Window.ShowWindowDialogYesOrNot("Are you sure you want to leave the clan?");
        }

    }
}

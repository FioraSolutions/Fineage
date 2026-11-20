using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ClanWindow
{
    private void OnClickLeave(ClickEvent evt)
    {
        if (!NetClanBook.Shared.HasClan)
            return;
        if (NetClanBook.Shared.IsLeader(SelfObjectId()))
        {
            ClanUiRequests.RejectLocal("ClanLeaveRequest", NetClanNotices.Map("clan_leader_leave"));
            return;
        }

        SystemMessagePacketServices.Window.OnButtonOk += OkLeave;
        SystemMessagePacketServices.Window.OnButtonClosed += OnРЎancel;
        SystemMessagePacketServices.Window.ShowWindowDialogYesOrNot("Are you sure you want to leave the clan?");
    }
}

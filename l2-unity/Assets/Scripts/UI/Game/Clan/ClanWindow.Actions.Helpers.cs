using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ClanWindow
{
    private void OnClickInvite(ClickEvent evt)
    {
        if (_packet != null && IsLeader(_packet.SubPledgeLeaderName))
        {
            SystemMessagePacketServices.Window.ShowWindowDialogDropdownYesOrNot(" Select a Unit " , new List<string> { _packet.PledgeName});
            SystemMessagePacketServices.Window.OnButtonOk += OkInvite;
            SystemMessagePacketServices.Window.OnButtonClosed += OnРЎancel;
        }

    }

    private string GetSelfName()
    {
        UserInfoDto user = StorageNpc.getInstance().GetFirstUser();
        if (user == null || user.PlayerInfoInterlude.Identity == null)
        {
            return null;
        }

        return user.PlayerInfoInterlude.Identity.Name;
    }

    private bool IsLeader(string leaderName)
    {
        string userName = GetSelfName();
        return userName != null && leaderName == userName;
    }

    private void OnClickPenalty(ClickEvent evt)
    {

            ZoneCommands.Send(new RequestUserCommandCommand(100));
        
    }

    private void OnClickClanInfo(ClickEvent evt)
    {
        _detailedClan.UpdateDetailedInfo(_pladgeClanInfo, _detailedInfoElement, _packet);
        ShowDetailedInfo();
    }
}

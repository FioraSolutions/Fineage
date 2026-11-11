using FMOD.Studio;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class ClanDetailedInfo
{
    private void OnOkDismiss(string memberName)
    {
        _selectDismiss = memberName;
        SystemMessagePacketServices.Window.OnButtonOk += SystemMessageClickOkDismiss;
        SystemMessagePacketServices.Window.OnButtonClosed += OnРЎancel;
        SystemMessagePacketServices.Window.ShowWindowDialogYesOrNot("Do you want to be kicked from the clan?");
    }

    private void SystemMessageClickOkDismiss()
    {
        ZoneCommands.Send(new RequestOustPledgeMemberCommand(_selectDismiss));
        CancelEvent();
    }

    private void OnРЎancel()
    {
        CancelEvent();
    }

    private void CancelEvent()
    {
        SystemMessagePacketServices.Window.OnButtonOk -= SystemMessageClickOkDismiss;
        SystemMessagePacketServices.Window.OnButtonClosed -= OnРЎancel;
    }
}

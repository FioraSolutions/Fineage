using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ClanWindow
{
    void OnCreateClan()
    {
        string name = _clanNameField != null ? _clanNameField.value : "";
        name = name != null ? name.Trim() : "";
        if (name.Length == 0)
        {
            NetChatFeed.NoteSystem("Enter a clan name.");
            return;
        }

        ZoneCommands.Send(new ClanCreateRequestCommand(name));
    }

    void OnKick()
    {
        if (!NetClanBook.Shared.IsLeader(SelfObjectId()))
            return;
        if (_selectedMemberId <= 0)
        {
            NetChatFeed.NoteSystem("Select a clan member to dismiss.");
            return;
        }

        if (_selectedMemberId == SelfObjectId())
        {
            NetChatFeed.NoteSystem(NetClanNotices.Map("clan_dismiss_self"));
            return;
        }

        int target = _selectedMemberId;
        Confirm("Dismiss this member from the clan?", () => ZoneCommands.Send(new ClanKickRequestCommand(target)));
    }

    void OnDissolve()
    {
        if (!NetClanBook.Shared.IsLeader(SelfObjectId()))
            return;
        Confirm("Register clan dissolution? The clan remains for 7 days.", () => ZoneCommands.Send(new ClanDissolveRequestCommand()));
    }

    void OnCancelDissolve()
    {
        if (!NetClanBook.Shared.IsLeader(SelfObjectId()))
            return;
        Confirm("Cancel the clan dissolution?", () => ZoneCommands.Send(new ClanCancelDissolveRequestCommand()));
    }

    void Confirm(string text, System.Action ok)
    {
        if (SystemMessagePacketServices.Window == null || ok == null)
            return;
        System.Action closed = null;
        System.Action accept = null;
        accept = () =>
        {
            SystemMessagePacketServices.Window.OnButtonOk -= accept;
            SystemMessagePacketServices.Window.OnButtonClosed -= closed;
            ok();
        };
        closed = () =>
        {
            SystemMessagePacketServices.Window.OnButtonOk -= accept;
            SystemMessagePacketServices.Window.OnButtonClosed -= closed;
        };
        SystemMessagePacketServices.Window.OnButtonOk += accept;
        SystemMessagePacketServices.Window.OnButtonClosed += closed;
        SystemMessagePacketServices.Window.ShowWindowDialogYesOrNot(text);
    }

    int SelfObjectId()
    {
        PlayerInfoInterlude info = ZoneCommands.LocalPlayerInfo;
        if (info.Identity != null && info.Identity.Id > 0)
            return info.Identity.Id;
        NetSession net = NetworkingPacketServices.Session;
        return net != null ? net.SessionObjectId : 0;
    }
}

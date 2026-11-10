using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ClanWindow
{
    void BuildZoneActions()
    {
        VisualElement footer = _windowEle.Q<VisualElement>("FooterButtons");
        VisualElement box = _windowEle.Q<VisualElement>("BoxContent");
        if (footer == null)
            footer = _windowEle;

        _zoneActions = new VisualElement();
        _zoneActions.style.flexDirection = FlexDirection.Row;
        _zoneActions.style.flexWrap = Wrap.Wrap;
        _zoneActions.style.marginTop = 4;
        _zoneActions.style.marginBottom = 2;

        _clanNameCaption = new Label("Clan");
        _clanNameCaption.style.color = new Color(0.7f, 0.7f, 0.7f);
        _clanNameCaption.style.unityFontStyleAndWeight = FontStyle.Normal;
        _clanNameCaption.style.fontSize = 11;
        _clanNameCaption.style.marginRight = 4;
        _clanNameCaption.style.alignSelf = Align.Center;

        _clanNameField = new TextField();
        _clanNameField.maxLength = 16;
        _clanNameField.style.width = 110;
        _clanNameField.style.height = 18;
        _clanNameField.style.marginRight = 4;

        _createClanButton = MakeZoneButton("Create", OnCreateClan);
        _kickButton = MakeZoneButton("Kick", OnKick);
        _dissolveButton = MakeZoneButton("Dissolve", OnDissolve);
        _cancelDissolveButton = MakeZoneButton("Cancel", OnCancelDissolve);

        _zoneActions.Add(_clanNameCaption);
        _zoneActions.Add(_clanNameField);
        _zoneActions.Add(_createClanButton);
        _zoneActions.Add(_kickButton);
        _zoneActions.Add(_dissolveButton);
        _zoneActions.Add(_cancelDissolveButton);
        if (box != null && footer != null && footer.parent == box)
            box.Insert(box.IndexOf(footer), _zoneActions);
        else
            footer.Insert(0, _zoneActions);
        TuneZoneButtons(NetClanBook.Shared);
    }

    static Button MakeZoneButton(string text, System.Action click)
    {
        var button = new Button(click) { text = text };
        button.style.height = 18;
        button.style.marginRight = 3;
        button.style.fontSize = 11;
        return button;
    }

    void ShowNoClan()
    {
        SetLabel("labelClanName", "");
        SetLabel("labelLeaderName", "");
        SetLabel("lvlName", "0");
        SetLabel("labelCountOnline", "(0/0)");
        Label peace = _windowEle.Q<Label>("labelAtPeace");
        if (peace != null)
            peace.text = "";
        if (_dropdown != null)
            _masterClan.SetDropdownList(_dropdown, "");
        if (_creatorTableWindows != null)
            _masterClan.CreateMembersTable(new List<ClanMember>(), _creatorTableWindows);
    }

    void SetLabel(string name, string text)
    {
        Label label = _windowEle.Q<Label>(name);
        if (label != null)
            label.text = text;
    }

    void TuneZoneButtons(NetClanBook book)
    {
        if (_zoneActions == null)
            return;
        _zoneActions.style.display = DisplayStyle.None;
    }

    void OnZoneRowClicked(int index, string text)
    {
        _selectedMemberId = 0;
        if (_masterClan == null)
            return;
        string name = _masterClan.GetSelectMemberName();
        NetPledgeMember member;
        if (NetClanBook.Shared.TryMemberByName(name, out member))
            _selectedMemberId = member.ObjectId;
    }

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
        IZoneTransport transport = ZoneCommands.Transport;
        if (transport != null && transport.PlayerInfo.Identity != null && transport.PlayerInfo.Identity.Id > 0)
            return transport.PlayerInfo.Identity.Id;
        NetSession net = NetworkingPacketServices.Session;
        return net != null ? net.SessionObjectId : 0;
    }

}

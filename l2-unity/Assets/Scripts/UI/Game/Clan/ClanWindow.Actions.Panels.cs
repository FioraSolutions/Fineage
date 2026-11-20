using System.Collections.Generic;
using System.Text;
using Fineage.Net.Contract;
using UnityEngine.UIElements;

public partial class ClanWindow
{
    void OnClickShowMember(ClickEvent evt)
    {
        if (_packet == null || !NetClanBook.Shared.HasClan)
        {
            ClanUiRequests.RejectLocal(nameof(PledgeCommandKind.RequestMemberList), "You are not in a clan.");
            return;
        }

        string memberName = _masterClan != null ? _masterClan.GetSelectMemberName() : "";
        if (string.IsNullOrEmpty(memberName))
        {
            ClanUiRequests.RejectLocal(nameof(PledgeCommandKind.RequestMemberList), "Select a clan member.");
            return;
        }

        NetPledgeMember row;
        if (!NetClanBook.Shared.TryMemberByName(memberName, out row))
        {
            ClanUiRequests.SendPledgeCommand(PledgeCommandKind.RequestMemberList, "RequestPledgeMemberList");
            return;
        }

        string subName = PledgeUnitLabel(row.PledgeType);
        var dto = new PledgeReceiveMemberInfoDto();
        dto.ApplySnapshot(row.PledgeType, row.Name, "", 0, subName, "");
        UpdateDetailedInfo(dto);
    }

    void OnClickPrivileges(ClickEvent evt)
    {
        if (_packet == null || !NetClanBook.Shared.HasClan)
        {
            ClanUiRequests.RejectLocal(nameof(PledgeCommandKind.RequestRankPrivileges), "You are not in a clan.");
            return;
        }

        ClanUiRequests.SendPledgeCommand(
            PledgeCommandKind.RequestRankPrivileges,
            "RequestPledgePowerGradeList",
            new PledgeCommandNetMsg { IntArg1 = 1 });
    }

    void OnClickEditAuthButton(ClickEvent evt)
    {
        OnClickPrivileges(evt);
    }

    static string PledgeUnitLabel(int pledgeType)
    {
        if (pledgeType == 0)
            return "Main Clan";
        if (pledgeType == -1 || pledgeType == 100)
            return "Academy";
        if (pledgeType >= 1001 && pledgeType <= 1002)
            return "Order of Knights";
        if (pledgeType == 200 || pledgeType == 201)
            return "Royal Guard";
        return "Sub-unit " + pledgeType;
    }

    void OnClickWarInfo(ClickEvent evt)
    {
        if (!NetClanBook.Shared.HasClan)
        {
            ClanUiRequests.RejectLocal(nameof(PledgeCommandKind.RequestWarList), "You are not in a clan.");
            return;
        }

        ClanUiRequests.SendPledgeCommand(PledgeCommandKind.RequestWarList, "RequestPledgeWarList");
        ShowWarListPanel();
    }

    void OnClickDeclareWar(ClickEvent evt)
    {
        if (!NetClanBook.Shared.IsLeader(SelfObjectId()))
        {
            ClanUiRequests.RejectLocal(nameof(PledgeCommandKind.DeclareWar), "Only the clan leader can declare war.");
            return;
        }

        ClanUiRequests.RejectLocal(
            nameof(PledgeCommandKind.DeclareWar),
            "Select an enemy clan name for RequestStartPledgeWar (name prompt not wired yet).");
    }

    void OnClickClanEntry(ClickEvent evt)
    {
        int target = ClanUiRequests.SelectedTargetObjectId();
        if (target <= 0)
        {
            ClanUiRequests.RejectLocal("RequestJoinPledge", "Select a character to invite into the clan.");
            return;
        }

        ZoneCommands.Send(new ClanInviteRequestCommand(target, SelectedPledgeType()));
    }

    void OnClickEditCrest(ClickEvent evt)
    {
        if (!NetClanBook.Shared.IsLeader(SelfObjectId()))
        {
            ClanUiRequests.RejectLocal(nameof(PledgeCommandKind.SetCrest), "Only the clan leader can change the crest.");
            return;
        }

        ClanUiRequests.RejectLocal(
            nameof(PledgeCommandKind.SetCrest),
            "Crest file upload UI is not implemented; use RequestSetPledgeCrest from the patched client.");
    }

    void OnClickNotice(ClickEvent evt)
    {
        string notice = NetClanBook.Shared.Notice;
        if (string.IsNullOrEmpty(notice))
            notice = "No clan notice has been set.";
        NetChatFeed.NoteSystem(notice);
    }

    void ShowWarListPanel()
    {
        NetClanBook book = NetClanBook.Shared;
        var body = new StringBuilder();
        AppendWarSection(body, "Declared", book.WarsDeclared);
        AppendWarSection(body, "Under attack", book.WarsUnderAttack);
        if (body.Length == 0)
            body.Append("No active clan wars.");
        NetChatFeed.NoteSystem(body.ToString());
    }

    static void AppendWarSection(StringBuilder body, string title, NetPledgeWarEntry[] rows)
    {
        if (rows == null || rows.Length == 0)
            return;
        body.Append(title).Append(":\n");
        for (int i = 0; i < rows.Length; i++)
        {
            NetPledgeWarEntry row = rows[i];
            body.Append("  ").Append(string.IsNullOrEmpty(row.Name) ? row.ClanId.ToString() : row.Name);
            if (row.Mutual)
                body.Append(" (mutual)");
            body.Append('\n');
        }
    }

    int SelectedPledgeType()
    {
        if (_dropdown == null || _listDropDown == null)
            return NetClanBook.MainPledge;
        int index = _listDropDown.IndexOf(_dropdown.value);
        if (index < 0 || index >= _pledgeFilterTypes.Count)
            return NetClanBook.MainPledge;
        return _pledgeFilterTypes[index];
    }
}
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetSession
{
    internal void ApplyCreatureSay(CreatureSayNetMsg say)
    {
        NetChatFeed.NoteSay(say.Name, say.Text, say.ChatType);
        // Chat log does not use ObjectId; overhead bubble does (#521).
        OverheadChatSay.Show(say.ObjectId, say.Text, say.ChatType);
    }

    internal void ApplySystemMessage(SystemMessageNetMsg message)
    {
        string shown = NetSystemLines.Resolve(message.Id, message.Text, message.Params);
        NetChatFeed.NoteSystem(shown);

        if (message.Id == SysMsgResurrectionOffer)
            ShowResurrectionOffer(shown);
    }

    void ShowResurrectionOffer(string messageText)
    {
        SystemMessageWindow window = SystemMessagePacketServices.Window;
        if (window == null)
            return;

        window.ClearDialogHandlers();
        window.OnButtonOk += () =>
            ZoneCommands.Send(new RequestRestartPointCommand(ReviveRequestNetMsg.AcceptSkillResurrection));
        window.OnButtonClosed += () =>
            ZoneCommands.Send(new RequestRestartPointCommand(ReviveRequestNetMsg.DeclineSkillResurrection));
        window.ShowWindowDialogYesOrNot(string.IsNullOrEmpty(messageText)
            ? "Do you want to be resurrected?"
            : messageText);
    }

    internal void ApplyPledgeStatus(PledgeStatusNetMsg status)
    {
        ClanPacketServices.ApplyStatus(new NetPledgeStatus
        {
            ClanId = status.ClanId,
            Name = status.Name ?? "",
            Level = status.Level,
            LeaderObjectId = status.LeaderObjectId,
            CrestId = status.CrestId,
            MemberCount = status.MemberCount,
            DissolvingExpiryUnixMs = status.DissolvingExpiryUnixMs,
            CharPenaltyExpiryUnixMs = status.CharPenaltyExpiryUnixMs
        });
    }

    internal void ApplyPledgeMemberList(PledgeMemberListNetMsg list)
    {
        PledgeMemberNet[] rows = list.Members ?? System.Array.Empty<PledgeMemberNet>();
        var members = new NetPledgeMember[rows.Length];
        for (int i = 0; i < rows.Length; i++)
        {
            PledgeMemberNet row = rows[i] ?? new PledgeMemberNet();
            members[i] = new NetPledgeMember
            {
                ObjectId = row.ObjectId,
                Name = row.Name ?? "",
                ClassId = row.ClassId,
                Level = row.Level,
                Online = row.Online,
                PledgeType = row.PledgeType
            };
        }

        ClanPacketServices.ApplyMembers(new NetPledgeMemberList
        {
            ClanId = list.ClanId,
            LeaderObjectId = list.LeaderObjectId,
            Members = members
        });
    }

    internal void ApplyClanAskJoin(ClanAskJoinNetMsg ask)
    {
        ClanPacketServices.ApplyAskJoin(new NetClanAskJoin
        {
            RequestorObjectId = ask.RequestorObjectId,
            RequestorName = ask.RequestorName ?? "",
            ClanId = ask.ClanId,
            ClanName = ask.ClanName ?? "",
            PledgeType = ask.PledgeType
        });
    }

    internal void ApplyClanJoinResult(ClanJoinResultNetMsg result)
    {
        ClanPacketServices.ApplyJoinResult(result.Response);
    }

    internal void ApplyPartyAskJoin(PartyAskJoinNetMsg ask)
    {
        var dto = AskJoinPartyDto.FromNet(ask.RequestorObjectId, ask.RequestorName ?? "", ask.ItemDistribution);
        if (_party != null) _party.ApplyAskJoinParty(dto);
        else AskJoinPartyApplyIncomingPacketServices.ApplyAskJoinParty(dto);
    }
}

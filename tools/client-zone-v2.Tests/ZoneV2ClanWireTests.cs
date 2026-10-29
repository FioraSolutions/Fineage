using MessagePack;
using Xunit;

namespace Fineage.Client.ZoneV2.Tests;

/// <summary>
/// Zone clan wire and pledge state. Layout matches Fineage-Backend <c>ClanProtocol</c> at 5a57705.
/// Unity EditMode cannot reference Assembly-CSharp; this project is the client wire test setup.
/// </summary>
public class ZoneV2ClanWireTests
{
    [Fact]
    public void Opcodes_match_the_backend_clan_contract()
    {
        Assert.Equal(15, (ushort)ZoneV2MsgType.UseItem);
        Assert.Equal(16, (ushort)ZoneV2MsgType.UnequipItem);
        Assert.Equal(17, (ushort)ZoneV2MsgType.GiveItem);
        Assert.Equal(32, (ushort)ZoneV2MsgType.ClanCreateRequest);
        Assert.Equal(33, (ushort)ZoneV2MsgType.ClanInviteRequest);
        Assert.Equal(34, (ushort)ZoneV2MsgType.ClanAnswerJoinRequest);
        Assert.Equal(35, (ushort)ZoneV2MsgType.ClanLeaveRequest);
        Assert.Equal(36, (ushort)ZoneV2MsgType.ClanKickRequest);
        Assert.Equal(37, (ushort)ZoneV2MsgType.ClanDissolveRequest);
        Assert.Equal(38, (ushort)ZoneV2MsgType.ClanCancelDissolveRequest);
        Assert.Equal(1005, (ushort)ZoneV2MsgType.CharInfo);
        Assert.Equal(1016, (ushort)ZoneV2MsgType.UserInfo);
        Assert.Equal(1040, (ushort)ZoneV2MsgType.SystemMessage);
        Assert.Equal(1050, (ushort)ZoneV2MsgType.ClanAskJoin);
        Assert.Equal(1051, (ushort)ZoneV2MsgType.PledgeStatusChanged);
        Assert.Equal(1052, (ushort)ZoneV2MsgType.PledgeMemberList);
        Assert.Equal(1053, (ushort)ZoneV2MsgType.ClanJoinResult);
        Assert.Equal(1099, (ushort)ZoneV2MsgType.Error);
        Assert.Equal(15, ZoneV2ClanBook.InviteTimeoutSeconds);
        Assert.Equal(0, ZoneV2ClanBook.MainPledge);
        Assert.Equal(20, (ushort)ZoneV2MsgType.Register);
        Assert.Equal(25, (ushort)ZoneV2MsgType.RestoreCharacter);
        for (ushort id = 1054; id <= 1059; id++)
            Assert.False(Enum.IsDefined(typeof(ZoneV2MsgType), id));
    }

    [Fact]
    public void Create_invite_answer_and_kick_round_trip()
    {
        var created = MessagePackSerializer.Deserialize<ClanCreateWireMsg>(ZoneV2Messages.EncodeClanCreateRequest("Wolves"));
        Assert.Equal("Wolves", created.Name);

        var invite = MessagePackSerializer.Deserialize<ClanInviteWireMsg>(
            ZoneV2Messages.EncodeClanInviteRequest(2001, ZoneV2ClanBook.MainPledge));
        Assert.Equal(2001, invite.TargetObjectId);
        Assert.Equal(0, invite.PledgeType);

        var answer = MessagePackSerializer.Deserialize<ClanAnswerWireMsg>(
            ZoneV2Messages.EncodeClanAnswerJoinRequest(1001, 1));
        Assert.Equal(1001, answer.RequestorObjectId);
        Assert.Equal(1, answer.Response);

        var kick = MessagePackSerializer.Deserialize<ClanKickWireMsg>(ZoneV2Messages.EncodeClanKickRequest(2001));
        Assert.Equal(2001, kick.TargetObjectId);
    }

    [Fact]
    public void Empty_clan_requests_are_empty_arrays()
    {
        Assert.Equal(new byte[] { 0x90 }, ZoneV2Messages.EncodeClanLeaveRequest());
        Assert.Equal(new byte[] { 0x90 }, ZoneV2Messages.EncodeClanDissolveRequest());
        Assert.Equal(new byte[] { 0x90 }, ZoneV2Messages.EncodeClanCancelDissolveRequest());
    }

    [Fact]
    public void Ask_join_status_members_and_result_decode()
    {
        var ask = new ClanAskJoinWireMsg
        {
            RequestorObjectId = 1001,
            RequestorName = "Leader",
            ClanId = 4,
            ClanName = "Wolves",
            PledgeType = 0
        };
        Assert.True(ZoneV2Messages.TryDecodeClanAskJoin(Pack(ask), out ZoneV2ClanAskJoin decodedAsk));
        Assert.Equal(1001, decodedAsk.RequestorObjectId);
        Assert.Equal("Leader", decodedAsk.RequestorName);
        Assert.Equal(4, decodedAsk.ClanId);
        Assert.Equal("Wolves", decodedAsk.ClanName);
        Assert.Equal(0, decodedAsk.PledgeType);

        long dissolving = 1_735_689_600_000L;
        var status = new PledgeStatusWireMsg
        {
            ClanId = 4,
            Name = "Wolves",
            Level = 0,
            LeaderObjectId = 1001,
            CrestId = 0,
            MemberCount = 2,
            DissolvingExpiryUnixMs = dissolving,
            CharPenaltyExpiryUnixMs = 0
        };
        Assert.True(ZoneV2Messages.TryDecodePledgeStatusChanged(Pack(status), out ZoneV2PledgeStatus decodedStatus));
        Assert.Equal(4, decodedStatus.ClanId);
        Assert.Equal("Wolves", decodedStatus.Name);
        Assert.Equal(0, decodedStatus.Level);
        Assert.Equal(1001, decodedStatus.LeaderObjectId);
        Assert.Equal(2, decodedStatus.MemberCount);
        Assert.Equal(dissolving, decodedStatus.DissolvingExpiryUnixMs);
        Assert.Equal(0, decodedStatus.CharPenaltyExpiryUnixMs);

        var list = new PledgeMemberListWireMsg
        {
            ClanId = 4,
            LeaderObjectId = 1001,
            Members =
            [
                new PledgeMemberWireMsg
                {
                    ObjectId = 1001,
                    Name = "Leader",
                    ClassId = 0,
                    Level = 10,
                    Online = true,
                    PledgeType = 0
                },
                new PledgeMemberWireMsg
                {
                    ObjectId = 2001,
                    Name = "Ally",
                    ClassId = 10,
                    Level = 8,
                    Online = false,
                    PledgeType = 0
                }
            ]
        };
        Assert.True(ZoneV2Messages.TryDecodePledgeMemberList(Pack(list), out ZoneV2PledgeMemberList roster));
        Assert.Equal(4, roster.ClanId);
        Assert.Equal(1001, roster.LeaderObjectId);
        Assert.Equal(2, roster.Members.Length);
        Assert.True(roster.Members[0].Online);
        Assert.False(roster.Members[1].Online);
        Assert.Equal("Ally", roster.Members[1].Name);
        Assert.Equal(8, roster.Members[1].Level);

        Assert.True(ZoneV2Messages.TryDecodeClanJoinResult(Pack(new ClanJoinResultWireMsg { Response = 0 }), out int response));
        Assert.Equal(0, response);
        Assert.False(ZoneV2Messages.TryDecodePledgeStatusChanged(Array.Empty<byte>(), out _));
        Assert.False(ZoneV2Messages.TryDecodeClanAskJoin(Pack(new ShortAskWireMsg { RequestorObjectId = 1 }), out _));
    }

    [Fact]
    public void Char_info_and_user_info_keep_clan_keys_optional()
    {
        int[] paperdoll = new int[ZoneV2Paperdoll.Count];
        paperdoll[ZoneV2Paperdoll.RightHand] = 2369;
        var other = new CharInfoClanWireMsg
        {
            ObjectId = 99,
            Name = "Other",
            X = 1,
            Y = 2,
            Z = 3,
            ClassId = 0,
            Paperdoll = paperdoll,
            ClanId = 4,
            ClanCrestId = 0
        };
        Assert.True(ZoneV2Messages.TryDecodeCharInfo(Pack(other), out ZoneV2CharInfo info));
        Assert.True(info.HasPaperdoll);
        Assert.Equal(2369, ZoneV2Paperdoll.ItemIdAt(info.PaperdollItemIds, ZoneV2Paperdoll.RightHand));
        Assert.True(info.HasClan);
        Assert.Equal(4, info.ClanId);
        Assert.Equal(0, info.ClanCrestId);

        var legacy = new CharInfoLegacyWireMsg
        {
            ObjectId = 3,
            Name = "Old",
            X = 1,
            Y = 2,
            Z = 3,
            ClassId = 0
        };
        Assert.True(ZoneV2Messages.TryDecodeCharInfo(Pack(legacy), out ZoneV2CharInfo old));
        Assert.False(old.HasClan);
        Assert.Equal(0, old.ClanId);

        var trailing = new CharInfoStringTailWireMsg
        {
            ObjectId = 4,
            Name = "Future",
            X = 1,
            Y = 2,
            Z = 3,
            ClassId = 0,
            Paperdoll = paperdoll,
            Future = "later"
        };
        Assert.True(ZoneV2Messages.TryDecodeCharInfo(Pack(trailing), out ZoneV2CharInfo kept));
        Assert.Equal(2369, ZoneV2Paperdoll.ItemIdAt(kept.PaperdollItemIds, ZoneV2Paperdoll.RightHand));
        Assert.False(kept.HasClan);

        var user = new UserInfoClanWireMsg
        {
            ObjectId = 42,
            Name = "Fighter",
            ClassId = 0,
            X = 1,
            Y = 2,
            Z = 3,
            RunSpeed = 120f,
            WalkSpeed = 80f,
            Running = true,
            Paperdoll = [],
            ClanId = 4,
            ClanCrestId = 0
        };
        Assert.True(ZoneV2Messages.TryDecodeUserInfo(Pack(user), out ZoneV2UserInfo self));
        Assert.True(self.HasClan);
        Assert.Equal(4, self.ClanId);
        Assert.Equal(0, self.ClanCrestId);
        Assert.Equal("Fighter", self.Name);
    }

    [Fact]
    public void Pledge_state_tracks_leader_members_and_clear()
    {
        var book = new ZoneV2ClanBook();
        book.NoteName(9, "Strangers");
        book.ApplyMembers(new ZoneV2PledgeMemberList
        {
            ClanId = 4,
            LeaderObjectId = 1001,
            Members =
            [
                new ZoneV2PledgeMember { ObjectId = 1001, Name = "Leader", Level = 10, Online = true },
                new ZoneV2PledgeMember { ObjectId = 2001, Name = "Ally", Level = 8, Online = false }
            ]
        });
        Assert.True(book.HasClan);
        Assert.Equal("", book.Name);
        Assert.True(book.IsLeader(1001));
        Assert.False(book.IsLeader(2001));
        Assert.Equal(1, book.OnlineCount());
        Assert.Equal("Leader", book.LeaderName());

        book.ApplyStatus(new ZoneV2PledgeStatus
        {
            ClanId = 4,
            Name = "Wolves",
            Level = 0,
            LeaderObjectId = 1001,
            MemberCount = 2,
            DissolvingExpiryUnixMs = 50
        });
        Assert.Equal("Wolves", book.Name);
        Assert.Equal("Wolves", book.NameFor(4));
        Assert.True(book.Dissolving);
        Assert.Equal(2, book.Members.Length);
        Assert.Equal("Strangers", book.NameFor(9));

        book.ApplyStatus(new ZoneV2PledgeStatus { ClanId = 0, Name = "Wolves" });
        Assert.False(book.HasClan);
        Assert.Empty(book.Members);
        Assert.False(book.Dissolving);
        Assert.Equal("Wolves", book.NameFor(4));
        Assert.Equal("Strangers", book.NameFor(9));

        book.ApplyStatus(new ZoneV2PledgeStatus { ClanId = 4, Name = "Wolves", LeaderObjectId = 1001 });
        book.ApplyMembers(new ZoneV2PledgeMemberList { ClanId = 0, Members = [] });
        Assert.False(book.HasClan);
        Assert.Empty(book.Members);
    }

    [Fact]
    public void Clan_error_codes_become_readable_lines()
    {
        Assert.Equal("The clan leader cannot leave. Dissolve the clan instead.", ZoneV2ClanNotices.Map("clan_leader_leave"));
        Assert.Equal("That clan invitation has expired.", ZoneV2ClanNotices.LineOrEquip("clan_invite", "clan_invite"));
        Assert.Equal("The clan is full.", ZoneV2ClanNotices.LineOrEquip("clan_full", ""));
        Assert.Equal("You must wait a day.", ZoneV2ClanNotices.LineOrEquip("clan_join_penalty", "You must wait a day."));
        Assert.Equal("You do not have that item.", ZoneV2EquipNotices.Line("not_owned", ""));
        Assert.Equal("You do not have that item.", ZoneV2ClanNotices.LineOrEquip("not_owned", ""));
        Assert.Equal("The clan request was refused.", ZoneV2ClanNotices.Map("clan_unknown"));
    }

    static byte[] Pack<T>(T msg) => MessagePackSerializer.Serialize(msg);
}

[MessagePackObject]
public sealed partial class ClanCreateWireMsg
{
    [Key(0)] public string Name { get; set; }
}

[MessagePackObject]
public sealed partial class ClanInviteWireMsg
{
    [Key(0)] public int TargetObjectId { get; set; }
    [Key(1)] public int PledgeType { get; set; }
}

[MessagePackObject]
public sealed partial class ClanAnswerWireMsg
{
    [Key(0)] public int RequestorObjectId { get; set; }
    [Key(1)] public int Response { get; set; }
}

[MessagePackObject]
public sealed partial class ClanKickWireMsg
{
    [Key(0)] public int TargetObjectId { get; set; }
}

[MessagePackObject]
public sealed partial class ClanAskJoinWireMsg
{
    [Key(0)] public int RequestorObjectId { get; set; }
    [Key(1)] public string RequestorName { get; set; }
    [Key(2)] public int ClanId { get; set; }
    [Key(3)] public string ClanName { get; set; }
    [Key(4)] public int PledgeType { get; set; }
}

[MessagePackObject]
public sealed partial class ShortAskWireMsg
{
    [Key(0)] public int RequestorObjectId { get; set; }
}

[MessagePackObject]
public sealed partial class PledgeStatusWireMsg
{
    [Key(0)] public int ClanId { get; set; }
    [Key(1)] public string Name { get; set; }
    [Key(2)] public int Level { get; set; }
    [Key(3)] public int LeaderObjectId { get; set; }
    [Key(4)] public int CrestId { get; set; }
    [Key(5)] public int MemberCount { get; set; }
    [Key(6)] public long DissolvingExpiryUnixMs { get; set; }
    [Key(7)] public long CharPenaltyExpiryUnixMs { get; set; }
}

[MessagePackObject]
public sealed partial class PledgeMemberListWireMsg
{
    [Key(0)] public int ClanId { get; set; }
    [Key(1)] public int LeaderObjectId { get; set; }
    [Key(2)] public PledgeMemberWireMsg[] Members { get; set; }
}

[MessagePackObject]
public sealed partial class PledgeMemberWireMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public string Name { get; set; }
    [Key(2)] public int ClassId { get; set; }
    [Key(3)] public int Level { get; set; }
    [Key(4)] public bool Online { get; set; }
    [Key(5)] public int PledgeType { get; set; }
}

[MessagePackObject]
public sealed partial class ClanJoinResultWireMsg
{
    [Key(0)] public int Response { get; set; }
}

[MessagePackObject]
public sealed partial class CharInfoClanWireMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public string Name { get; set; }
    [Key(2)] public int X { get; set; }
    [Key(3)] public int Y { get; set; }
    [Key(4)] public int Z { get; set; }
    [Key(5)] public int ClassId { get; set; }
    [Key(6)] public int[] Paperdoll { get; set; }
    [Key(7)] public int ClanId { get; set; }
    [Key(8)] public int ClanCrestId { get; set; }
}

[MessagePackObject]
public sealed partial class CharInfoLegacyWireMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public string Name { get; set; }
    [Key(2)] public int X { get; set; }
    [Key(3)] public int Y { get; set; }
    [Key(4)] public int Z { get; set; }
    [Key(5)] public int ClassId { get; set; }
}

[MessagePackObject]
public sealed partial class CharInfoStringTailWireMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public string Name { get; set; }
    [Key(2)] public int X { get; set; }
    [Key(3)] public int Y { get; set; }
    [Key(4)] public int Z { get; set; }
    [Key(5)] public int ClassId { get; set; }
    [Key(6)] public int[] Paperdoll { get; set; }
    [Key(7)] public string Future { get; set; }
}

[MessagePackObject]
public sealed partial class UserInfoClanWireMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public string Name { get; set; }
    [Key(2)] public int ClassId { get; set; }
    [Key(3)] public int X { get; set; }
    [Key(4)] public int Y { get; set; }
    [Key(5)] public int Z { get; set; }
    [Key(6)] public float RunSpeed { get; set; }
    [Key(7)] public float WalkSpeed { get; set; }
    [Key(8)] public bool Running { get; set; }
    [Key(9)] public UserPaperdollSlotWire[] Paperdoll { get; set; }
    [Key(10)] public int WeaponGradePenalty { get; set; }
    [Key(11)] public int ArmorGradePenalty { get; set; }
    [Key(12)] public int GradePenalty { get; set; }
    [Key(13)] public int ClanId { get; set; }
    [Key(14)] public int ClanCrestId { get; set; }
}

[MessagePackObject]
public sealed partial class UserPaperdollSlotWire
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public int ItemId { get; set; }
}

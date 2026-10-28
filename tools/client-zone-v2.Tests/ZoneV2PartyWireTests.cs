using MessagePack;
using MessagePackObject = MessagePack.MessagePackObjectAttribute;
using Key = MessagePack.KeyAttribute;
using Xunit;

namespace Fineage.Client.ZoneV2.Tests;

public class ZoneV2PartyWireTests
{
    [Fact]
    public void Opcodes_match_the_backend_contract()
    {
        Assert.Equal(15, (ushort)ZoneV2MsgType.UseItem);
        Assert.Equal(16, (ushort)ZoneV2MsgType.UnequipItem);
        Assert.Equal(17, (ushort)ZoneV2MsgType.GiveItem);
        Assert.Equal(18, (ushort)ZoneV2MsgType.PartyInviteRequest);
        Assert.Equal(19, (ushort)ZoneV2MsgType.PartyAnswerJoinRequest);
        Assert.Equal(26, (ushort)ZoneV2MsgType.PartyLeaveRequest);
        Assert.Equal(27, (ushort)ZoneV2MsgType.PartyKickRequest);
        Assert.Equal(1027, (ushort)ZoneV2MsgType.PartyAskJoin);
        Assert.Equal(1028, (ushort)ZoneV2MsgType.PartyJoinResult);
        Assert.Equal(1029, (ushort)ZoneV2MsgType.PartyMemberList);
    }

    [Fact]
    public void Party_invite_and_answer_round_trip()
    {
        byte[] invite = ZoneV2Messages.EncodePartyInviteRequest(2001, 1);
        var inviteMsg = MessagePackSerializer.Deserialize<PartyInviteTestMsg>(invite);
        Assert.Equal(2001, inviteMsg.TargetObjectId);
        Assert.Equal(1, inviteMsg.ItemDistribution);

        byte[] answer = ZoneV2Messages.EncodePartyAnswerJoinRequest(1001, 1);
        var answerMsg = MessagePackSerializer.Deserialize<PartyAnswerTestMsg>(answer);
        Assert.Equal(1001, answerMsg.RequestorObjectId);
        Assert.Equal(1, answerMsg.Response);
    }

    [Fact]
    public void Party_ask_join_and_member_list_decode()
    {
        var ask = new PartyAskJoinTestMsg
        {
            RequestorObjectId = 1001,
            RequestorName = "Hero",
            ItemDistribution = 0
        };
        Assert.True(ZoneV2Messages.TryDecodePartyAskJoin(Pack(ask), out ZoneV2PartyAskJoin decodedAsk));
        Assert.Equal(1001, decodedAsk.RequestorObjectId);
        Assert.Equal("Hero", decodedAsk.RequestorName);

        AskJoinPartyDto dto = AskJoinPartyDto.FromZoneV2(decodedAsk.RequestorObjectId, decodedAsk.RequestorName, decodedAsk.ItemDistribution);
        Assert.Equal(1001, dto.RequestorObjectId);

        var list = new PartyMemberListTestMsg
        {
            LeaderObjectId = 1001,
            PartyId = 7,
            Members =
            [
                new PartyMemberEntryTestMsg
                {
                    ObjectId = 1001,
                    Name = "Hero",
                    ClassId = 0,
                    Level = 1,
                    CurHp = 100,
                    MaxHp = 100,
                    CurMp = 50,
                    MaxMp = 50
                },
                new PartyMemberEntryTestMsg
                {
                    ObjectId = 2001,
                    Name = "Ally",
                    ClassId = 10,
                    Level = 1,
                    CurHp = 80,
                    MaxHp = 100,
                    CurMp = 40,
                    MaxMp = 50
                }
            ]
        };

        Assert.True(ZoneV2Messages.TryDecodePartyMemberList(Pack(list), out ZoneV2PartyMemberList roster));
        Assert.Equal(1001, roster.LeaderObjectId);
        Assert.Equal(2, roster.Members.Length);
        Assert.Contains("Hero", ZoneV2Messages.FormatPartyMemberList(roster));
        Assert.Contains("Ally", ZoneV2Messages.FormatPartyMemberList(roster));
    }

    [Fact]
    public void Party_join_result_decodes()
    {
        var msg = new PartyJoinResultTestMsg { Response = 1 };
        Assert.True(ZoneV2Messages.TryDecodePartyJoinResult(Pack(msg), out int response));
        Assert.Equal(1, response);
    }

    static byte[] Pack<T>(T msg) => MessagePackSerializer.Serialize(msg);

    [MessagePackObject]
    sealed class PartyInviteTestMsg
    {
        [Key(0)] public int TargetObjectId { get; set; }
        [Key(1)] public int ItemDistribution { get; set; }
    }

    [MessagePackObject]
    sealed class PartyAnswerTestMsg
    {
        [Key(0)] public int RequestorObjectId { get; set; }
        [Key(1)] public int Response { get; set; }
    }

    [MessagePackObject]
    sealed class PartyAskJoinTestMsg
    {
        [Key(0)] public int RequestorObjectId { get; set; }
        [Key(1)] public string RequestorName { get; set; }
        [Key(2)] public int ItemDistribution { get; set; }
    }

    [MessagePackObject]
    sealed class PartyJoinResultTestMsg
    {
        [Key(0)] public int Response { get; set; }
    }

    [MessagePackObject]
    sealed class PartyMemberListTestMsg
    {
        [Key(0)] public int LeaderObjectId { get; set; }
        [Key(1)] public int PartyId { get; set; }
        [Key(2)] public PartyMemberEntryTestMsg[] Members { get; set; }
    }

    [MessagePackObject]
    sealed class PartyMemberEntryTestMsg
    {
        [Key(0)] public int ObjectId { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public int ClassId { get; set; }
        [Key(3)] public int Level { get; set; }
        [Key(4)] public int CurHp { get; set; }
        [Key(5)] public int MaxHp { get; set; }
        [Key(6)] public int CurMp { get; set; }
        [Key(7)] public int MaxMp { get; set; }
    }
}

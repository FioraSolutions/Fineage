using Fineage.ZoneServer.Wire;
using MessagePack;
using Xunit;

namespace Fineage.ZoneServer.Tests;

public class WireMessagesTests
{
    [Fact]
    public void HelloMsg_RoundTrips()
    {
        var msg = new HelloMsg { ProtocolVersion = 3, ClientBuild = "abc-123" };
        var bytes = MessagePackSerializer.Serialize(msg);
        var decoded = MessagePackSerializer.Deserialize<HelloMsg>(bytes);

        Assert.Equal(3, decoded.ProtocolVersion);
        Assert.Equal("abc-123", decoded.ClientBuild);
    }

    [Fact]
    public void HelloAckMsg_RoundTrips()
    {
        var msg = new HelloAckMsg { ProtocolVersion = 2, ServerName = "TestZone" };
        var bytes = MessagePackSerializer.Serialize(msg);
        var decoded = MessagePackSerializer.Deserialize<HelloAckMsg>(bytes);

        Assert.Equal(2, decoded.ProtocolVersion);
        Assert.Equal("TestZone", decoded.ServerName);
    }

    [Fact]
    public void AuthMsg_RoundTrips()
    {
        var msg = new AuthMsg { Account = "player1", Password = "secret" };
        var bytes = MessagePackSerializer.Serialize(msg);
        var decoded = MessagePackSerializer.Deserialize<AuthMsg>(bytes);

        Assert.Equal("player1", decoded.Account);
        Assert.Equal("secret", decoded.Password);
    }

    [Fact]
    public void AuthOkMsg_RoundTrips()
    {
        var msg = new AuthOkMsg { SessionId = 42, CharacterName = "Warrior" };
        var bytes = MessagePackSerializer.Serialize(msg);
        var decoded = MessagePackSerializer.Deserialize<AuthOkMsg>(bytes);

        Assert.Equal(42, decoded.SessionId);
        Assert.Equal("Warrior", decoded.CharacterName);
    }

    [Fact]
    public void PlayerSpawnedMsg_RoundTrips()
    {
        var msg = new PlayerSpawnedMsg
        {
            ObjectId = 7,
            Name = "Hero",
            X = -84318,
            Y = 244578,
            Z = -3730,
            RunSpeed = 120f,
            WalkSpeed = 80f,
            Running = true
        };
        var bytes = MessagePackSerializer.Serialize(msg);
        var decoded = MessagePackSerializer.Deserialize<PlayerSpawnedMsg>(bytes);

        Assert.Equal(7, decoded.ObjectId);
        Assert.Equal("Hero", decoded.Name);
        Assert.Equal(-84318, decoded.X);
        Assert.Equal(244578, decoded.Y);
        Assert.Equal(-3730, decoded.Z);
        Assert.Equal(120f, decoded.RunSpeed);
        Assert.Equal(80f, decoded.WalkSpeed);
        Assert.True(decoded.Running);
    }

    [Fact]
    public void MoveToMsg_RoundTrips()
    {
        var msg = new MoveToMsg
        {
            ObjectId = 1,
            DestX = 100,
            DestY = 200,
            DestZ = 300,
            OriginX = 10,
            OriginY = 20,
            OriginZ = 30
        };
        var bytes = MessagePackSerializer.Serialize(msg);
        var decoded = MessagePackSerializer.Deserialize<MoveToMsg>(bytes);

        Assert.Equal(1, decoded.ObjectId);
        Assert.Equal(100, decoded.DestX);
        Assert.Equal(200, decoded.DestY);
        Assert.Equal(300, decoded.DestZ);
        Assert.Equal(10, decoded.OriginX);
        Assert.Equal(20, decoded.OriginY);
        Assert.Equal(30, decoded.OriginZ);
    }

    [Fact]
    public void ErrorMsg_RoundTrips()
    {
        var msg = new ErrorMsg { Code = "auth", Message = "Auth required" };
        var bytes = MessagePackSerializer.Serialize(msg);
        var decoded = MessagePackSerializer.Deserialize<ErrorMsg>(bytes);

        Assert.Equal("auth", decoded.Code);
        Assert.Equal("Auth required", decoded.Message);
    }

    [Fact]
    public void EnterWorldMsg_DefaultSlot_IsZero()
    {
        var msg = new EnterWorldMsg();
        var bytes = MessagePackSerializer.Serialize(msg);
        var decoded = MessagePackSerializer.Deserialize<EnterWorldMsg>(bytes);

        Assert.Equal(0, decoded.CharacterSlot);
    }

    [Fact]
    public void MsgType_Opcodes_AreCorrect()
    {
        Assert.Equal(1, (ushort)MsgType.Hello);
        Assert.Equal(2, (ushort)MsgType.Auth);
        Assert.Equal(3, (ushort)MsgType.EnterWorld);
        Assert.Equal(4, (ushort)MsgType.MoveTo);
        Assert.Equal(5, (ushort)MsgType.SelectTarget);
        Assert.Equal(6, (ushort)MsgType.AttackRequest);
        Assert.Equal(1001, (ushort)MsgType.HelloAck);
        Assert.Equal(1002, (ushort)MsgType.AuthOk);
        Assert.Equal(1003, (ushort)MsgType.PlayerSpawned);
        Assert.Equal(1004, (ushort)MsgType.MoveToAck);
        Assert.Equal(1005, (ushort)MsgType.CharInfo);
        Assert.Equal(1006, (ushort)MsgType.NpcInfo);
        Assert.Equal(1007, (ushort)MsgType.DeleteObject);
        Assert.Equal(1008, (ushort)MsgType.MyTargetSelected);
        Assert.Equal(1009, (ushort)MsgType.Attack);
        Assert.Equal(1010, (ushort)MsgType.StatusUpdate);
        Assert.Equal(1099, (ushort)MsgType.Error);
    }

    [Fact]
    public void CombatMsgs_RoundTrip()
    {
        var target = new MyTargetSelectedMsg { ObjectId = 20_003, ColorHint = 11 };
        var attack = new AttackMsg
        {
            AttackerObjectId = 1,
            TargetObjectId = 20_003,
            Damage = 32,
            AttackerX = 1,
            AttackerY = 2,
            AttackerZ = 3,
            TargetX = 4,
            TargetY = 5,
            TargetZ = 6
        };
        var status = new StatusUpdateMsg
        {
            ObjectId = 20_003,
            AttributeIds = new[] { 0x09, 0x0a },
            Values = new[] { 64, 96 }
        };

        Assert.Equal(20_003,
            MessagePackSerializer.Deserialize<MyTargetSelectedMsg>(MessagePackSerializer.Serialize(target)).ObjectId);
        Assert.Equal(32,
            MessagePackSerializer.Deserialize<AttackMsg>(MessagePackSerializer.Serialize(attack)).Damage);
        Assert.Equal(64,
            MessagePackSerializer.Deserialize<StatusUpdateMsg>(MessagePackSerializer.Serialize(status)).Values[0]);
    }

    [Fact]
    public void PresenceMsgs_RoundTrip()
    {
        var charInfo = new CharInfoMsg
        {
            ObjectId = 9,
            Name = "Hero",
            X = 1,
            Y = 2,
            Z = 3,
            ClassId = 10
        };
        var npc = new NpcInfoMsg { ObjectId = 20_001, NpcTemplateId = 30_001, X = 4, Y = 5, Z = 6 };
        var del = new DeleteObjectMsg { ObjectId = 99 };

        Assert.Equal(charInfo.ObjectId,
            MessagePackSerializer.Deserialize<CharInfoMsg>(MessagePackSerializer.Serialize(charInfo)).ObjectId);
        Assert.Equal(npc.NpcTemplateId,
            MessagePackSerializer.Deserialize<NpcInfoMsg>(MessagePackSerializer.Serialize(npc)).NpcTemplateId);
        Assert.Equal(del.ObjectId,
            MessagePackSerializer.Deserialize<DeleteObjectMsg>(MessagePackSerializer.Serialize(del)).ObjectId);
    }
}

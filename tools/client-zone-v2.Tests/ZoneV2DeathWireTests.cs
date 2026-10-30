using MessagePack;
using Xunit;

namespace Fineage.Client.ZoneV2.Tests;

/// <summary>
/// Death / respawn wire. Layout matches Fineage-Backend <c>DeathProtocol</c> / <c>Messages.cs</c>.
/// </summary>
public class ZoneV2DeathWireTests
{
    [Fact]
    public void Opcodes_match_the_backend_death_contract()
    {
        Assert.Equal(52, (ushort)ZoneV2MsgType.ReviveRequest);
        Assert.Equal(53, (ushort)ZoneV2MsgType.AdminKillPlayer);
        Assert.Equal(1074, (ushort)ZoneV2MsgType.Die);
        Assert.Equal(1075, (ushort)ZoneV2MsgType.Revive);
        Assert.Equal(1076, (ushort)ZoneV2MsgType.TeleportToLocation);
    }

    [Fact]
    public void Revive_request_encodes_village_action()
    {
        byte[] body = ZoneV2Messages.EncodeReviveRequest(ZoneV2Death.ReviveToVillage);
        var msg = MessagePackSerializer.Deserialize<ReviveRequestTestMsg>(body);
        Assert.Equal(0, msg.Action);
    }

    [Fact]
    public void Die_decodes_object_and_killer()
    {
        var msg = new DieTestMsg { ObjectId = 100_001, KillerObjectId = 100_008 };
        Assert.True(ZoneV2Messages.TryDecodeDie(Pack(msg), out ZoneV2Die die));
        Assert.Equal(100_001, die.ObjectId);
        Assert.Equal(100_008, die.KillerObjectId);
    }

    [Fact]
    public void Revive_decodes_object_id()
    {
        var msg = new ReviveTestMsg { ObjectId = 100_001 };
        Assert.True(ZoneV2Messages.TryDecodeRevive(Pack(msg), out ZoneV2Revive revive));
        Assert.Equal(100_001, revive.ObjectId);
    }

    [Fact]
    public void Teleport_decodes_coords()
    {
        var msg = new TeleportTestMsg
        {
            ObjectId = 100_001,
            X = -82687,
            Y = 243157,
            Z = -3734,
            Heading = 0
        };
        Assert.True(ZoneV2Messages.TryDecodeTeleportToLocation(Pack(msg), out ZoneV2TeleportToLocation tp));
        Assert.Equal(-82687, tp.X);
        Assert.Equal(243157, tp.Y);
        Assert.Equal(-3734, tp.Z);
    }

    static byte[] Pack<T>(T msg) => MessagePackSerializer.Serialize(msg);
}

[MessagePackObject]
public sealed class ReviveRequestTestMsg
{
    [Key(0)] public int Action { get; set; }
}

[MessagePackObject]
public sealed class DieTestMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public int KillerObjectId { get; set; }
}

[MessagePackObject]
public sealed class ReviveTestMsg
{
    [Key(0)] public int ObjectId { get; set; }
}

[MessagePackObject]
public sealed class TeleportTestMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public int X { get; set; }
    [Key(2)] public int Y { get; set; }
    [Key(3)] public int Z { get; set; }
    [Key(4)] public int Heading { get; set; }
}

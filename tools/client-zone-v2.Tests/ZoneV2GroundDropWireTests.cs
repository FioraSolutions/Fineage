using MessagePack;
using Xunit;

namespace Fineage.Client.ZoneV2.Tests;

/// <summary>SpawnItem (1018) RX + PickupRequest (14) TX — #451 slice on main.</summary>
public class ZoneV2GroundDropWireTests
{
    [Fact]
    public void Ground_drop_opcodes_match_backend_contract()
    {
        Assert.Equal(1018, (ushort)ZoneV2MsgType.SpawnItem);
        Assert.Equal(14, (ushort)ZoneV2MsgType.PickupRequest);
    }

    [Fact]
    public void Spawn_item_decodes_gremlin_drop_shape()
    {
        var msg = new BackendSpawnItemMsg
        {
            ObjectId = 100_001,
            ItemId = 57,
            X = -84318,
            Y = 244579,
            Z = -3730,
            Stackable = true,
            Count = 25
        };

        Assert.True(ZoneV2Messages.TryDecodeSpawnItem(Pack(msg), out ZoneV2SpawnItem item));
        SpawnItemApplyFields fields = ZoneV2Messages.ToSpawnItemApplyFields(item);
        Assert.Equal(100_001, fields.ObjectId);
        Assert.Equal(57, fields.ItemId);
        Assert.Equal(-84318, fields.X);
        Assert.True(fields.Stackable);
        Assert.Equal(25, fields.Count);
        Assert.Equal(244579, fields.Y);
        Assert.Equal(-3730, fields.Z);
    }

    [Fact]
    public void Pickup_request_body_is_messagepack_object_id()
    {
        byte[] body = ZoneV2Messages.EncodePickupRequest(100_001);
        var msg = MessagePackSerializer.Deserialize<PickupRequestTestMsg>(body);
        Assert.Equal(100_001, msg.ObjectId);
    }

    private static byte[] Pack<T>(T value) =>
        MessagePackSerializer.Serialize(value);
}

[MessagePackObject]
public sealed partial class BackendSpawnItemMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public int ItemId { get; set; }
    [Key(2)] public int X { get; set; }
    [Key(3)] public int Y { get; set; }
    [Key(4)] public int Z { get; set; }
    [Key(5)] public bool Stackable { get; set; }
    [Key(6)] public int Count { get; set; } = 1;
    [Key(7)] public int Unknown { get; set; }
}

[MessagePackObject]
public sealed partial class PickupRequestTestMsg
{
    [Key(0)] public int ObjectId { get; set; }
}

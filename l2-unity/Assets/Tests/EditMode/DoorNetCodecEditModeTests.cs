using Fineage.Net.Contract;
using NUnit.Framework;

public sealed class DoorNetCodecEditModeTests
{
    [Test]
    public void DoorStatus_roundtrip_includes_hallId()
    {
        var msg = new DoorStatusNetMsg
        {
            ObjectId = 1001,
            DoorId = 17220001,
            Open = true,
            CurHp = 500_000,
            MaxHp = 1_000_000,
            Attackable = false,
            Wall = false,
            CastleId = 0,
            HallId = 28,
            UpgradeRatio = 1
        };
        var back = NetCodec.DecodeDoorStatus(NetCodec.EncodeDoorStatus(msg));
        Assert.AreEqual(msg.HallId, back.HallId);
        Assert.AreEqual(0, back.CastleId);
        Assert.AreEqual(msg.DoorId, back.DoorId);
        Assert.IsTrue(back.Open);
    }

    [Test]
    public void DoorStatus_castle_gate_roundtrip()
    {
        var msg = new DoorStatusNetMsg
        {
            ObjectId = 42,
            DoorId = 19210001,
            CastleId = 1,
            HallId = 0,
            UpgradeRatio = 3,
            Attackable = true,
            MaxHp = 2_000_000,
            CurHp = 1_750_000
        };
        var back = NetCodec.DecodeDoorStatus(NetCodec.EncodeDoorStatus(msg));
        Assert.AreEqual(1, back.CastleId);
        Assert.AreEqual(0, back.HallId);
        Assert.AreEqual(3, back.UpgradeRatio);
    }
}
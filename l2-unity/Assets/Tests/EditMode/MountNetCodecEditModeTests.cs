using System;
using Fineage.Net.Contract;
using NUnit.Framework;

public sealed class MountNetCodecEditModeTests
{
    [Test]
    public void Ride_roundtrip()
    {
        var msg = new RideNetMsg { ObjectId = 42, Action = 1, MountKind = 1, RideNpcId = 1012526 };
        byte[] bytes = NetCodec.EncodeRide(msg);
        RideNetMsg back = NetCodec.DecodeRide(bytes);
        Assert.AreEqual(msg.ObjectId, back.ObjectId);
        Assert.AreEqual(msg.MountKind, back.MountKind);
    }

    [Test]
    public void PlayerActionState_roundtrip()
    {
        var msg = new PlayerActionStateNetMsg
        {
            PetOut = true,
            MountablePet = true,
            Mounted = false,
            CanRide = true,
            MountKind = 1,
            PetControlObjectId = 9001
        };
        byte[] bytes = NetCodec.EncodePlayerActionState(msg);
        PlayerActionStateNetMsg back = NetCodec.DecodePlayerActionState(bytes);
        Assert.IsTrue(back.CanRide);
        Assert.AreEqual(9001, back.PetControlObjectId);
    }

    [Test]
    public void SummonInfo_mountable_tail_roundtrip()
    {
        var msg = new SummonInfoNetMsg
        {
            ObjectId = 1,
            Kind = SummonKindNet.Pet,
            NpcId = 12526,
            Dead = false,
            Mountable = true
        };
        byte[] bytes = NetCodec.EncodeSummonInfo(msg);
        SummonInfoNetMsg back = NetCodec.DecodeSummonInfo(bytes);
        Assert.IsTrue(back.Mountable);
    }
}
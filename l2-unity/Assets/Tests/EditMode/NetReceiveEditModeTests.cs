using System;
using System.Threading;
using Fineage.Net.Contract;
using NUnit.Framework;

public sealed class NetReceiveEditModeTests
{
    [Test]
    public void Policy_EntitySpawn_is_never_dropped()
    {
        Assert.AreEqual(
            NetPacketApplyPolicy.Disposition.Preserve,
            NetPacketApplyPolicy.GetDisposition(NetOpcode.EntitySpawn));
    }

    [Test]
    public void Policy_AbnormalStatusUpdate_is_never_dropped()
    {
        Assert.AreEqual(
            NetPacketApplyPolicy.Disposition.Preserve,
            NetPacketApplyPolicy.GetDisposition(NetOpcode.AbnormalStatusUpdate));
    }

    [Test]
    public void Policy_MoveState_coalesces_per_entity()
    {
        Assert.AreEqual(
            NetPacketApplyPolicy.Disposition.Coalesce,
            NetPacketApplyPolicy.GetDisposition(NetOpcode.MoveState));

        var first = new MoveStateMsg { EntityValue = 42 };
        var second = new MoveStateMsg { EntityValue = 42 };
        long keyA = NetPacketApplyPolicy.GetCoalesceKey(NetOpcode.MoveState, first);
        long keyB = NetPacketApplyPolicy.GetCoalesceKey(NetOpcode.MoveState, second);
        Assert.AreEqual(keyA, keyB);

        var other = new MoveStateMsg { EntityValue = 99 };
        long keyC = NetPacketApplyPolicy.GetCoalesceKey(NetOpcode.MoveState, other);
        Assert.AreNotEqual(keyA, keyC);
    }

    [Test]
    public void Policy_SystemMessage_is_discardable()
    {
        Assert.AreEqual(
            NetPacketApplyPolicy.Disposition.Discardable,
            NetPacketApplyPolicy.GetDisposition(NetOpcode.SystemMessage));
    }

    [Test]
    public void Coalesce_lane_keeps_latest_move_state()
    {
        var pump = new NetMainThreadPump();
        int applied = 0;

        var msgA = new MoveStateMsg { EntityValue = 7, DestX = 1 };
        var msgB = new MoveStateMsg { EntityValue = 7, DestX = 2 };
        pump.Post(() => applied++, NetOpcode.MoveState, msgA);
        pump.Post(() => applied++, NetOpcode.MoveState, msgB);

        Assert.AreEqual(1, pump.CoalesceSlotCount);
        pump.DrainFrame();
        Assert.AreEqual(1, applied);
        Assert.AreEqual(1, pump.GetCoalescedCount(NetOpcode.MoveState));
    }

    [Test]
    public void Preserve_lane_survives_discardable_overflow_from_background_thread()
    {
        var pump = new NetMainThreadPump();
        int spawns = 0;
        Exception threadError = null;

        var thread = new Thread(() =>
        {
            try
            {
                for (int i = 0; i < NetMainThreadPump.MaxPendingDiscardable + 64; i++)
                {
                    pump.Post(() => { }, NetOpcode.SystemMessage);
                    pump.Post(() => Interlocked.Increment(ref spawns), NetOpcode.EntitySpawn);
                }
            }
            catch (Exception ex)
            {
                threadError = ex;
            }
        });

        thread.Start();
        thread.Join(TimeSpan.FromSeconds(30));
        Assert.IsFalse(thread.IsAlive);
        Assert.IsNull(threadError);

        pump.DrainFrame();
        while (pump.Depth > 0)
            pump.DrainFrame();

        Assert.Greater(spawns, NetMainThreadPump.MaxPendingDiscardable);
        Assert.Greater(pump.GetDroppedCount(NetOpcode.SystemMessage), 0);
    }

    [Test]
    public void Drain_respects_preserve_ordering()
    {
        var pump = new NetMainThreadPump();
        var order = string.Empty;

        pump.Post(() => order += "a", NetOpcode.EntitySpawn);
        pump.Post(() => order += "b", NetOpcode.EntityDespawn);
        pump.Post(() => order += "c", NetOpcode.InventoryUpdate);

        pump.DrainFrame();
        Assert.AreEqual("abc", order);
    }
}
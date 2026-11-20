using System;
using System.Linq;
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
    public void Policy_SystemMessage_and_CreatureSay_are_preserved()
    {
        Assert.AreEqual(
            NetPacketApplyPolicy.Disposition.Preserve,
            NetPacketApplyPolicy.GetDisposition(NetOpcode.SystemMessage));
        Assert.AreEqual(
            NetPacketApplyPolicy.Disposition.Preserve,
            NetPacketApplyPolicy.GetDisposition(NetOpcode.CreatureSay));
    }

    [Test]
    public void Policy_PlaySound_is_discardable()
    {
        Assert.AreEqual(
            NetPacketApplyPolicy.Disposition.Discardable,
            NetPacketApplyPolicy.GetDisposition(NetOpcode.PlaySound));
    }

    [Test]
    public void Policy_StatusUpdate_uses_merge_attribute_map()
    {
        Assert.AreEqual(
            NetPacketApplyPolicy.CoalesceStrategy.MergeAttributeMap,
            NetPacketApplyPolicy.GetCoalesceStrategy(NetOpcode.StatusUpdate));
    }

    [Test]
    public void Merge_StatusUpdate_unions_partial_attribute_rows()
    {
        var a = new StatusUpdateNetMsg
        {
            EntityValue = 5,
            AttributeIds = new[] { 9 },
            Values = new[] { 100 },
        };
        var b = new StatusUpdateNetMsg
        {
            EntityValue = 5,
            AttributeIds = new[] { 10 },
            Values = new[] { 55 },
        };

        var merged = (StatusUpdateNetMsg)NetPacketApplyPolicy.MergeCoalesceBodies(
            NetOpcode.StatusUpdate,
            a,
            b);

        Assert.AreEqual(2, merged.AttributeIds.Length);
        Assert.AreEqual(2, merged.Values.Length);
        CollectionAssert.AreEquivalent(new[] { 9, 10 }, merged.AttributeIds);
        Assert.AreEqual(100, merged.Values[Array.IndexOf(merged.AttributeIds, 9)]);
        Assert.AreEqual(55, merged.Values[Array.IndexOf(merged.AttributeIds, 10)]);
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
    public void Coalesce_lane_keeps_latest_move_state()
    {
        var pump = new NetMainThreadPump();
        int applied = 0;

        var packetA = new ZoneApplyPacket
        {
            Opcode = NetOpcode.MoveState,
            Body = new MoveStateMsg { EntityValue = 7, DestX = 1 },
        };
        var packetB = new ZoneApplyPacket
        {
            Opcode = NetOpcode.MoveState,
            Body = new MoveStateMsg { EntityValue = 7, DestX = 2 },
        };

        pump.Post(() => applied++, NetOpcode.MoveState, packetA.Body, packetA);
        pump.Post(() => applied++, NetOpcode.MoveState, packetB.Body, packetB);

        Assert.AreEqual(1, pump.CoalesceSlotCount);
        pump.DrainFrame();
        Assert.AreEqual(1, applied);
        Assert.AreEqual(1, pump.GetCoalescedCount(NetOpcode.MoveState));
        Assert.AreEqual(2, ((MoveStateMsg)packetB.Body).DestX);
    }

    [Test]
    public void Coalesce_lane_merges_status_update_attributes_on_apply()
    {
        var pump = new NetMainThreadPump();
        StatusUpdateNetMsg applied = null;

        var packetA = new ZoneApplyPacket
        {
            Opcode = NetOpcode.StatusUpdate,
            Body = new StatusUpdateNetMsg { EntityValue = 1, AttributeIds = new[] { 9 }, Values = new[] { 100 } },
        };
        var packetB = new ZoneApplyPacket
        {
            Opcode = NetOpcode.StatusUpdate,
            Body = new StatusUpdateNetMsg { EntityValue = 1, AttributeIds = new[] { 10 }, Values = new[] { 55 } },
        };

        pump.Post(() => applied = (StatusUpdateNetMsg)packetA.Body, NetOpcode.StatusUpdate, packetA.Body, packetA);
        pump.Post(() => { }, NetOpcode.StatusUpdate, packetB.Body, packetB);

        pump.DrainFrame();

        Assert.NotNull(applied);
        CollectionAssert.AreEquivalent(new[] { 9, 10 }, applied.AttributeIds);
        Assert.AreEqual(100, applied.Values[Array.IndexOf(applied.AttributeIds, 9)]);
        Assert.AreEqual(55, applied.Values[Array.IndexOf(applied.AttributeIds, 10)]);
    }

    [Test]
    public void Discardable_overflow_from_background_thread_does_not_drop_play_sound()
    {
        var pump = new NetMainThreadPump();
        int spawns = 0;
        int sounds = 0;
        Exception threadError = null;

        var thread = new Thread(() =>
        {
            try
            {
                for (int i = 0; i < NetMainThreadPump.MaxPendingDiscardable + 64; i++)
                {
                    pump.Post(() => Interlocked.Increment(ref sounds), NetOpcode.PlaySound);
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
        Assert.Greater(pump.GetDroppedCount(NetOpcode.PlaySound), 0);
        Assert.Less(sounds, NetMainThreadPump.MaxPendingDiscardable + 64);
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

    [Test]
    public void Preserve_high_water_tracks_depth_peak()
    {
        var pump = new NetMainThreadPump();
        for (int i = 0; i < 8; i++)
            pump.Post(() => { }, NetOpcode.EntitySpawn);

        Assert.AreEqual(8, pump.PreserveCount);
        Assert.GreaterOrEqual(pump.PreserveHighWater, 8);
    }
}
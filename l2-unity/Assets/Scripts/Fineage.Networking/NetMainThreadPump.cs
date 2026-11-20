using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

/// <summary>
/// Socket-thread → main-thread apply pump (B3 budget / audit #3 / #9).
/// Preserve queue is never dropped; coalesce lane latest-wins; only discardable packets drop oldest.
/// </summary>
public sealed class NetMainThreadPump
{
    /// <summary>Max zone packet applies per frame excluding uncapped preserve traffic.</summary>
    public const int MaxNormalActionsPerFrame = 96;

    /// <summary>Soft cap on pending discardable applies; excess drops oldest discardable only.</summary>
    public const int MaxPendingDiscardable = 512;

    /// <summary>Legacy name from the pre-policy pump.</summary>
    public const int MaxPendingNormal = MaxPendingDiscardable;

    const int DepthWarnThreshold = 256;
    const double DepthLogCooldownSeconds = 5d;

    static readonly Stopwatch Clock = Stopwatch.StartNew();

    readonly ConcurrentQueue<NetQueuedWork> _preserve = new ConcurrentQueue<NetQueuedWork>();
    readonly ConcurrentQueue<NetQueuedWork> _discardable = new ConcurrentQueue<NetQueuedWork>();

    readonly ConcurrentDictionary<long, NetQueuedWork> _coalescePending = new ConcurrentDictionary<long, NetQueuedWork>();
    readonly ConcurrentQueue<long> _coalesceOrder = new ConcurrentQueue<long>();

    readonly ConcurrentDictionary<ushort, long> _droppedByOpcode = new ConcurrentDictionary<ushort, long>();
    readonly ConcurrentDictionary<ushort, long> _coalescedByOpcode = new ConcurrentDictionary<ushort, long>();
    readonly ConcurrentQueue<string> _deferredWarnings = new ConcurrentQueue<string>();

    double _nextDepthLogTime;
    double _nextDropLogTime;

    public int PreserveCount => _preserve.Count;
    public int DiscardableCount => _discardable.Count;
    public int CoalesceSlotCount => _coalescePending.Count;
    public int Depth => PreserveCount + DiscardableCount + CoalesceSlotCount;

    public long DroppedDiscardable => SumCounts(_droppedByOpcode);
    public long CoalescedReplacements => SumCounts(_coalescedByOpcode);

    /// <summary>Legacy name used by diagnostics.</summary>
    public long DroppedNormal => DroppedDiscardable;

    public long GetDroppedCount(ushort opcode) =>
        _droppedByOpcode.TryGetValue(opcode, out long n) ? n : 0;

    public long GetCoalescedCount(ushort opcode) =>
        _coalescedByOpcode.TryGetValue(opcode, out long n) ? n : 0;

    public void Post(Action action, ushort opcode, object coalesceBody = null)
    {
        if (action == null)
            return;

        var work = new NetQueuedWork(action, opcode, coalesceBody);
        using (NetReceiveProfiling.Queue.Auto())
        {
            switch (NetPacketApplyPolicy.GetDisposition(opcode))
            {
                case NetPacketApplyPolicy.Disposition.Preserve:
                    _preserve.Enqueue(work);
                    break;
                case NetPacketApplyPolicy.Disposition.Coalesce:
                    EnqueueCoalesce(work);
                    break;
                default:
                    EnqueueDiscardable(work);
                    break;
            }
        }
    }

    /// <summary>Preserve-tier enqueue (compat shim for critical call sites).</summary>
    public void PostCritical(Action action, ushort opcode = 0) => Post(action, opcode);

    public void Post(Action action) => Post(action, 0);

    public void DrainFrame()
    {
        using (NetReceiveProfiling.Drain.Auto())
        {
            DrainQueue(_preserve, int.MaxValue);
            int applyBudget = MaxNormalActionsPerFrame;
            applyBudget = DrainCoalesce(applyBudget);
            DrainQueue(_discardable, applyBudget);
            FlushDeferredWarnings();
            MaybeWarnDepth();
        }
    }

    void EnqueueCoalesce(NetQueuedWork work)
    {
        long key = NetPacketApplyPolicy.GetCoalesceKey(work.Opcode, work.CoalesceBody);
        if (key == 0)
        {
            _preserve.Enqueue(work);
            return;
        }

        if (_coalescePending.TryGetValue(key, out NetQueuedWork prior))
        {
            RecordCoalesced(work.Opcode);
            if (!string.Equals(NetPacketApplyPolicy.OpcodeLabel(prior.Opcode), NetPacketApplyPolicy.OpcodeLabel(work.Opcode), StringComparison.Ordinal))
                RecordCoalesced(prior.Opcode);
        }
        else
            _coalesceOrder.Enqueue(key);

        _coalescePending[key] = work;
    }

    void EnqueueDiscardable(NetQueuedWork work)
    {
        int dropped = 0;
        ushort droppedOpcode = 0;
        while (_discardable.Count >= MaxPendingDiscardable && _discardable.TryDequeue(out NetQueuedWork removed))
        {
            dropped++;
            droppedOpcode = removed.Opcode;
            RecordDrop(removed.Opcode);
        }

        if (dropped > 0)
            MaybeWarnDrops(dropped, droppedOpcode);

        _discardable.Enqueue(work);
    }

    int DrainCoalesce(int maxActions)
    {
        int remaining = maxActions;
        while (remaining > 0 && _coalesceOrder.TryDequeue(out long key))
        {
            if (!_coalescePending.TryRemove(key, out NetQueuedWork work))
                continue;
            remaining--;
            RunWork(work);
        }

        return remaining;
    }

    static void DrainQueue(ConcurrentQueue<NetQueuedWork> queue, int maxActions)
    {
        int remaining = maxActions;
        while (remaining > 0 && queue.TryDequeue(out NetQueuedWork work))
        {
            remaining--;
            RunWork(work);
        }
    }

    static void RunWork(NetQueuedWork work)
    {
        try
        {
            using (NetReceiveProfiling.Apply.Auto())
                work.Apply();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    void RecordDrop(ushort opcode) =>
        _droppedByOpcode.AddOrUpdate(opcode, 1, (_, prev) => prev + 1);

    void RecordCoalesced(ushort opcode) =>
        _coalescedByOpcode.AddOrUpdate(opcode, 1, (_, prev) => prev + 1);

    void MaybeWarnDepth()
    {
        int depth = Depth;
        if (depth < DepthWarnThreshold)
            return;

        double now = Clock.Elapsed.TotalSeconds;
        if (now < _nextDepthLogTime)
            return;

        _nextDepthLogTime = now + DepthLogCooldownSeconds;
        Debug.LogWarning(
            $"[Net] main-thread packet queue depth {depth} (preserve {PreserveCount}, coalesce {CoalesceSlotCount}, discardable {DiscardableCount}); "
            + $"apply budget {MaxNormalActionsPerFrame}/frame, discardable soft-cap {MaxPendingDiscardable}.");
    }

    void MaybeWarnDrops(int justDropped, ushort opcode)
    {
        double now = Clock.Elapsed.TotalSeconds;
        if (now < _nextDropLogTime)
            return;

        _nextDropLogTime = now + DepthLogCooldownSeconds;
        _deferredWarnings.Enqueue(
            $"[Net] discardable apply backpressure: dropped {justDropped} oldest (opcode {NetPacketApplyPolicy.OpcodeLabel(opcode)}); "
            + $"total dropped {DroppedDiscardable}; soft-cap {MaxPendingDiscardable}.");
    }

    void FlushDeferredWarnings()
    {
        while (_deferredWarnings.TryDequeue(out string line))
            Debug.LogWarning(line);
    }

    static long SumCounts(ConcurrentDictionary<ushort, long> map)
    {
        long sum = 0;
        foreach (KeyValuePair<ushort, long> kv in map)
            sum += kv.Value;
        return sum;
    }

    internal readonly struct NetQueuedWork
    {
        internal readonly Action Apply;
        internal readonly ushort Opcode;
        internal readonly object CoalesceBody;

        public NetQueuedWork(Action apply, ushort opcode, object coalesceBody = null)
        {
            Apply = apply;
            Opcode = opcode;
            CoalesceBody = coalesceBody;
        }
    }
}
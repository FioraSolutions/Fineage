using System.Collections.Generic;
using Xunit;

namespace Fineage.Client.Domain.Tests;

public class PacketApplyQueueTests
{
    sealed class ImmediatePump : IMainThreadApplyPump
    {
        public readonly List<System.Action> Pending = new List<System.Action>();

        public void QueueOnMainThread(System.Action action)
        {
            if (action != null)
                Pending.Add(action);
        }

        public void Drain()
        {
            while (Pending.Count > 0)
            {
                System.Action next = Pending[0];
                Pending.RemoveAt(0);
                next();
            }
        }
    }

    [Fact]
    public void QueueApply_nested_Queue_runs_inline()
    {
        var pump = new ImmediatePump();
        var queue = new PacketApplyQueue(pump);
        int order = 0;
        int nestedAt = -1;

        queue.QueueApply(() =>
        {
            order = 1;
            queue.Queue(() => nestedAt = order);
            order = 2;
        });

        Assert.Equal(-1, nestedAt);
        pump.Drain();
        Assert.Equal(2, nestedAt);
    }

    [Fact]
    public void apply_depth_is_per_queue_instance()
    {
        var pumpA = new ImmediatePump();
        var pumpB = new ImmediatePump();
        var queueA = new PacketApplyQueue(pumpA);
        var queueB = new PacketApplyQueue(pumpB);
        bool bQueuedNextFrame = false;

        queueA.QueueApply(() =>
        {
            // Sibling queue must not treat this as nested apply on B.
            queueB.Queue(() => bQueuedNextFrame = true);
        });

        pumpA.Drain();
        Assert.False(bQueuedNextFrame);
        Assert.Single(pumpB.Pending);
        pumpB.Drain();
        Assert.True(bQueuedNextFrame);
    }
}

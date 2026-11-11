using System;
using System.Collections.Concurrent;
using UnityEngine;

public partial class EventProcessor : MonoBehaviour
{
    public void WireGameApply(WorldPacketApply worldApply)
    {
        if (worldApply != null)
        {
            _worldApply = worldApply;
            NetworkingPacketServices.Bind(worldApply: worldApply);
        }
    }

    private void Start()
    {
        World world = GameWorld;
        NetworkingPacketServices.Bind(world: world, worldApply: _worldApply);
        if (world != null && world.OfflineMode)
        {
            this.enabled = false;
            return;
        }
    }

    public void QueueEvent(Action action)
    {
        if (action == null)
            return;
        lock (_batchGate)
        {
            _batches[_writeBatch].Enqueue(action);
            System.Threading.Interlocked.Increment(ref _pending);
        }
    }

    private void Update()
    {
        SpawnFrameBudget.BeginFrame();

        ConcurrentQueue<Action> batch;
        lock (_batchGate)
        {
            ConcurrentQueue<Action> write = _batches[_writeBatch];
            if (write.IsEmpty)
                return;

            batch = write;
            _writeBatch ^= 1;
        }

        Action action;
        while (batch.TryDequeue(out action))
        {
            System.Threading.Interlocked.Decrement(ref _pending);
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        PacketApplyLog.EndBatch();
    }
}

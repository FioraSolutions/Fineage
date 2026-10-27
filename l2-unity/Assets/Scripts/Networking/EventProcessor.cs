using System;
using System.Collections.Concurrent;
using UnityEngine;
using VContainer;

public class EventProcessor : MonoBehaviour
{
    [Inject] World _world;
    WorldPacketApply _worldApply;
    private static EventProcessor _instance;
    [System.Obsolete("Use [Inject] instead")]
    public static EventProcessor Instance { get { return _instance; } }

    // Closed batch for one frame. The reader fills only the write buffer.
    // Update swaps once, then applies that buffer to the end. Anything enqueued
    // while the batch runs goes to the other buffer and waits for the next frame.
    private readonly object _batchGate = new object();
    private readonly ConcurrentQueue<Action>[] _batches =
    {
        new ConcurrentQueue<Action>(),
        new ConcurrentQueue<Action>()
    };
    private int _writeBatch;
    private int _pending;

    public int PendingCount
    {
        get { return System.Threading.Volatile.Read(ref _pending); }
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(this);
    }

    public void WireGameApply(WorldPacketApply worldApply)
    {
        if (worldApply != null)
        {
            _worldApply = worldApply;
            NetworkingPacketServices.Bind(worldApply: worldApply);
        }
    }

    private World GameWorld => NetworkingPacketServices.GameWorld;

    private WorldPacketApply WorldApply => NetworkingPacketServices.WorldApply;

    private void Start()
    {
        NetworkingPacketServices.Bind(world: _world, worldApply: _worldApply);
        World world = GameWorld;
        if (world != null && world.OfflineMode)
        {
            this.enabled = false;
            return;
        }
    }

    public void QueueEvent(Action action)
    {
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
                // Debug.LogError("Критическая ошибка EventProcessor: " + ex);
                // LobbyFlowLog.Exception("EventProcessor.Update (Apply on main thread)", ex);
            }
        }

        int pendingMoves = MovementPacketLatest.Count;
        long flushStart = System.Diagnostics.Stopwatch.GetTimestamp();
        PacketApplyLog.Time("MoveLatestFlush", FlushMovementLatest);
        double flushMs = (System.Diagnostics.Stopwatch.GetTimestamp() - flushStart) * 1000.0 / System.Diagnostics.Stopwatch.Frequency;
        MoveCostLog.NoteFlush(pendingMoves, flushMs);
        PacketApplyLog.EndBatch();
    }

    private void FlushMovementLatest()
    {
        WorldPacketApply apply = WorldApply;
        if (apply != null)
            MovementPacketLatest.Flush(apply);
        else
            NetworkingPacketServices.ApplyWorld(MovementPacketLatest.Flush);
    }
}

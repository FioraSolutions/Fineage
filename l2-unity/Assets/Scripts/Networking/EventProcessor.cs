using System;
using System.Collections.Concurrent;
using UnityEngine;

public partial class EventProcessor : MonoBehaviour
{
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

    /// <summary>
    /// Deferred apply must always have a pump. If DI has not produced one yet,
    /// create an app-lifetime host instead of running work inline.
    /// </summary>




    private World GameWorld => NetworkingPacketServices.GameWorld;

    private WorldPacketApply WorldApply => NetworkingPacketServices.WorldApply;



}

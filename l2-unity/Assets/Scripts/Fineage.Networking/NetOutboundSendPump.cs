using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public sealed partial class NetOutboundSendPump : IDisposable
{
    readonly ConcurrentQueue<PendingSend> _queue = new ConcurrentQueue<PendingSend>();
    readonly SemaphoreSlim _signal = new SemaphoreSlim(0);
    readonly object _gate = new object();
    CancellationTokenSource _cts;
    Task _loop;
    Stream _stream;
    bool _disposed;

    struct PendingSend
    {
        public byte[] Frame;
        public TaskCompletionSource<bool> Completion;
        public CancellationToken CallerCt;
    }

    /// <summary>Start (or restart) the writer loop against <paramref name="stream"/>.</summary>





}

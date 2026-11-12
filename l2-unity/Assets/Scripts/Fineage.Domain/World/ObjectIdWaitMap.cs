using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Event-style wait for an object id to appear. Replaces fixed <c>Task.Delay</c> polls.
/// Completes early on <see cref="Notify"/>; times out with null; honors cancellation.
/// </summary>
public sealed class ObjectIdWaitMap<T> where T : class
{
    readonly object _gate = new object();
    readonly Dictionary<int, List<Waiter>> _waiters = new Dictionary<int, List<Waiter>>();

    sealed class Waiter
    {
        public TaskCompletionSource<T> Tcs;
        public CancellationTokenRegistration Ctr;
        public CancellationTokenSource TimeoutCts;
    }

    public Task<T> WaitAsync(
        int objectId,
        Func<int, T> tryGet,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        if (tryGet == null)
            throw new ArgumentNullException(nameof(tryGet));

        T existing = tryGet(objectId);
        if (existing != null)
            return Task.FromResult(existing);

        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<T>(cancellationToken);

        var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        Waiter waiter;
        lock (_gate)
        {
            existing = tryGet(objectId);
            if (existing != null)
                return Task.FromResult(existing);

            waiter = new Waiter
            {
                Tcs = tcs,
                TimeoutCts = timeout > TimeSpan.Zero ? new CancellationTokenSource() : null
            };

            if (!_waiters.TryGetValue(objectId, out List<Waiter> list))
            {
                list = new List<Waiter>(1);
                _waiters[objectId] = list;
            }

            list.Add(waiter);
        }

        if (cancellationToken.CanBeCanceled)
        {
            waiter.Ctr = cancellationToken.Register(() =>
            {
                if (TryRemove(objectId, waiter))
                    tcs.TrySetCanceled(cancellationToken);
            });
        }

        if (waiter.TimeoutCts != null)
        {
            waiter.TimeoutCts.CancelAfter(timeout);
            waiter.TimeoutCts.Token.Register(() =>
            {
                if (TryRemove(objectId, waiter))
                    tcs.TrySetResult(null);
            });
        }

        _ = tcs.Task.ContinueWith(
            _ =>
            {
                waiter.Ctr.Dispose();
                waiter.TimeoutCts?.Dispose();
            },
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);

        return tcs.Task;
    }

    public void Notify(int objectId, T value)
    {
        List<Waiter> list;
        lock (_gate)
        {
            if (!_waiters.TryGetValue(objectId, out list))
                return;
            _waiters.Remove(objectId);
        }

        for (int i = 0; i < list.Count; i++)
        {
            Waiter waiter = list[i];
            waiter.Tcs.TrySetResult(value);
        }
    }

    /// <summary>Fail waiting callers with null (e.g. despawn before spawn observed).</summary>
    public void Fail(int objectId) => Notify(objectId, null);

    /// <summary>Complete every pending wait with null (world clear / destroy).</summary>
    public void CancelAll()
    {
        List<Waiter> all = new List<Waiter>();
        lock (_gate)
        {
            foreach (KeyValuePair<int, List<Waiter>> pair in _waiters)
                all.AddRange(pair.Value);
            _waiters.Clear();
        }

        for (int i = 0; i < all.Count; i++)
            all[i].Tcs.TrySetResult(null);
    }

    bool TryRemove(int objectId, Waiter waiter)
    {
        lock (_gate)
        {
            if (!_waiters.TryGetValue(objectId, out List<Waiter> list))
                return false;
            if (!list.Remove(waiter))
                return false;
            if (list.Count == 0)
                _waiters.Remove(objectId);
            return true;
        }
    }
}

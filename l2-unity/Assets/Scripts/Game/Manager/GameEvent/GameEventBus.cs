using System;
using System.Collections.Generic;

/// <summary>Default <see cref="IGameEventBus"/> — main-thread only, expand EventBus (#143).</summary>
public sealed class GameEventBus : IGameEventBus
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new Dictionary<Type, List<Delegate>>();

    public IDisposable Subscribe<T>(Action<T> handler) where T : struct
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        Type key = typeof(T);
        if (!_handlers.TryGetValue(key, out List<Delegate> list))
        {
            list = new List<Delegate>();
            _handlers[key] = list;
        }

        list.Add(handler);
        return new Subscription(() =>
        {
            if (_handlers.TryGetValue(key, out List<Delegate> current))
                current.Remove(handler);
        });
    }

    public void Publish<T>(in T gameEvent) where T : struct
    {
        if (!_handlers.TryGetValue(typeof(T), out List<Delegate> list) || list.Count == 0)
            return;

        Delegate[] snapshot = list.ToArray();
        for (int i = 0; i < snapshot.Length; i++)
            ((Action<T>)snapshot[i])(gameEvent);
    }

    private sealed class Subscription : IDisposable
    {
        private Action _dispose;

        public Subscription(Action dispose)
        {
            _dispose = dispose;
        }

        public void Dispose()
        {
            _dispose?.Invoke();
            _dispose = null;
        }
    }
}

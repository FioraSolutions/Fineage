using System;

/// <summary>
/// Typed pub/sub for in-game domain events (spawn, death, movement).
/// Wire-level DTOs stay on <see cref="IWorldDomainSink"/>; new client features subscribe here.
/// </summary>
public interface IGameEventBus
{
    IDisposable Subscribe<T>(Action<T> handler) where T : struct;

    void Publish<T>(in T gameEvent) where T : struct;
}

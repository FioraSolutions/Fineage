using System;

/// <summary>
/// Event queue <c>World</c> injects for main-thread apply (#131 S38g).
/// <c>AppLifetimeScope</c> registers <c>EventProcessor</c> as this interface.
/// </summary>
public interface IEventQueue
{
    void QueueEvent(Action action);
}

using System;

/// <summary>
/// Shortcut bar apply hub (#142 batch slice 161).
/// </summary>
public static class ShortcutApplyIncomingPacketServices
{
    public static PlayerShortcuts Shortcuts => IncomingPacketLocatorApplyIncomingPacketServices.Shortcuts;

    public static void Queue(Action action) => IncomingPacketQueueApplyIncomingPacketServices.Queue(action);
}

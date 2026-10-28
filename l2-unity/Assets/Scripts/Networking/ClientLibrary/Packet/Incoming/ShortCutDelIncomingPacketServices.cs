using System;

/// <summary>
/// Shortcut delete wire packet handler entry (#142 batch slice 141).
/// </summary>
public static class ShortCutDelIncomingPacketServices
{
    public static PlayerShortcuts Shortcuts => UiIncomingPacketServices.Shortcuts;

    public static void Queue(Action action) => UiIncomingPacketServices.Queue(action);

    public static void ApplyDel(ShortCutDelDto packet) =>
        Queue(() => Shortcuts.RemoveShotcutLocally(packet.Slot));
}

using System;

/// <summary>
/// Shortcut delete wire packet handler entry (#142 batch slice 141).
/// Apply facade (#142 batch slice 155). Re-homes from <see cref="ShortCutDelIncomingPacketServices"/>.
/// </summary>
public static class ShortCutDelApplyIncomingPacketServices
{
    public static PlayerShortcuts Shortcuts => ShortcutApplyIncomingPacketServices.Shortcuts;

    public static void Queue(Action action) => ShortcutApplyIncomingPacketServices.Queue(action);

    public static void ApplyDel(ShortCutDelDto packet) =>
        Queue(() => Shortcuts.RemoveShotcutLocally(packet.Slot));
}

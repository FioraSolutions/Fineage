using System;

/// <summary>
/// Shortcut bar apply hub (#142 batch slice 161; UI hub slice 167).
/// </summary>
public static class ShortcutApplyIncomingPacketServices
{
    public static PlayerShortcuts Shortcuts => UiApplyIncomingPacketServices.Shortcuts;

    public static void Queue(Action action) => UiApplyIncomingPacketServices.Queue(action);
}

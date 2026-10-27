using System;

/// <summary>
/// Skillbar shortcut wire packet handler access via UiIncomingPacketServices (#142 sub-slice 70).
/// </summary>
public static class ShortcutIncomingPacketServices
{
    public static PlayerShortcuts Shortcuts => UiIncomingPacketServices.Shortcuts;

    public static void Queue(Action action) => UiIncomingPacketServices.Queue(action);

    public static void ApplyInit(ShortCutInitDto packet) =>
        Queue(() => Shortcuts.SetShortcutList(packet.ShortCuts));

    public static void ApplyRegister(ShortCutRegisterDto packet) =>
        Queue(() => Shortcuts.RegisterShortcut(packet.Shortcut));

    public static void ApplyDel(ShortCutDelDto packet) =>
        Queue(() => Shortcuts.RemoveShotcutLocally(packet.Slot));
}

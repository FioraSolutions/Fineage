using System;

/// <summary>
/// Shortcut init wire packet handler entry (#142 batch slice 141).
/// Apply facade (#142 batch slice 155). Re-homes from <see cref="ShortCutInitIncomingPacketServices"/>.
/// </summary>
public static class ShortCutInitApplyIncomingPacketServices
{
    public static PlayerShortcuts Shortcuts => UiIncomingPacketServices.Shortcuts;

    public static void Queue(Action action) => UiIncomingPacketServices.Queue(action);

    public static void ApplyInit(ShortCutInitDto packet) =>
        Queue(() => Shortcuts.SetShortcutList(packet.ShortCuts));
}

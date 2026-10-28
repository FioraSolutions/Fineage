using System;

/// <summary>
/// Shortcut register wire packet handler entry (#142 batch slice 141).
/// </summary>
public static class ShortCutRegisterIncomingPacketServices
{
    public static PlayerShortcuts Shortcuts => UiIncomingPacketServices.Shortcuts;

    public static void Queue(Action action) => UiIncomingPacketServices.Queue(action);

    public static void ApplyRegister(ShortCutRegisterDto packet) =>
        Queue(() => Shortcuts.RegisterShortcut(packet.Shortcut));
}

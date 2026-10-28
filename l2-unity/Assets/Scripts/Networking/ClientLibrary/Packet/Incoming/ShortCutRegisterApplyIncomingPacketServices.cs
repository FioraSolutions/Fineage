using System;

/// <summary>
/// Shortcut register wire packet handler entry (#142 batch slice 141).
/// Apply facade (#142 batch slice 155). Re-homes from <see cref="ShortCutRegisterIncomingPacketServices"/>.
/// </summary>
public static class ShortCutRegisterApplyIncomingPacketServices
{
    public static PlayerShortcuts Shortcuts => UiApplyIncomingPacketServices.Shortcuts;

    public static void Queue(Action action) => UiApplyIncomingPacketServices.Queue(action);

    public static void ApplyRegister(ShortCutRegisterDto packet) =>
        Queue(() => Shortcuts.RegisterShortcut(packet.Shortcut));
}

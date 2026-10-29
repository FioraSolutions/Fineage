using System;

/// <summary>
/// Character create fail wire packet handler entry (#142 batch slice 147; locator queue slice 255).
/// Re-homes char-create-fail apply from <see cref="CharCreateFailIncomingPacketServices"/>; splits from <see cref="CharacterIncomingPacketServices"/> (sub-slice 75 encapsulation).
/// </summary>
public static class CharCreateFailApplyIncomingPacketServices
{
    public static GameManager Manager => LocatorApplyIncomingPacketServices.Manager;

    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyCharCreateFail(CharCreateFailDto packet) =>
        Queue(() => Manager.OnCreateUserFail(packet.Text));
}

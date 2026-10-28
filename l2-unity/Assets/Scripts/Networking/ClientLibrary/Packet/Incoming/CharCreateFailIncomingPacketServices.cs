using System;

/// <summary>
/// Character create fail wire packet handler entry (#142 sub-slice 101).
/// Splits char-create-fail apply out of <see cref="CharacterIncomingPacketServices"/> (sub-slice 75 encapsulation).
/// </summary>
public static class CharCreateFailIncomingPacketServices
{
    public static GameManager Manager => LoginPacketServices.Manager;

    public static void Queue(Action action) => IncomingPacketServices.Queue(action);

    public static void ApplyCharCreateFail(CharCreateFailDto packet) =>
        Queue(() => Manager.OnCreateUserFail(packet.Text));
}

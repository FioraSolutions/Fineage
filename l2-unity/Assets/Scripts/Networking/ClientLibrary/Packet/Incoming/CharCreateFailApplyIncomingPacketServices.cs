using System;

/// <summary>
/// Character create fail wire packet handler entry (#142 batch slice 147).
/// Re-homes char-create-fail apply from <see cref="CharCreateFailIncomingPacketServices"/>; splits from <see cref="CharacterIncomingPacketServices"/> (sub-slice 75 encapsulation).
/// </summary>
public static class CharCreateFailApplyIncomingPacketServices
{
    public static GameManager Manager => LobbyApplyIncomingPacketServices.Manager;

    public static void Queue(Action action) => CharacterApplyIncomingPacketServices.Queue(action);

    public static void ApplyCharCreateFail(CharCreateFailDto packet) =>
        Queue(() => Manager.OnCreateUserFail(packet.Text));
}

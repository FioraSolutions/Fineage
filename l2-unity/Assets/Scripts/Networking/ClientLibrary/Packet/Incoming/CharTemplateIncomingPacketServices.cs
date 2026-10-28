using System;

/// <summary>
/// Character template wire packet handler entry (#142 sub-slice 102).
/// Splits char-template apply out of <see cref="CharacterIncomingPacketServices"/> (sub-slice 75 encapsulation).
/// </summary>
public static class CharTemplateIncomingPacketServices
{
    public static GameManager Manager => LoginPacketServices.Manager;

    public static void Queue(Action action) => IncomingPacketServices.Queue(action);

    public static void ApplyCharTemplate(CharTemplatesDto templates) =>
        Queue(() => Manager.OnCreateUser(templates.PlayerTemplates));
}

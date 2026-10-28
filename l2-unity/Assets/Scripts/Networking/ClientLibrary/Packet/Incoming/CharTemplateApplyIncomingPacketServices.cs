using System;

/// <summary>
/// Character template wire packet handler entry (#142 batch slice 147).
/// Re-homes char-template apply from <see cref="CharTemplateIncomingPacketServices"/>; splits from <see cref="CharacterIncomingPacketServices"/> (sub-slice 75 encapsulation).
/// </summary>
public static class CharTemplateApplyIncomingPacketServices
{
    public static GameManager Manager => LoginApplyIncomingPacketServices.Manager;

    public static void Queue(Action action) => LoginApplyIncomingPacketServices.Queue(action);

    public static void ApplyCharTemplate(CharTemplatesDto templates) =>
        Queue(() => Manager.OnCreateUser(templates.PlayerTemplates));
}

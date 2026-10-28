using System;

/// <summary>
/// Character template wire packet handler entry (#142 batch slice 147; locator queue slice 255).
/// Re-homes char-template apply from <see cref="CharTemplateIncomingPacketServices"/>; splits from <see cref="CharacterIncomingPacketServices"/> (sub-slice 75 encapsulation).
/// </summary>
public static class CharTemplateApplyIncomingPacketServices
{
    public static GameManager Manager => LocatorApplyIncomingPacketServices.Manager;

    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyCharTemplate(CharTemplatesDto templates) =>
        Queue(() => Manager.OnCreateUser(templates.PlayerTemplates));
}

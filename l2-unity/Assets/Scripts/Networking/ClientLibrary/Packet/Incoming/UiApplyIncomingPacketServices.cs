using System;

/// <summary>
/// UI-domain incoming packet handler access via domain apply hubs (#142 sub-slice 50; queue orchestration slice 176).
/// </summary>
public static class UiApplyIncomingPacketServices
{
    public static SkillLearnWindow SkillLearn => SkillApplyIncomingPacketServices.SkillLearn;

    public static DescriptionSkillWindow SkillDesc => SkillApplyIncomingPacketServices.SkillDesc;

    public static SkillListWindow SkillList => SkillApplyIncomingPacketServices.SkillList;

    public static PartyInvitationWindow PartyInvite => PartyApplyIncomingPacketServices.PartyInvite;

    public static RecipeBookWindow RecipeBook => CraftApplyIncomingPacketServices.RecipeBook;

    public static CraftingItemWindow Crafting => CraftApplyIncomingPacketServices.Crafting;

    public static MultiSellWindow MultiSell => MultiSellApplyIncomingPacketServices.MultiSell;

    public static HtmlWindow Html => HtmlApplyIncomingPacketServices.Html;

    public static ShowListWindow ShowList => ShowListApplyIncomingPacketServices.ShowList;

    public static PlayerShortcuts Shortcuts => ShortcutApplyIncomingPacketServices.Shortcuts;

    public static BufferPanel Buffer => BufferApplyIncomingPacketServices.Buffer;

    public static PositionValidationController PositionValidation =>
        PositionApplyIncomingPacketServices.PositionValidation;

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);
}

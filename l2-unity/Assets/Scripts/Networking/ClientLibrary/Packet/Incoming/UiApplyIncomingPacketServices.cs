using System;

/// <summary>
/// UI-domain incoming packet handler access via locator-backed facades (#142 sub-slice 50; slice 165).
/// </summary>
public static class UiApplyIncomingPacketServices
{
    public static SkillLearnWindow SkillLearn => IncomingPacketLocatorApplyIncomingPacketServices.SkillLearn;

    public static DescriptionSkillWindow SkillDesc => IncomingPacketLocatorApplyIncomingPacketServices.SkillDesc;

    public static SkillListWindow SkillList => IncomingPacketLocatorApplyIncomingPacketServices.SkillList;

    public static PartyInvitationWindow PartyInvite =>
        IncomingPacketLocatorApplyIncomingPacketServices.PartyInvite;

    public static RecipeBookWindow RecipeBook => IncomingPacketLocatorApplyIncomingPacketServices.RecipeBook;

    public static CraftingItemWindow Crafting => IncomingPacketLocatorApplyIncomingPacketServices.Crafting;

    public static MultiSellWindow MultiSell => IncomingPacketLocatorApplyIncomingPacketServices.MultiSell;

    public static HtmlWindow Html => IncomingPacketLocatorApplyIncomingPacketServices.Html;

    public static ShowListWindow ShowList => IncomingPacketLocatorApplyIncomingPacketServices.ShowList;

    public static PlayerShortcuts Shortcuts => IncomingPacketLocatorApplyIncomingPacketServices.Shortcuts;

    public static BufferPanel Buffer => IncomingPacketLocatorApplyIncomingPacketServices.Buffer;

    public static PositionValidationController PositionValidation =>
        IncomingPacketLocatorApplyIncomingPacketServices.PositionValidation;

    public static void Queue(Action action) =>
        IncomingPacketQueueApplyIncomingPacketServices.Queue(action);
}

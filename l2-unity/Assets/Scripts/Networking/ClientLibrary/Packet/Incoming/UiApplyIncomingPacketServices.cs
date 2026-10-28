using System;

/// <summary>
/// UI-domain incoming packet handler access via domain apply hubs (#142 sub-slice 50; UI locator hub slice 251).
/// </summary>
public static class UiApplyIncomingPacketServices
{
    public static SkillLearnWindow SkillLearn => LocatorApplyIncomingPacketServices.SkillLearn;

    public static DescriptionSkillWindow SkillDesc => LocatorApplyIncomingPacketServices.SkillDesc;

    public static SkillListWindow SkillList => LocatorApplyIncomingPacketServices.SkillList;

    public static PartyInvitationWindow PartyInvite =>
        LocatorApplyIncomingPacketServices.PartyInvite;

    public static RecipeBookWindow RecipeBook => LocatorApplyIncomingPacketServices.RecipeBook;

    public static CraftingItemWindow Crafting => LocatorApplyIncomingPacketServices.Crafting;

    public static MultiSellWindow MultiSell => LocatorApplyIncomingPacketServices.MultiSell;

    public static HtmlWindow Html => LocatorApplyIncomingPacketServices.Html;

    public static ShowListWindow ShowList => LocatorApplyIncomingPacketServices.ShowList;

    public static PlayerShortcuts Shortcuts => LocatorApplyIncomingPacketServices.Shortcuts;

    public static BufferPanel Buffer => LocatorApplyIncomingPacketServices.Buffer;

    public static PositionValidationController PositionValidation =>
        LocatorApplyIncomingPacketServices.PositionValidation;

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);
}

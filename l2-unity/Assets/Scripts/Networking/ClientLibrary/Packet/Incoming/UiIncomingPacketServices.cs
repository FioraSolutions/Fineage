using System;

/// <summary>
/// UI-domain incoming packet handler access via facades and IncomingPacketServices queue fallback (#142 sub-slice 50).
/// </summary>
public static class UiIncomingPacketServices
{
    public static SkillLearnWindow SkillLearn => SkillLearnPacketServices.SkillLearn;

    public static DescriptionSkillWindow SkillDesc => SkillLearnPacketServices.SkillDesc;

    public static SkillListWindow SkillList => SkillLearnPacketServices.SkillList;

    public static PartyInvitationWindow PartyInvite => PartyPacketServices.PartyInvite;

    public static RecipeBookWindow RecipeBook => CraftPacketServices.RecipeBook;

    public static CraftingItemWindow Crafting => CraftPacketServices.Crafting;

    public static MultiSellWindow MultiSell => MultiSellPacketServices.MultiSell;

    public static HtmlWindow Html => GameWindowPacketServices.Html;

    public static ShowListWindow ShowList => GameWindowPacketServices.ShowList;

    public static PlayerShortcuts Shortcuts => ShortcutPacketServices.Shortcuts;

    public static BufferPanel Buffer => AuxiliaryGameUIPacketServices.Buffer;

    public static PositionValidationController PositionValidation =>
        ControllerPacketServices.PositionValidation;

    public static void Queue(Action action) => IncomingPacketServices.Queue(action);
}

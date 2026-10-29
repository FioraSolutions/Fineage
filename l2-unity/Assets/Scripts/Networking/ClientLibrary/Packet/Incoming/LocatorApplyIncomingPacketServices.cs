/// <summary>
/// DI locator facade for incoming handler hubs (#142 batch slice 173; bind via <see cref="BindApplyIncomingPacketServices"/> slice 174).
/// </summary>
public static class LocatorApplyIncomingPacketServices
{
    public static void Bind(
        GameClient game = null,
        GameManager manager = null,
        World gameWorld = null,
        WorldPacketApply worldApply = null,
        MessagePacketApply messageApply = null,
        IWorldDomainSink domain = null,
        PlayerInventory inventory = null,
        TargetManager targets = null,
        PlayerShortcuts shortcuts = null,
        PositionValidationController positionValidation = null,
        LoginWindow loginWindow = null,
        CharSelectWindow charSelect = null,
        CharacterSelector characters = null,
        HtmlWindow html = null,
        DealerWindow dealer = null,
        MultiSellWindow multiSell = null,
        TradeWindow trade = null,
        TradeRequestWindow tradeRequest = null,
        PartyInvitationWindow partyInvite = null,
        EnchantWindow enchant = null,
        RecipeBookWindow recipeBook = null,
        CraftingItemWindow crafting = null,
        SkillLearnWindow skillLearn = null,
        DescriptionSkillWindow skillDesc = null,
        SkillListWindow skillList = null,
        QuestWindow quest = null,
        QuestListWindow questList = null,
        ClanWindow clan = null,
        ShowListWindow showList = null,
        SeedInfoWindow seedInfo = null,
        SellCropListWindow sellCrop = null,
        BufferPanel buffer = null) =>
        IncomingPacketLocatorApplyIncomingPacketServices.Bind(
            game: game,
            manager: manager,
            gameWorld: gameWorld,
            worldApply: worldApply,
            messageApply: messageApply,
            domain: domain,
            inventory: inventory,
            targets: targets,
            shortcuts: shortcuts,
            positionValidation: positionValidation,
            loginWindow: loginWindow,
            charSelect: charSelect,
            characters: characters,
            html: html,
            dealer: dealer,
            multiSell: multiSell,
            trade: trade,
            tradeRequest: tradeRequest,
            partyInvite: partyInvite,
            enchant: enchant,
            recipeBook: recipeBook,
            crafting: crafting,
            skillLearn: skillLearn,
            skillDesc: skillDesc,
            skillList: skillList,
            quest: quest,
            questList: questList,
            clan: clan,
            showList: showList,
            seedInfo: seedInfo,
            sellCrop: sellCrop,
            buffer: buffer);

    public static GameClient Game => IncomingPacketLocatorApplyIncomingPacketServices.Game;

    public static GameManager Manager => IncomingPacketLocatorApplyIncomingPacketServices.Manager;

    public static World GameWorld => IncomingPacketLocatorApplyIncomingPacketServices.GameWorld;

    public static WorldPacketApply WorldApply => IncomingPacketLocatorApplyIncomingPacketServices.WorldApply;

    public static MessagePacketApply MessageApply => IncomingPacketLocatorApplyIncomingPacketServices.MessageApply;

    public static IWorldDomainSink Domain => IncomingPacketLocatorApplyIncomingPacketServices.Domain;

    public static PlayerInventory Inventory => IncomingPacketLocatorApplyIncomingPacketServices.Inventory;

    public static TargetManager Targets => IncomingPacketLocatorApplyIncomingPacketServices.Targets;

    public static PlayerShortcuts Shortcuts => IncomingPacketLocatorApplyIncomingPacketServices.Shortcuts;

    public static PositionValidationController PositionValidation =>
        IncomingPacketLocatorApplyIncomingPacketServices.PositionValidation;

    public static LoginWindow LoginWindow => IncomingPacketLocatorApplyIncomingPacketServices.LoginWindow;

    public static CharSelectWindow CharSelect => IncomingPacketLocatorApplyIncomingPacketServices.CharSelect;

    public static CharacterSelector Characters => IncomingPacketLocatorApplyIncomingPacketServices.Characters;

    public static HtmlWindow Html => IncomingPacketLocatorApplyIncomingPacketServices.Html;

    public static DealerWindow Dealer => IncomingPacketLocatorApplyIncomingPacketServices.Dealer;

    public static MultiSellWindow MultiSell => IncomingPacketLocatorApplyIncomingPacketServices.MultiSell;

    public static TradeWindow Trade => IncomingPacketLocatorApplyIncomingPacketServices.Trade;

    public static TradeRequestWindow TradeRequest =>
        IncomingPacketLocatorApplyIncomingPacketServices.TradeRequest;

    public static PartyInvitationWindow PartyInvite =>
        IncomingPacketLocatorApplyIncomingPacketServices.PartyInvite;

    public static EnchantWindow Enchant => IncomingPacketLocatorApplyIncomingPacketServices.Enchant;

    public static RecipeBookWindow RecipeBook => IncomingPacketLocatorApplyIncomingPacketServices.RecipeBook;

    public static CraftingItemWindow Crafting => IncomingPacketLocatorApplyIncomingPacketServices.Crafting;

    public static SkillLearnWindow SkillLearn => IncomingPacketLocatorApplyIncomingPacketServices.SkillLearn;

    public static DescriptionSkillWindow SkillDesc => IncomingPacketLocatorApplyIncomingPacketServices.SkillDesc;

    public static SkillListWindow SkillList => IncomingPacketLocatorApplyIncomingPacketServices.SkillList;

    public static QuestWindow Quest => IncomingPacketLocatorApplyIncomingPacketServices.Quest;

    public static QuestListWindow QuestList => IncomingPacketLocatorApplyIncomingPacketServices.QuestList;

    public static ClanWindow Clan => IncomingPacketLocatorApplyIncomingPacketServices.Clan;

    public static ShowListWindow ShowList => IncomingPacketLocatorApplyIncomingPacketServices.ShowList;

    public static SeedInfoWindow SeedInfo => IncomingPacketLocatorApplyIncomingPacketServices.SeedInfo;

    public static SellCropListWindow SellCrop => IncomingPacketLocatorApplyIncomingPacketServices.SellCrop;

    public static BufferPanel Buffer => IncomingPacketLocatorApplyIncomingPacketServices.Buffer;
}

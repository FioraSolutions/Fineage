using System;

/// <summary>
/// Incoming handler access to game services with DI wiring and IncomingPacketActions fallback (#142).
/// Locator, queue, and bind apply facades re-homed in batch slices 158–159 and 174; compatibility entry.
/// </summary>
public static class IncomingPacketServices
{
    public static void Bind(
        GameClient game = null,
        LoginClient login = null,
        GameManager manager = null,
        World gameWorld = null,
        WorldPacketApply worldApply = null,
        MessagePacketApply messageApply = null,
        IWorldDomainSink domain = null,
        PacketApplyQueue applyQueue = null,
        EventProcessor events = null,
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
        BufferPanel buffer = null)
    {
        BindApplyIncomingPacketServices.Bind(
            game: game,
            login: login,
            manager: manager,
            gameWorld: gameWorld,
            worldApply: worldApply,
            messageApply: messageApply,
            domain: domain,
            applyQueue: applyQueue,
            events: events,
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
    }

    public static GameClient Game => LocatorApplyIncomingPacketServices.Game;

    public static LoginClient Login => LocatorApplyIncomingPacketServices.Login;

    public static GameManager Manager => LocatorApplyIncomingPacketServices.Manager;

    public static World GameWorld => LocatorApplyIncomingPacketServices.GameWorld;

    public static WorldPacketApply WorldApply => LocatorApplyIncomingPacketServices.WorldApply;

    public static MessagePacketApply MessageApply => LocatorApplyIncomingPacketServices.MessageApply;

    public static IWorldDomainSink Domain => LocatorApplyIncomingPacketServices.Domain;

    public static PlayerInventory Inventory => LocatorApplyIncomingPacketServices.Inventory;

    public static TargetManager Targets => LocatorApplyIncomingPacketServices.Targets;

    public static PlayerShortcuts Shortcuts => LocatorApplyIncomingPacketServices.Shortcuts;

    public static PositionValidationController PositionValidation =>
        LocatorApplyIncomingPacketServices.PositionValidation;

    public static LoginWindow LoginWindow => LocatorApplyIncomingPacketServices.LoginWindow;

    public static CharSelectWindow CharSelect => LocatorApplyIncomingPacketServices.CharSelect;

    public static CharacterSelector Characters => LocatorApplyIncomingPacketServices.Characters;

    public static HtmlWindow Html => LocatorApplyIncomingPacketServices.Html;

    public static DealerWindow Dealer => LocatorApplyIncomingPacketServices.Dealer;

    public static MultiSellWindow MultiSell => LocatorApplyIncomingPacketServices.MultiSell;

    public static TradeWindow Trade => LocatorApplyIncomingPacketServices.Trade;

    public static TradeRequestWindow TradeRequest => LocatorApplyIncomingPacketServices.TradeRequest;

    public static PartyInvitationWindow PartyInvite => LocatorApplyIncomingPacketServices.PartyInvite;

    public static EnchantWindow Enchant => LocatorApplyIncomingPacketServices.Enchant;

    public static RecipeBookWindow RecipeBook => LocatorApplyIncomingPacketServices.RecipeBook;

    public static CraftingItemWindow Crafting => LocatorApplyIncomingPacketServices.Crafting;

    public static SkillLearnWindow SkillLearn => LocatorApplyIncomingPacketServices.SkillLearn;

    public static DescriptionSkillWindow SkillDesc => LocatorApplyIncomingPacketServices.SkillDesc;

    public static SkillListWindow SkillList => LocatorApplyIncomingPacketServices.SkillList;

    public static QuestWindow Quest => LocatorApplyIncomingPacketServices.Quest;

    public static QuestListWindow QuestList => LocatorApplyIncomingPacketServices.QuestList;

    public static ClanWindow Clan => LocatorApplyIncomingPacketServices.Clan;

    public static ShowListWindow ShowList => LocatorApplyIncomingPacketServices.ShowList;

    public static SeedInfoWindow SeedInfo => LocatorApplyIncomingPacketServices.SeedInfo;

    public static SellCropListWindow SellCrop => LocatorApplyIncomingPacketServices.SellCrop;

    public static BufferPanel Buffer => LocatorApplyIncomingPacketServices.Buffer;

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        QueueApplyIncomingPacketServices.ApplyWorld(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        QueueApplyIncomingPacketServices.QueueWorld(apply);

    public static void ApplyDomain(Action<IWorldDomainSink> apply) =>
        QueueApplyIncomingPacketServices.ApplyDomain(apply);

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        QueueApplyIncomingPacketServices.QueueDomain(apply);

    public static void ApplyMessage(Action<MessagePacketApply> apply) =>
        QueueApplyIncomingPacketServices.ApplyMessage(apply);

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);
}

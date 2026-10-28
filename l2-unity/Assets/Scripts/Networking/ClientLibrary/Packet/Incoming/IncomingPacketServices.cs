using System;

/// <summary>
/// Incoming handler access to game services with DI wiring and IncomingPacketActions fallback (#142).
/// </summary>
public static class IncomingPacketServices
{
    private static GameClient _game;
    private static LoginClient _login;
    private static GameManager _manager;
    private static World _gameWorld;
    private static WorldPacketApply _worldApply;
    private static MessagePacketApply _messageApply;
    private static IWorldDomainSink _domain;
    private static PlayerInventory _inventory;
    private static TargetManager _targets;
    private static PlayerShortcuts _shortcuts;
    private static PositionValidationController _positionValidation;
    private static LoginWindow _loginWindow;
    private static CharSelectWindow _charSelect;
    private static CharacterSelector _characters;
    private static HtmlWindow _html;
    private static DealerWindow _dealer;
    private static MultiSellWindow _multiSell;
    private static TradeWindow _trade;
    private static TradeRequestWindow _tradeRequest;
    private static PartyInvitationWindow _partyInvite;
    private static EnchantWindow _enchant;
    private static RecipeBookWindow _recipeBook;
    private static CraftingItemWindow _crafting;
    private static SkillLearnWindow _skillLearn;
    private static DescriptionSkillWindow _skillDesc;
    private static SkillListWindow _skillList;
    private static QuestWindow _quest;
    private static QuestListWindow _questList;
    private static ClanWindow _clan;
    private static ShowListWindow _showList;
    private static SeedInfoWindow _seedInfo;
    private static SellCropListWindow _sellCrop;
    private static BufferPanel _buffer;

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
        if (game != null)
            _game = game;
        if (login != null)
            _login = login;
        if (manager != null)
            _manager = manager;
        if (gameWorld != null)
            _gameWorld = gameWorld;
        if (worldApply != null)
            _worldApply = worldApply;
        if (messageApply != null)
            _messageApply = messageApply;
        if (domain != null)
            _domain = domain;
        IncomingPacketQueueApplyIncomingPacketServices.Bind(applyQueue: applyQueue, events: events);
        if (inventory != null)
            _inventory = inventory;
        if (targets != null)
            _targets = targets;
        if (shortcuts != null)
            _shortcuts = shortcuts;
        if (positionValidation != null)
            _positionValidation = positionValidation;
        if (loginWindow != null)
            _loginWindow = loginWindow;
        if (charSelect != null)
            _charSelect = charSelect;
        if (characters != null)
            _characters = characters;
        if (html != null)
            _html = html;
        if (dealer != null)
            _dealer = dealer;
        if (multiSell != null)
            _multiSell = multiSell;
        if (trade != null)
            _trade = trade;
        if (tradeRequest != null)
            _tradeRequest = tradeRequest;
        if (partyInvite != null)
            _partyInvite = partyInvite;
        if (enchant != null)
            _enchant = enchant;
        if (recipeBook != null)
            _recipeBook = recipeBook;
        if (crafting != null)
            _crafting = crafting;
        if (skillLearn != null)
            _skillLearn = skillLearn;
        if (skillDesc != null)
            _skillDesc = skillDesc;
        if (skillList != null)
            _skillList = skillList;
        if (quest != null)
            _quest = quest;
        if (questList != null)
            _questList = questList;
        if (clan != null)
            _clan = clan;
        if (showList != null)
            _showList = showList;
        if (seedInfo != null)
            _seedInfo = seedInfo;
        if (sellCrop != null)
            _sellCrop = sellCrop;
        if (buffer != null)
            _buffer = buffer;
    }

    public static GameClient Game =>
        _game != null ? _game : IncomingPacketActions.Game;

    public static LoginClient Login =>
        _login != null ? _login : IncomingPacketActions.Login;

    public static GameManager Manager =>
        _manager != null ? _manager : IncomingPacketActions.Manager;

    public static World GameWorld =>
        _gameWorld != null ? _gameWorld : IncomingPacketActions.GameWorld;

    public static WorldPacketApply WorldApply =>
        _worldApply != null ? _worldApply : IncomingPacketActions.WorldApply;

    public static MessagePacketApply MessageApply =>
        _messageApply != null ? _messageApply : IncomingPacketActions.MessageApply;

    public static IWorldDomainSink Domain =>
        _domain != null ? _domain : IncomingPacketActions.Domain;

    public static PlayerInventory Inventory =>
        _inventory != null ? _inventory : IncomingPacketActions.Inventory;

    public static TargetManager Targets =>
        _targets != null ? _targets : IncomingPacketActions.Targets;

    public static PlayerShortcuts Shortcuts =>
        _shortcuts != null ? _shortcuts : IncomingPacketActions.Shortcuts;

    public static PositionValidationController PositionValidation =>
        _positionValidation != null ? _positionValidation : IncomingPacketActions.PositionValidation;

    public static LoginWindow LoginWindow =>
        _loginWindow != null ? _loginWindow : IncomingPacketActions.LoginWindow;

    public static CharSelectWindow CharSelect =>
        _charSelect != null ? _charSelect : IncomingPacketActions.CharSelect;

    public static CharacterSelector Characters =>
        _characters != null ? _characters : IncomingPacketActions.Characters;

    public static HtmlWindow Html =>
        _html != null ? _html : IncomingPacketActions.Html;

    public static DealerWindow Dealer =>
        _dealer != null ? _dealer : IncomingPacketActions.Dealer;

    public static MultiSellWindow MultiSell =>
        _multiSell != null ? _multiSell : IncomingPacketActions.MultiSell;

    public static TradeWindow Trade =>
        _trade != null ? _trade : IncomingPacketActions.Trade;

    public static TradeRequestWindow TradeRequest =>
        _tradeRequest != null ? _tradeRequest : IncomingPacketActions.TradeRequest;

    public static PartyInvitationWindow PartyInvite =>
        _partyInvite != null ? _partyInvite : IncomingPacketActions.PartyInvite;

    public static EnchantWindow Enchant =>
        _enchant != null ? _enchant : IncomingPacketActions.Enchant;

    public static RecipeBookWindow RecipeBook =>
        _recipeBook != null ? _recipeBook : IncomingPacketActions.RecipeBook;

    public static CraftingItemWindow Crafting =>
        _crafting != null ? _crafting : IncomingPacketActions.Crafting;

    public static SkillLearnWindow SkillLearn =>
        _skillLearn != null ? _skillLearn : IncomingPacketActions.SkillLearn;

    public static DescriptionSkillWindow SkillDesc =>
        _skillDesc != null ? _skillDesc : IncomingPacketActions.SkillDesc;

    public static SkillListWindow SkillList =>
        _skillList != null ? _skillList : IncomingPacketActions.SkillList;

    public static QuestWindow Quest =>
        _quest != null ? _quest : IncomingPacketActions.Quest;

    public static QuestListWindow QuestList =>
        _questList != null ? _questList : IncomingPacketActions.QuestList;

    public static ClanWindow Clan =>
        _clan != null ? _clan : IncomingPacketActions.Clan;

    public static ShowListWindow ShowList =>
        _showList != null ? _showList : IncomingPacketActions.ShowList;

    public static SeedInfoWindow SeedInfo =>
        _seedInfo != null ? _seedInfo : IncomingPacketActions.SeedInfo;

    public static SellCropListWindow SellCrop =>
        _sellCrop != null ? _sellCrop : IncomingPacketActions.SellCrop;

    public static BufferPanel Buffer =>
        _buffer != null ? _buffer : IncomingPacketActions.Buffer;

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.ApplyWorld(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.QueueWorld(apply);

    public static void ApplyDomain(Action<IWorldDomainSink> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.ApplyDomain(apply);

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.QueueDomain(apply);

    public static void ApplyMessage(Action<MessagePacketApply> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.ApplyMessage(apply);

    public static void Queue(Action action) =>
        IncomingPacketQueueApplyIncomingPacketServices.Queue(action);
}

/// <summary>
/// DI-backed service locator for incoming packet handlers (#142).
/// Holds bind state; peer PacketServices / live Instances are the fallback.
/// </summary>
public static class LocatorApplyIncomingPacketServices
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
        _game != null ? _game : DiRuntimePacketServices.Game;

    public static LoginClient Login =>
        _login != null ? _login : NetworkingPacketServices.Login;

    public static GameManager Manager =>
        _manager != null ? _manager : NetworkingPacketServices.Manager;

    public static World GameWorld =>
        _gameWorld != null ? _gameWorld : NetworkingPacketServices.GameWorld;

    public static WorldPacketApply WorldApply =>
        _worldApply != null ? _worldApply : NetworkingPacketServices.WorldApply;

    // Bound here first; avoid MessagePacketServices (it falls back to this locator).
    public static MessagePacketApply MessageApply => _messageApply;

    public static IWorldDomainSink Domain =>
        _domain != null ? _domain : NetworkingPacketServices.Domain;

    public static PlayerInventory Inventory =>
        _inventory != null ? _inventory : InventoryPacketServices.Inventory;

    public static TargetManager Targets =>
        _targets != null ? _targets : ControllerPacketServices.Targets;

    public static PlayerShortcuts Shortcuts =>
        _shortcuts != null ? _shortcuts : ShortcutPacketServices.Shortcuts;

    public static PositionValidationController PositionValidation =>
        _positionValidation != null
            ? _positionValidation
            : ControllerPacketServices.PositionValidation;

    public static LoginWindow LoginWindow =>
        _loginWindow != null ? _loginWindow : LoginPacketServices.LoginWindow;

    public static CharSelectWindow CharSelect =>
        _charSelect != null ? _charSelect : LoginPacketServices.CharSelect;

    public static CharacterSelector Characters =>
        _characters != null ? _characters : LoginPacketServices.Characters;

    public static HtmlWindow Html =>
        _html != null ? _html : GameWindowPacketServices.Html;

    public static DealerWindow Dealer =>
        _dealer != null ? _dealer : MerchantPacketServices.Dealer;

    public static MultiSellWindow MultiSell =>
        _multiSell != null ? _multiSell : MultiSellPacketServices.MultiSell;

    public static TradeWindow Trade =>
        _trade != null ? _trade : TradePacketServices.Trade;

    public static TradeRequestWindow TradeRequest =>
        _tradeRequest != null ? _tradeRequest : TradePacketServices.TradeRequest;

    public static PartyInvitationWindow PartyInvite =>
        _partyInvite != null ? _partyInvite : PartyPacketServices.PartyInvite;

    public static EnchantWindow Enchant =>
        _enchant != null ? _enchant : ItemEnchantPacketServices.Enchant;

    public static RecipeBookWindow RecipeBook =>
        _recipeBook != null ? _recipeBook : CraftPacketServices.RecipeBook;

    public static CraftingItemWindow Crafting =>
        _crafting != null ? _crafting : CraftPacketServices.Crafting;

    public static SkillLearnWindow SkillLearn =>
        _skillLearn != null ? _skillLearn : SkillLearnPacketServices.SkillLearn;

    public static DescriptionSkillWindow SkillDesc =>
        _skillDesc != null ? _skillDesc : SkillLearnPacketServices.SkillDesc;

    public static SkillListWindow SkillList =>
        _skillList != null ? _skillList : SkillLearnPacketServices.SkillList;

    public static QuestWindow Quest =>
        _quest != null ? _quest : QuestPacketServices.Quest;

    public static ClanWindow Clan =>
        _clan != null ? _clan : ClanPacketServices.Clan;

    public static ShowListWindow ShowList =>
        _showList != null ? _showList : GameWindowPacketServices.ShowList;

    public static SeedInfoWindow SeedInfo =>
        _seedInfo != null ? _seedInfo : AuxiliaryGameUIPacketServices.SeedInfo;

    public static SellCropListWindow SellCrop =>
        _sellCrop != null ? _sellCrop : AuxiliaryGameUIPacketServices.SellCrop;

    public static BufferPanel Buffer =>
        _buffer != null ? _buffer : ControllerPacketServices.Buffer;
}

using VContainer;

public sealed class GameRuntime
{
    private PlayerController _player;
    private PlayerActions _actions;
    private World _world;
    private PlayerInventory _inventory;
    private TargetManager _targets;
    private HitManager _hits;
    private ClickManager _clicks;
    private PlayerShortcuts _shortcuts;
    private InputManager _input;
    private PositionValidationController _positionValidation;
    private EffectManager _effects;
    private EventBus _bus;
    private IGameEventBus _gameEvents;
    private CameraController _camera;
    private IMoveAllCharacters _moves;
    private CombatFacingService _facing;
    private IAnimationManager _animations;
    private SkillExecutor _skillRunner;
    private L2GameUI _ui;
    private ChatWindow _chat;
    private HtmlWindow _html;
    private DealerWindow _dealer;
    private MultiSellWindow _multiSell;
    private TradeWindow _trade;
    private TradeRequestWindow _tradeRequest;
    private PartyInvitationWindow _partyInvite;
    private EnchantWindow _enchant;
    private RecipeBookWindow _recipeBook;
    private CraftingItemWindow _crafting;
    private SkillLearnWindow _skillLearn;
    private DescriptionSkillWindow _skillDesc;
    private SkillListWindow _skillList;
    private QuestWindow _quest;
    private ClanWindow _clan;
    private ShowListWindow _showList;
    private SeedInfoWindow _seedInfo;
    private SellCropListWindow _sellCrop;
    private SkillbarWindow _skillbar;
    private SystemMessageWindow _systemMessageUi;
    private BufferPanel _buffer;
    private InventoryWindow _inventoryUi;
    private DeadWindow _dead;
    private StatusWindow _status;

    private EventProcessor _events;
    private GameClient _game;
    private LoginClient _login;
    private GameManager _manager;

    public EventProcessor Events => _events;

    public GameClient Game => DiRuntimePacketServices.Game;

    public LoginClient Login => DiRuntimePacketServices.Login;

    public GameManager Manager => DiRuntimePacketServices.Manager;

    public World World => _world;
    public PlayerInventory Inventory { get { return Live(ref _inventory, InventoryPacketServices.Inventory); } }
    public TargetManager Targets { get { return Live(ref _targets, TargetPacketServices.Targets); } }
    public HitManager Hits { get { return Live(ref _hits, HitManager.Instance); } }
    public ClickManager Clicks { get { return Live(ref _clicks, ClickManager.Instance); } }
    public PlayerShortcuts Shortcuts { get { return Live(ref _shortcuts, PlayerShortcuts.Instance); } }
#pragma warning disable CS0618 // Locator fallback until Bind covers all scopes
    public InputManager Input { get { return Live(ref _input, InputManager.Instance); } }
#pragma warning restore CS0618
    public PositionValidationController PositionValidation { get { return Live(ref _positionValidation, PositionValidationController.Instance); } }
    public EffectManager Effects { get { return Live(ref _effects, GlobalEffectsPacketServices.Effects); } }
#pragma warning disable CS0618 // Locator fallback until Bind covers all scopes
    public EventBus Bus { get { return Live(ref _bus, EventBus.Instance); } }
#pragma warning restore CS0618
    public IGameEventBus GameEvents => _gameEvents;
    public CameraController Camera { get { return Live(ref _camera, CameraPacketServices.Controller); } }
    public IMoveAllCharacters Moves { get { return Live(ref _moves, MoveAllCharactersPacketServices.Moves); } }
    public CombatFacingService Facing { get { return Live(ref _facing, CombatFacingService.Instance); } }
    public IAnimationManager Animations { get { return Live(ref _animations, AnimationPacketServices.Animations); } }
    public SkillExecutor SkillRunner { get { return Live(ref _skillRunner, SkillExecutor.Instance); } }

    public L2GameUI Ui { get { return Live(ref _ui, SystemUIPacketServices.Ui); } }
    public ChatWindow Chat { get { return Live(ref _chat, MessagePacketServices.Chat); } }
    public HtmlWindow Html { get { return Live(ref _html, GameWindowPacketServices.Html); } }
    public DealerWindow Dealer { get { return Live(ref _dealer, MerchantPacketServices.Dealer); } }
    public MultiSellWindow MultiSell { get { return Live(ref _multiSell, MultiSellPacketServices.MultiSell); } }
    public TradeWindow Trade { get { return Live(ref _trade, TradePacketServices.Trade); } }
    public TradeRequestWindow TradeRequest { get { return Live(ref _tradeRequest, TradePacketServices.TradeRequest); } }
    public PartyInvitationWindow PartyInvite { get { return Live(ref _partyInvite, PartyPacketServices.PartyInvite); } }
    public EnchantWindow Enchant { get { return Live(ref _enchant, ItemEnchantPacketServices.Enchant); } }
    public RecipeBookWindow RecipeBook { get { return Live(ref _recipeBook, CraftPacketServices.RecipeBook); } }
    public CraftingItemWindow Crafting { get { return Live(ref _crafting, CraftPacketServices.Crafting); } }
    public SkillLearnWindow SkillLearn { get { return Live(ref _skillLearn, SkillLearnPacketServices.SkillLearn); } }
    public DescriptionSkillWindow SkillDesc { get { return Live(ref _skillDesc, SkillLearnPacketServices.SkillDesc); } }
    public SkillListWindow SkillList { get { return Live(ref _skillList, SkillLearnPacketServices.SkillList); } }
    public QuestWindow Quest { get { return Live(ref _quest, QuestPacketServices.Quest); } }
    public ClanWindow Clan { get { return Live(ref _clan, ClanPacketServices.Clan); } }
    public ShowListWindow ShowList { get { return Live(ref _showList, GameWindowPacketServices.ShowList); } }
    public SeedInfoWindow SeedInfo { get { return Live(ref _seedInfo, AuxiliaryGameUIPacketServices.SeedInfo); } }
    public SellCropListWindow SellCrop { get { return Live(ref _sellCrop, AuxiliaryGameUIPacketServices.SellCrop); } }
    public SkillbarWindow Skillbar { get { return Live(ref _skillbar, SkillbarPacketServices.Skillbar); } }
    public SystemMessageWindow SystemMessageUi { get { return Live(ref _systemMessageUi, SystemMessagePacketServices.Window); } }
    public BufferPanel Buffer { get { return Live(ref _buffer, AuxiliaryGameUIPacketServices.Buffer); } }
    public InventoryWindow InventoryUi { get { return Live(ref _inventoryUi, InventoryPacketServices.Window); } }
    public DeadWindow Dead { get { return Live(ref _dead, GameWindowPacketServices.Dead); } }
    public StatusWindow Status { get { return Live(ref _status, GameWindowPacketServices.Status); } }
    public WorldPacketApply WorldApply { get; private set; }
    public MessagePacketApply MessageApply { get; private set; }

    public PlayerController Player => _player;

    public PlayerActions Actions
    {
        get { return _actions != null ? _actions : (_actions = PlayerActions.Instance); }
    }

    [Inject]
    public void Construct(
        EventProcessor events,
        GameClient game,
        LoginClient login,
        GameManager manager,
        IGameEventBus gameEvents,
        World world,
        PlayerController player)
    {
        if (events != null)
            _events = events;
        if (game != null)
            _game = game;
        if (login != null)
            _login = login;
        if (manager != null)
            _manager = manager;
        if (gameEvents != null)
            _gameEvents = gameEvents;
        if (world != null)
            _world = world;
        if (player != null)
            _player = player;
        DiRuntimePacketServices.Bind(game: _game, login: _login, manager: _manager);
    }

    public void BindApply(WorldPacketApply worldApply, MessagePacketApply messageApply)
    {
        if (worldApply != null)
            WorldApply = worldApply;
        if (messageApply != null)
            MessageApply = messageApply;
    }

    private static T Live<T>(ref T field, T instance) where T : class
    {
        if (field == null)
            field = instance;
        return field;
    }
}

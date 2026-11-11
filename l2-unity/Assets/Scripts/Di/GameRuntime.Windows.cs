
using VContainer;

public sealed partial class GameRuntime
{
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
}

/// <summary>
/// Zone opcodes match Fineage-Backend <c>Fineage.Protocol.MsgType</c>
/// (and legacy tools/zone-server where ids overlap).
/// Login opcodes match Fineage-Backend Fineage.Protocol.MsgType.
/// </summary>
public enum ZoneV2MsgType : ushort
{
    Hello = 1,
    Auth = 2,
    EnterWorld = 3,
    MoveTo = 4,
    SelectTarget = 5,
    AttackRequest = 6,
    /// <summary>Zone chat. Body is text (key 0) and chat type (key 1). Fan-out is <see cref="CreatureSay"/>.</summary>
    Say = 8,
    GrantItemRequest = 13,
    /// <summary>Pick up a visible ground item. Backend <c>PickupRequestMsg</c> key 0 objectId.</summary>
    PickupRequest = 14,
    /// <summary>Equip a bag item, or unequip it when it is already worn. Backend <c>UseItemMsg</c> key 0 objectId.</summary>
    UseItem = 15,
    /// <summary>Clear one paperdoll index (0..16). Backend <c>UnequipItemMsg</c> key 0 paperdollSlot.</summary>
    UnequipItem = 16,
    /// <summary>Admin bag grant. Backend <c>GiveItemMsg</c> keys 0 itemId, 1 count. The zone rejects it unless admin commands are on.</summary>
    GiveItem = 17,
    /// <summary>Party invite. Backend <c>PartyInviteRequestMsg</c> keys 0 targetObjectId, 1 itemDistribution.</summary>
    PartyInviteRequest = 18,
    /// <summary>Accept or decline pending invite. Backend <c>PartyAnswerJoinRequestMsg</c>.</summary>
    PartyAnswerJoinRequest = 19,

    Register = 20,
    CreateCharacter = 21,
    ListCharacters = 22,
    SelectCharacter = 23,
    /// <summary>Lobby delete. Do not reuse zone CharInfo/NpcInfo/DeleteObject (1005–1007) or V2 lobby 1030–1033.</summary>
    DeleteCharacter = 24,
    RestoreCharacter = 25,
    /// <summary>Leave current party. Backend <c>PartyLeaveRequestMsg</c> (empty).</summary>
    PartyLeaveRequest = 26,
    /// <summary>Leader kick. Backend <c>PartyKickRequestMsg</c> key 0 targetObjectId.</summary>
    PartyKickRequest = 27,
    /// <summary>Request trade with a visible player. Backend <c>TradeRequestMsg</c> key 0 targetObjectId.</summary>
    TradeRequest = 28,
    /// <summary>Accept or decline a pending trade ask. Backend <c>TradeAnswerRequestMsg</c>.</summary>
    TradeAnswerRequest = 29,
    /// <summary>Create a clan. Backend <c>ClanCreateRequestMsg</c> key 0 name.</summary>
    ClanCreateRequest = 32,
    /// <summary>Invite a visible player. Backend <c>ClanInviteRequestMsg</c> keys 0 targetObjectId, 1 pledgeType.</summary>
    ClanInviteRequest = 33,
    /// <summary>Accept or decline a pending clan invite. Backend <c>ClanAnswerJoinRequestMsg</c>.</summary>
    ClanAnswerJoinRequest = 34,
    /// <summary>Leave the current clan. Empty body. The leader cannot leave.</summary>
    ClanLeaveRequest = 35,
    /// <summary>Leader dismisses a member. Backend <c>ClanKickRequestMsg</c> key 0 targetObjectId.</summary>
    ClanKickRequest = 36,
    /// <summary>Leader registers dissolution. Empty body.</summary>
    ClanDissolveRequest = 37,
    /// <summary>Leader cancels a pending dissolution. Empty body.</summary>
    ClanCancelDissolveRequest = 38,
    /// <summary>Select an enchant scroll. Backend <c>EnchantRequestMsg</c> key 0 scroll object id.</summary>
    EnchantRequest = 39,
    /// <summary>Enchant the chosen weapon or armor. Backend <c>EnchantConfirmMsg</c> key 0 target object id.</summary>
    EnchantConfirm = 40,
    /// <summary>Close the enchant window. Backend <c>EnchantCancelMsg</c> is an empty array.</summary>
    EnchantCancel = 41,
    /// <summary>Ask for the journal. Backend <c>RequestQuestListMsg</c> is an empty array.</summary>
    RequestQuestList = 42,
    /// <summary>Abort a started quest. Backend <c>RequestQuestAbortMsg</c> key 0 questId.</summary>
    RequestQuestAbort = 43,
    /// <summary>Talk to a non-attackable NPC. Backend <c>NpcTalkMsg</c> key 0 npcObjectId.</summary>
    NpcTalk = 44,
    /// <summary>Quest talk or adv-event. Backend <c>QuestEventMsg</c>.</summary>
    QuestEvent = 45,
    /// <summary>Use a known skill. Backend <c>SkillUseRequestMsg</c> keys 0 skillId, 1 targetObjectId, 2 ctrl.</summary>
    SkillUseRequest = 50,
    /// <summary>Respawn after death. Backend <c>ReviveRequestMsg</c> key 0 action (<see cref="ZoneV2Death.ReviveToVillage"/>).</summary>
    ReviveRequest = 52,
    /// <summary>Admin kill (empty body). Backend <c>AdminKillPlayerMsg</c>.</summary>
    AdminKillPlayer = 53,
    /// <summary>Add an inventory row to the active trade offer. Backend <c>TradeAddItemRequestMsg</c>.</summary>
    TradeAddItemRequest = 54,
    /// <summary>Confirm or cancel the trade window. Backend <c>TradeDoneRequestMsg</c> key 0 response.</summary>
    TradeDoneRequest = 55,
    /// <summary>NPC HTML link. Backend <c>BypassRequestMsg</c> keys 0 npcObjectId, 1 command.</summary>
    BypassRequest = 95,
    /// <summary>Buy from the open buylist. Backend <c>RequestBuyItemMsg</c>.</summary>
    RequestBuyItem = 96,
    /// <summary>Sell to the open merchant. Backend <c>RequestSellItemMsg</c>.</summary>
    RequestSellItem = 97,
    /// <summary>Choose a multisell row. Backend <c>RequestMultiSellChooseMsg</c>.</summary>
    RequestMultiSellChoose = 98,
    /// <summary>Wear preview. Backend replies with system message 189. <c>RequestPreviewItemMsg</c>.</summary>
    RequestPreviewItem = 99,
    /// <summary>Pick a gatekeeper destination. Backend <c>TeleportSelectRequestMsg</c>.</summary>
    TeleportSelectRequest = 105,
    /// <summary>Deposit into the private warehouse. Backend <c>WarehouseDepositRequestMsg</c>.</summary>
    WarehouseDepositRequest = 106,
    /// <summary>Withdraw from the private warehouse. Backend <c>WarehouseWithdrawRequestMsg</c>.</summary>
    WarehouseWithdrawRequest = 107,
    /// <summary>Newbie Helper supplemental magic. Backend <c>SupportMagicRequestMsg</c>.</summary>
    SupportMagicRequest = 108,
    /// <summary>Blessing of Protection. Backend <c>GiveBlessingRequestMsg</c>.</summary>
    GiveBlessingRequest = 109,

    HelloAck = 1001,
    AuthOk = 1002,
    PlayerSpawned = 1003,
    MoveToAck = 1004,
    CharInfo = 1005,
    NpcInfo = 1006,
    DeleteObject = 1007,
    MyTargetSelected = 1008,
    Attack = 1009,
    StatusUpdate = 1010,
    /// <summary>Zone chat fan-out. Keys 0 objectId, 1 name, 2 text, 3 chatType.</summary>
    CreatureSay = 1012,
    Pong = 1013,
    ItemList = 1014,
    InventoryUpdate = 1015,
    UserInfo = 1016,
    AbnormalStatusUpdate = 1017,
    /// <summary>Ground item in AOI. Keys match Backend <c>SpawnItemMsg</c> / Interlude <c>SpawnItemDto</c>.</summary>
    SpawnItem = 1018,

    RegisterOk = 1020,
    CharacterCreated = 1021,
    CharacterList = 1022,
    Handoff = 1023,
    LoginOk = 1024,
    /// <summary>Always followed by a fresh <see cref="CharacterList"/>.</summary>
    CharacterDeleted = 1025,
    /// <summary>Delete rejected. The roster is not resent.</summary>
    CharacterDeleteFailed = 1026,
    /// <summary>Party invite prompt. Backend <c>PartyAskJoinMsg</c>.</summary>
    PartyAskJoin = 1027,
    /// <summary>Invite result to inviter. Backend <c>PartyJoinResultMsg</c>.</summary>
    PartyJoinResult = 1028,
    /// <summary>Full party roster snapshot. Backend <c>PartyMemberListMsg</c>.</summary>
    PartyMemberList = 1029,
    /// <summary>System line. Keys 0 id, 1 params, 2 text. Chat band 1040–1049; 1041–1049 are reserved.</summary>
    SystemMessage = 1040,
    /// <summary>Pending clan invite. Backend <c>ClanAskJoinMsg</c>. Clan band 1050–1059; 1054–1059 are reserved.</summary>
    ClanAskJoin = 1050,
    /// <summary>Clan snapshot. Backend <c>PledgeStatusChangedMsg</c>. Clan id 0 clears the pledge.</summary>
    PledgeStatusChanged = 1051,
    /// <summary>Full clan member list. Backend <c>PledgeMemberListMsg</c>.</summary>
    PledgeMemberList = 1052,
    /// <summary>Invite outcome for the requestor. Backend <c>ClanJoinResultMsg</c>. 0 declined.</summary>
    ClanJoinResult = 1053,
    /// <summary>
    /// Enchant roll or cancel. Keys 0 result, 1 level after the attempt, 2 target object id, 3 crystal item id, 4 crystal count.
    /// Result 0 cancel, 1 success, 2 keep, 3 reset, 4 crystallize.
    /// </summary>
    EnchantResult = 1060,
    /// <summary>Started quests and the one-time mask. Backend <c>QuestListMsg</c>.</summary>
    QuestList = 1061,
    /// <summary>Quest HTML step. Backend <c>QuestDialogMsg</c>.</summary>
    QuestDialog = 1062,
    /// <summary>Quest stinger. Backend <c>QuestSoundMsg</c> key 0 name.</summary>
    QuestSound = 1063,
    /// <summary>Known skills after <see cref="ItemList"/> on enter. Backend <c>SkillListMsg</c>.</summary>
    SkillList = 1065,
    /// <summary>Skill cast fan-out. Backend <c>MagicSkillUseMsg</c>.</summary>
    MagicSkillUse = 1066,
    /// <summary>Creature died. Backend <c>DieMsg</c>. See <c>DeathProtocol</c>.</summary>
    Die = 1070,
    /// <summary>Creature revived. Backend <c>ReviveMsg</c>.</summary>
    Revive = 1071,
    /// <summary>Instant relocation (respawn, …). Backend <c>TeleportToLocationMsg</c>.</summary>
    TeleportToLocation = 1072,
    /// <summary>Incoming trade ask. Backend <c>TradeAskMsg</c>.</summary>
    TradeAsk = 1073,
    /// <summary>Trade ask outcome for the requestor. Backend <c>TradeRequestResultMsg</c>.</summary>
    TradeRequestResult = 1074,
    /// <summary>Trade session opened. Backend <c>TradeStartMsg</c>.</summary>
    TradeStart = 1075,
    /// <summary>Offer column update. Backend <c>TradeOfferUpdateMsg</c>.</summary>
    TradeOfferUpdate = 1076,
    /// <summary>Dual confirm flags. Backend <c>TradeConfirmStateMsg</c>.</summary>
    TradeConfirmState = 1077,
    /// <summary>Trade session ended. Backend <c>TradeFinishedMsg</c>.</summary>
    TradeFinished = 1078,
    /// <summary>NPC page. Backend <c>NpcHtmlMsg</c>. Quest steps stay on <see cref="QuestDialog"/>.</summary>
    NpcHtml = 1094,
    /// <summary>Merchant buylist. Backend <c>BuyListMsg</c>.</summary>
    BuyList = 1095,
    /// <summary>Items the merchant will buy. Backend <c>SellListMsg</c>.</summary>
    SellList = 1096,
    /// <summary>Exchange list. Backend <c>MultiSellListMsg</c>.</summary>
    MultiSellList = 1097,
    /// <summary>Adena, weight, and slots for the open shop. Backend <c>ShopBalanceMsg</c>.</summary>
    ShopBalance = 1098,
    /// <summary>Gatekeeper destinations. Backend <c>TeleportListMsg</c>.</summary>
    TeleportList = 1105,
    /// <summary>Private warehouse deposit rows. Backend <c>WarehouseDepositListMsg</c>.</summary>
    WarehouseDepositList = 1106,
    /// <summary>Private warehouse withdraw rows. Backend <c>WarehouseWithdrawListMsg</c>.</summary>
    WarehouseWithdrawList = 1107,
    /// <summary>Supplemental magic outcome. Backend <c>NewbieBuffResultMsg</c>.</summary>
    NewbieBuffResult = 1108,
    /// <summary>Blessing of Protection outcome. Backend <c>BlessingResultMsg</c>.</summary>
    BlessingResult = 1109,

    Error = 1099
}

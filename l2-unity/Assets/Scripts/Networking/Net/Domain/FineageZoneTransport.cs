using System;
using Fineage.Net.Contract;
using UnityEngine;

/// <summary>
/// Routes <see cref="IZoneTransport"/> calls to <see cref="NetSession"/>.
/// Keeps protocol mode checks out of GameManager, GameClient, and LoginWindow.
/// The Zone V2 wire path is no longer reachable from here.
/// </summary>
public sealed class FineageZoneTransport : IZoneTransport
{
    public event Action OnDisconnected;

    bool _disconnectNotified;

    /// <summary>
    /// Prefer DI-bound <see cref="NetSession"/> via <see cref="NetworkingPacketServices"/>;
    /// <see cref="NetSession.Ensure"/> only for early login before the binder runs.
    /// Static so party/trade user-command helpers can share the same locator.
    /// </summary>
    static NetSession Session => NetworkingPacketServices.Session ?? NetSession.Ensure();

    /// <summary>
    /// The local player profile. NetSession writes this when the character is selected and
    /// when the server sends UserInfo; the world scene reads it to spawn the player.
    /// </summary>
    public PlayerInfoInterlude PlayerInfo { get; set; } = new PlayerInfoInterlude();

    public bool Connected
    {
        get { return NetworkingPacketServices.Session != null; }
    }

    public int SessionObjectId
    {
        get
        {
            NetSession session = NetworkingPacketServices.Session;
            return session != null ? session.SessionObjectId : 0;
        }
    }

    public bool TrySendGameCommand(INetworkCommand command)
    {
        MoveToCommand netMove = command as MoveToCommand;
        if (netMove != null)
        {
            SendMove(netMove.From, netMove.To);
            return true;
        }

        ClickActionCommand netAction = command as ClickActionCommand;
        if (netAction != null)
        {
            World world = WorldApplyIncomingPacketServices.GameWorld;
            if (world != null)
            {
                Entity clicked = world.GetEntityNoLockSync(netAction.ObjectId);
                if (clicked is ItemEntity)
                {
                    Session.SendPickupRequest(netAction.ObjectId);
                    return true;
                }

                if (clicked is NpcEntity &&
                    !NetNpcSession.IsAttackable(netAction.ObjectId, clicked is MonsterEntity))
                {
                    Session.SendSelectTarget(netAction.ObjectId);
                    if (NetNpcWindows.InRange(clicked))
                        Session.SendNpcTalk(netAction.ObjectId);
                    else
                        NetChatFeed.NoteSystem("You are too far away.");
                    return true;
                }
            }

            Session.SendSelectTarget(netAction.ObjectId);
            TargetManager targets = ControllerPacketServices.Targets;
            if (targets != null && targets.IsAttackTargetSet())
                Session.SendAttackRequest(netAction.ObjectId);
            return true;
        }

        ZonePickupRequestCommand netPickup = command as ZonePickupRequestCommand;
        if (netPickup != null)
        {
            Session.SendPickupRequest(netPickup.GroundObjectId);
            return true;
        }

        RequestDropItemCommand netDrop = command as RequestDropItemCommand;
        if (netDrop != null)
        {
            Session.SendDropItemRequest(
                netDrop.ObjectId, netDrop.Count, netDrop.X, netDrop.Y, netDrop.Z);
            return true;
        }

        // Mobius RequestDestroyItem: the trash basket. It shares nothing with a drop, so it needs its own opcode.
        RequestDestroyItemCommand netDestroy = command as RequestDestroyItemCommand;
        if (netDestroy != null)
        {
            Session.SendDestroyItemRequest(netDestroy.ObjectId, netDestroy.Count);
            return true;
        }

        // Mobius enchant window: the scroll click and the roll travel as their own opcodes.
        RequestEnchantItemCommand netEnchant = command as RequestEnchantItemCommand;
        if (netEnchant != null)
        {
            if (netEnchant.ObjectId > 0)
                Session.SendEnchantRequest(netEnchant.ObjectId);
            else
                Session.SendEnchantCancel();
            return true;
        }

        UseItemCommand netUse = command as UseItemCommand;
        if (netUse != null)
        {
            Session.SendUseItem(netUse.ObjectId);
            return true;
        }

        // Mobius RequestAutoSoulShot: the hotbar right-click toggle for soulshots / spiritshots.
        RequestAutoSoulShotCommand netAutoShot = command as RequestAutoSoulShotCommand;
        if (netAutoShot != null)
        {
            if (netAutoShot.ItemId > 0)
                Session.SendRequestAutoSoulShot(netAutoShot.ItemId, netAutoShot.Enabled);
            return true;
        }

        UnequipItemCommand netUnequip = command as UnequipItemCommand;
        if (netUnequip != null)
        {
            Session.SendUnequipItem(netUnequip.PaperdollSlot);
            return true;
        }

        RequestShortCutRegCommand netShortCutReg = command as RequestShortCutRegCommand;
        if (netShortCutReg != null)
        {
            Session.SendShortCutReg(
                netShortCutReg.TypeId, netShortCutReg.WorldSlot, netShortCutReg.Id, netShortCutReg.Level);
            return true;
        }

        RequestShortCutDelCommand netShortCutDel = command as RequestShortCutDelCommand;
        if (netShortCutDel != null)
        {
            Session.SendShortCutDel(netShortCutDel.WorldSlot);
            return true;
        }

        ValidatePositionCommand netValidate = command as ValidatePositionCommand;
        if (netValidate != null)
        {
            Vector3 l2 = VectorUtils.ConvertPosUnityToL2j(new Vector3(netValidate.X, netValidate.Y, netValidate.Z));
            Session.SendValidatePosition(
                Mathf.RoundToInt(l2.x),
                Mathf.RoundToInt(l2.y),
                Mathf.RoundToInt(l2.z),
                netValidate.Heading);
            return true;
        }

        RequestSay2Command netSay = command as RequestSay2Command;
        if (netSay != null)
        {
            int chatType = netSay.Data != null ? netSay.Data.Type : NetChat.All;
            Session.SendSay(netSay.Message, chatType);
            return true;
        }

        RequestMagicSkillUseCommand netSkill = command as RequestMagicSkillUseCommand;
        if (netSkill != null)
        {
            // Mobius Player.useMagic: the caster stops the moment the cast is ordered, before
            // the zone answers with MagicSkillUse. Waiting for the reply let the client walk a
            // few metres past the cast origin, then PositionCorrection snapped it back.
            LocalCastStop.Halt();
            int targetId = ResolveSelectedTargetObjectId();
            bool ctrl = netSkill.CtrlPressed != 0;
            Session.SendUseSkill(netSkill.SkillId, targetId, ctrl);
            return true;
        }

        if (command is RequestRestartPointCommand)
        {
            Session.SendReviveRequest(ReviveRequestNetMsg.ReviveToVillage);
            return true;
        }

        // Mobius trainer window: RequestAcquireSkillInfo / RequestAcquireSkill go straight to the zone.
        RequestAcquireSkillInfoCommand netSkillInfo = command as RequestAcquireSkillInfoCommand;
        if (netSkillInfo != null)
        {
            Session.SendRequestAcquireSkillInfo(
                netSkillInfo.SkillId, netSkillInfo.SkillLevel, netSkillInfo.SkillType);
            return true;
        }

        RequestAcquireSkillCommand netSkillLearn = command as RequestAcquireSkillCommand;
        if (netSkillLearn != null)
        {
            Session.SendRequestAcquireSkill(
                netSkillLearn.SkillId, netSkillLearn.SkillLevel, netSkillLearn.SkillType);
            return true;
        }

        RequestBypassToServerCommand netBypass = command as RequestBypassToServerCommand;
        if (netBypass != null)
        {
            string bypass = netBypass.Bypass ?? "";
            // Mobius admin_* (//gmshop, //give, HTML admin_buy): no NPC — zone gates by KuK + FINEAGE_ZONE_ADMIN.
            if (IsAdminBypass(bypass))
            {
                // //give → admin_give; zone also accepts admin_create_item.
                if (NetGiveCommand.TryParse(bypass, out int giveItemId, out int giveCount))
                    bypass = "admin_create_item " + giveItemId + " " + giveCount;
                Session.SendNpcLink(0, bypass);
                return true;
            }

            int npcId = NetNpcSession.ActiveNpcObjectId;
            if (npcId > 0)
                Session.SendNpcLink(npcId, bypass);
            return true;
        }

        RequestBuyItemCommand netBuy = command as RequestBuyItemCommand;
        if (netBuy != null)
        {
            // Admin GM lists open with NpcObjectId=0; prefer that over a stale merchant id.
            int shopNpc = NetNpcSession.ShopNpcObjectId;
            Session.SendRequestBuyItem(
                shopNpc,
                netBuy.ListId > 0 ? netBuy.ListId : NetNpcSession.ShopListId,
                NetNpcOrders.ItemCounts(netBuy.BuyList));
            return true;
        }

        RequestSellItemCommand netSell = command as RequestSellItemCommand;
        if (netSell != null)
        {
            Session.SendRequestSellItem(
                NetNpcSession.ShopNpcObjectId,
                NetNpcOrders.ObjectCounts(netSell.SellList));
            return true;
        }

        MultiSellChooseCommand netMulti = command as MultiSellChooseCommand;
        if (netMulti != null)
        {
            Session.SendRequestMultiSellChoose(
                NetNpcSession.ShopNpcObjectId,
                netMulti.ListId,
                netMulti.EntryId,
                netMulti.Amount);
            return true;
        }

        SendWarehouseDepositListCommand netDeposit = command as SendWarehouseDepositListCommand;
        if (netDeposit != null)
        {
            Session.SendWarehouseDeposit(
                NetNpcSession.ShopNpcObjectId,
                NetNpcOrders.ObjectCounts(netDeposit.SellList));
            return true;
        }

        SendWarehouseWithdrawListCommand netWithdraw = command as SendWarehouseWithdrawListCommand;
        if (netWithdraw != null)
        {
            Session.SendWarehouseWithdraw(
                NetNpcSession.ShopNpcObjectId,
                NetNpcOrders.ObjectCounts(netWithdraw.SellList));
            return true;
        }

        if (command is RequestQuestListCommand)
        {
            Session.SendRequestQuestList();
            return true;
        }

        RequestQuestAbortCommand netAbortQuest = command as RequestQuestAbortCommand;
        if (netAbortQuest != null)
        {
            Session.SendRequestQuestAbort(netAbortQuest.QuestId);
            return true;
        }

        RequestJoinPartyCommand netJoinParty = command as RequestJoinPartyCommand;
        if (netJoinParty != null)
        {
            Session.SendPartyInviteRequest(netJoinParty.TargetObjectId, netJoinParty.ItemDistribution);
            return true;
        }

        RequestAnswerJoinPartyCommand netAnswerParty = command as RequestAnswerJoinPartyCommand;
        if (netAnswerParty != null)
        {
            if (netAnswerParty.RequestorObjectId > 0)
                Session.SendPartyAnswerJoinRequest(netAnswerParty.RequestorObjectId, netAnswerParty.Response);
            return true;
        }

        if (command is RequestWithdrawalPartyCommand)
        {
            Session.SendPartyLeaveRequest();
            return true;
        }

        RequestOustPartyMemberCommand netOustParty = command as RequestOustPartyMemberCommand;
        if (netOustParty != null)
        {
            Session.SendPartyKickRequest(netOustParty.TargetObjectId);
            return true;
        }

        // Mobius clan window: the seven pledge opcodes travel as their own request opcodes.
        ClanCreateRequestCommand netClanCreate = command as ClanCreateRequestCommand;
        if (netClanCreate != null)
        {
            Session.SendClanCreateRequest(netClanCreate.Name);
            return true;
        }

        // RequestJoinPledge and ClanInviteRequestCommand are the same Mobius opcode: the clan
        // owner invites the target, and a clanless player asks the owner to take them in.
        ClanInviteRequestCommand netClanInvite = command as ClanInviteRequestCommand;
        if (netClanInvite != null)
        {
            Session.SendClanInviteRequest(netClanInvite.TargetObjectId);
            return true;
        }

        RequestJoinPledgeCommand netClanJoin = command as RequestJoinPledgeCommand;
        if (netClanJoin != null)
        {
            Session.SendClanInviteRequest(netClanJoin.ObjectId);
            return true;
        }

        ClanAnswerJoinRequestCommand netClanAnswer = command as ClanAnswerJoinRequestCommand;
        if (netClanAnswer != null)
        {
            if (netClanAnswer.RequestorObjectId > 0)
                Session.SendClanAnswerJoinRequest(netClanAnswer.RequestorObjectId, netClanAnswer.Response);
            return true;
        }

        RequestWithdrawPledgeCommand netClanLeave = command as RequestWithdrawPledgeCommand;
        if (netClanLeave != null)
        {
            Session.SendClanLeaveRequest();
            return true;
        }

        ClanLeaveRequestCommand netClanLeaveExplicit = command as ClanLeaveRequestCommand;
        if (netClanLeaveExplicit != null)
        {
            Session.SendClanLeaveRequest();
            return true;
        }

        ClanKickRequestCommand netClanKick = command as ClanKickRequestCommand;
        if (netClanKick != null)
        {
            Session.SendClanKickRequest(netClanKick.TargetObjectId);
            return true;
        }

        // RequestOustPledgeMember is the Interlude name and carries a name; the Net opcode
        // takes the object id, so resolve it through the roster the zone just sent.
        RequestOustPledgeMemberCommand netClanOust = command as RequestOustPledgeMemberCommand;
        if (netClanOust != null)
        {
            NetPledgeMember member;
            if (NetClanBook.Shared.TryMemberByName(netClanOust.MemberName, out member))
                Session.SendClanKickRequest(member.ObjectId);
            return true;
        }

        // Mobius RequestTargetCanceld: an empty body. The client clears its own selection in
        // TargetManager; this is what tells the zone, so the next untargeted skill stops aiming there.
        if (command is RequestTargetCanceldCommand)
        {
            Session.SendTargetCancel();
            return true;
        }

        ClanDissolveRequestCommand netClanDissolve = command as ClanDissolveRequestCommand;
        if (netClanDissolve != null)
        {
            Session.SendClanDissolveRequest();
            return true;
        }

        ClanCancelDissolveRequestCommand netClanCancel = command as ClanCancelDissolveRequestCommand;
        if (netClanCancel != null)
        {
            Session.SendClanCancelDissolveRequest();
            return true;
        }

        RequestTradeCommand netRequestTrade = command as RequestTradeCommand;        if (netRequestTrade != null)
        {
            Session.SendTradeRequest(netRequestTrade.TargetObjectId);
            return true;
        }

        AnswerTradeRequestCommand netAnswerTrade = command as AnswerTradeRequestCommand;
        if (netAnswerTrade != null)
        {
            if (netAnswerTrade.RequestorObjectId > 0)
                Session.SendTradeAnswerRequest(netAnswerTrade.RequestorObjectId, netAnswerTrade.Response);
            return true;
        }

        AddTradeItemCommand netAddTrade = command as AddTradeItemCommand;
        if (netAddTrade != null)
        {
            Session.SendTradeAddItemRequest(netAddTrade.ObjectId, netAddTrade.Count);
            return true;
        }

        TradeDoneCommand netTradeDone = command as TradeDoneCommand;
        if (netTradeDone != null)
        {
            Session.SendTradeDoneRequest(netTradeDone.Response);
            return true;
        }

        RequestUserCommandCommand netUserCommand = command as RequestUserCommandCommand;
        if (netUserCommand != null)
        {
            if (TrySendNetPartyUserCommand(netUserCommand.Id))
                return true;
            if (TrySendNetTradeUserCommand(netUserCommand.Id))
                return true;
        }

        // No Net opcode exists for this command yet. Returning true still drops it, but say
        // so: returning true silently is how a clan invite or a recipe-book open vanishes
        // with no trace, and the caller has no way to tell it apart from a delivered command.
        FineageLog.Warn(FineageLogCategory.GameNet, "TX dropped (no Net opcode): " + command.GetType().Name);
        return true;
    }

    static bool TrySendNetPartyUserCommand(int commandId)
    {
        int targetId = ResolveSelectedTargetObjectId();
        switch (commandId)
        {
            case 16:
                if (targetId <= 0)
                    return true;
                Session.SendPartyInviteRequest(targetId, 0);
                return true;
            case 17:
                Session.SendPartyLeaveRequest();
                return true;
            case 18:
                if (targetId <= 0)
                    return true;
                Session.SendPartyKickRequest(targetId);
                return true;
            default:
                return false;
        }
    }

    static bool TrySendNetTradeUserCommand(int commandId)
    {
        if (commandId != (int)PlayerCommand.TRADE)
            return false;
        int targetId = ResolveSelectedTargetObjectId();
        if (targetId <= 0)
            return true;
        Session.SendTradeRequest(targetId);
        return true;
    }

    static int ResolveSelectedTargetObjectId()
    {
        TargetManager targets = ControllerPacketServices.Targets;
        if (targets == null)
            return 0;
        TargetData target = targets.Target ?? targets.AttackTarget;
        if (target?.Identity != null)
            return target.Identity.Id;
        return 0;
    }

    static bool IsAdminBypass(string bypass)
    {
        if (string.IsNullOrWhiteSpace(bypass))
            return false;
        string text = bypass.Trim();
        if (text.StartsWith("bypass -h ", System.StringComparison.OrdinalIgnoreCase))
            text = text.Substring("bypass -h ".Length).TrimStart();
        else if (text.StartsWith("bypass ", System.StringComparison.OrdinalIgnoreCase))
            text = text.Substring("bypass ".Length).TrimStart();
        if (text.StartsWith("npc_", System.StringComparison.OrdinalIgnoreCase))
        {
            int split = text.IndexOf('_', 4);
            if (split > 4)
                text = text.Substring(split + 1).TrimStart();
        }
        return text.StartsWith("admin_", System.StringComparison.OrdinalIgnoreCase);
    }

    public void BeginSession(string account, string password)
    {
        _disconnectNotified = false;
        Session.BeginLobby(account, password, registerIfMissing: true);
    }

    public void SendMove(Vector3 fromUnity, Vector3 toUnity)
    {
        Vector3 l2 = VectorUtils.ConvertPosUnityToL2j(toUnity);
        Session.SendMove(
            Mathf.RoundToInt(l2.x),
            Mathf.RoundToInt(l2.y),
            Mathf.RoundToInt(l2.z));
    }

    public void Disconnect()
    {
        NetSession session = NetworkingPacketServices.Session;
        if (session != null)
            UnityEngine.Object.Destroy(session.gameObject);
        NotifyDisconnected();
    }

    public void NotifyDisconnected()
    {
        if (_disconnectNotified)
            return;
        _disconnectNotified = true;
        OnDisconnected?.Invoke();
    }

    /// <summary>Allow a later zone connect to raise disconnect again.</summary>
    internal void ResetDisconnectLatch() => _disconnectNotified = false;
}

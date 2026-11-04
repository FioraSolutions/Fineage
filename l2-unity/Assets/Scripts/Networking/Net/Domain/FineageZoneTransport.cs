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

    /// <summary>
    /// The local player profile. NetSession writes this when the character is selected and
    /// when the server sends UserInfo; the world scene reads it to spawn the player.
    /// </summary>
    public PlayerInfoInterlude PlayerInfo { get; set; } = new PlayerInfoInterlude();

    public bool Connected
    {
        get { return NetSession.Instance != null; }
    }

    public int SessionObjectId
    {
        get
        {
            NetSession session = NetSession.Instance;
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
                    NetSession.Ensure().SendPickupRequest(netAction.ObjectId);
                    return true;
                }

                if (clicked is NpcEntity &&
                    !NetNpcSession.IsAttackable(netAction.ObjectId, clicked is MonsterEntity))
                {
                    NetSession.Ensure().SendSelectTarget(netAction.ObjectId);
                    if (NetNpcWindows.InRange(clicked))
                        NetSession.Ensure().SendNpcTalk(netAction.ObjectId);
                    else
                        NetChatFeed.NoteSystem("You are too far away.");
                    return true;
                }
            }

            NetSession.Ensure().SendSelectTarget(netAction.ObjectId);
            TargetManager targets = ControllerPacketServices.Targets;
            if (targets != null && targets.IsAttackTargetSet())
                NetSession.Ensure().SendAttackRequest(netAction.ObjectId);
            return true;
        }

        ZonePickupRequestCommand netPickup = command as ZonePickupRequestCommand;
        if (netPickup != null)
        {
            NetSession.Ensure().SendPickupRequest(netPickup.GroundObjectId);
            return true;
        }

        RequestDropItemCommand netDrop = command as RequestDropItemCommand;
        if (netDrop != null)
        {
            NetSession.Ensure().SendDropItemRequest(
                netDrop.ObjectId, netDrop.Count, netDrop.X, netDrop.Y, netDrop.Z);
            return true;
        }

        // Mobius RequestDestroyItem: the trash basket. It shares nothing with a drop, so it needs its own opcode.
        RequestDestroyItemCommand netDestroy = command as RequestDestroyItemCommand;
        if (netDestroy != null)
        {
            NetSession.Ensure().SendDestroyItemRequest(netDestroy.ObjectId, netDestroy.Count);
            return true;
        }

        // Mobius enchant window: the scroll click and the roll travel as their own opcodes.
        RequestEnchantItemCommand netEnchant = command as RequestEnchantItemCommand;
        if (netEnchant != null)
        {
            if (netEnchant.ObjectId > 0)
                NetSession.Ensure().SendEnchantRequest(netEnchant.ObjectId);
            else
                NetSession.Ensure().SendEnchantCancel();
            return true;
        }

        UseItemCommand netUse = command as UseItemCommand;
        if (netUse != null)
        {
            NetSession.Ensure().SendUseItem(netUse.ObjectId);
            return true;
        }

        UnequipItemCommand netUnequip = command as UnequipItemCommand;
        if (netUnequip != null)
        {
            NetSession.Ensure().SendUnequipItem(netUnequip.PaperdollSlot);
            return true;
        }

        RequestShortCutRegCommand netShortCutReg = command as RequestShortCutRegCommand;
        if (netShortCutReg != null)
        {
            NetSession.Ensure().SendShortCutReg(
                netShortCutReg.TypeId, netShortCutReg.WorldSlot, netShortCutReg.Id, netShortCutReg.Level);
            return true;
        }

        RequestShortCutDelCommand netShortCutDel = command as RequestShortCutDelCommand;
        if (netShortCutDel != null)
        {
            NetSession.Ensure().SendShortCutDel(netShortCutDel.WorldSlot);
            return true;
        }

        ValidatePositionCommand netValidate = command as ValidatePositionCommand;
        if (netValidate != null)
        {
            Vector3 l2 = VectorUtils.ConvertPosUnityToL2j(new Vector3(netValidate.X, netValidate.Y, netValidate.Z));
            NetSession.Ensure().SendValidatePosition(
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
            NetSession.Ensure().SendSay(netSay.Message, chatType);
            return true;
        }

        RequestMagicSkillUseCommand netSkill = command as RequestMagicSkillUseCommand;
        if (netSkill != null)
        {
            int targetId = ResolveSelectedTargetObjectId();
            bool ctrl = netSkill.CtrlPressed != 0;
            NetSession.Ensure().SendUseSkill(netSkill.SkillId, targetId, ctrl);
            return true;
        }

        if (command is RequestRestartPointCommand)
        {
            NetSession.Ensure().SendReviveRequest(ReviveRequestNetMsg.ReviveToVillage);
            return true;
        }

        // Mobius trainer window: RequestAcquireSkillInfo / RequestAcquireSkill go straight to the zone.
        RequestAcquireSkillInfoCommand netSkillInfo = command as RequestAcquireSkillInfoCommand;
        if (netSkillInfo != null)
        {
            NetSession.Ensure().SendRequestAcquireSkillInfo(
                netSkillInfo.SkillId, netSkillInfo.SkillLevel, netSkillInfo.SkillType);
            return true;
        }

        RequestAcquireSkillCommand netSkillLearn = command as RequestAcquireSkillCommand;
        if (netSkillLearn != null)
        {
            NetSession.Ensure().SendRequestAcquireSkill(
                netSkillLearn.SkillId, netSkillLearn.SkillLevel, netSkillLearn.SkillType);
            return true;
        }

        RequestBypassToServerCommand netBypass = command as RequestBypassToServerCommand;
        int netGiveItemId;
        int netGiveCount;
        if (netBypass != null && NetGiveCommand.TryParse(netBypass.Bypass, out netGiveItemId, out netGiveCount))
        {
            // Grant-item admin bypass: still V2-only until a Net grant opcode exists.
            return true;
        }

        if (netBypass != null)
        {
            int npcId = NetNpcSession.ActiveNpcObjectId;
            if (npcId > 0)
                NetSession.Ensure().SendNpcLink(npcId, netBypass.Bypass);
            return true;
        }

        RequestBuyItemCommand netBuy = command as RequestBuyItemCommand;
        if (netBuy != null)
        {
            NetSession.Ensure().SendRequestBuyItem(
                NetNpcSession.ShopNpcObjectId,
                netBuy.ListId,
                NetNpcOrders.ItemCounts(netBuy.BuyList));
            return true;
        }

        RequestSellItemCommand netSell = command as RequestSellItemCommand;
        if (netSell != null)
        {
            NetSession.Ensure().SendRequestSellItem(
                NetNpcSession.ShopNpcObjectId,
                NetNpcOrders.ObjectCounts(netSell.SellList));
            return true;
        }

        MultiSellChooseCommand netMulti = command as MultiSellChooseCommand;
        if (netMulti != null)
        {
            NetSession.Ensure().SendRequestMultiSellChoose(
                NetNpcSession.ShopNpcObjectId,
                netMulti.ListId,
                netMulti.EntryId,
                netMulti.Amount);
            return true;
        }

        SendWarehouseDepositListCommand netDeposit = command as SendWarehouseDepositListCommand;
        if (netDeposit != null)
        {
            NetSession.Ensure().SendWarehouseDeposit(
                NetNpcSession.ShopNpcObjectId,
                NetNpcOrders.ObjectCounts(netDeposit.SellList));
            return true;
        }

        SendWarehouseWithdrawListCommand netWithdraw = command as SendWarehouseWithdrawListCommand;
        if (netWithdraw != null)
        {
            NetSession.Ensure().SendWarehouseWithdraw(
                NetNpcSession.ShopNpcObjectId,
                NetNpcOrders.ObjectCounts(netWithdraw.SellList));
            return true;
        }

        if (command is RequestQuestListCommand)
        {
            NetSession.Ensure().SendRequestQuestList();
            return true;
        }

        RequestQuestAbortCommand netAbortQuest = command as RequestQuestAbortCommand;
        if (netAbortQuest != null)
        {
            NetSession.Ensure().SendRequestQuestAbort(netAbortQuest.QuestId);
            return true;
        }

        RequestJoinPartyCommand netJoinParty = command as RequestJoinPartyCommand;
        if (netJoinParty != null)
        {
            NetSession.Ensure().SendPartyInviteRequest(netJoinParty.TargetObjectId, netJoinParty.ItemDistribution);
            return true;
        }

        RequestAnswerJoinPartyCommand netAnswerParty = command as RequestAnswerJoinPartyCommand;
        if (netAnswerParty != null)
        {
            if (netAnswerParty.RequestorObjectId > 0)
                NetSession.Ensure().SendPartyAnswerJoinRequest(netAnswerParty.RequestorObjectId, netAnswerParty.Response);
            return true;
        }

        if (command is RequestWithdrawalPartyCommand)
        {
            NetSession.Ensure().SendPartyLeaveRequest();
            return true;
        }

        RequestOustPartyMemberCommand netOustParty = command as RequestOustPartyMemberCommand;
        if (netOustParty != null)
        {
            NetSession.Ensure().SendPartyKickRequest(netOustParty.TargetObjectId);
            return true;
        }

        // Mobius clan window: the seven pledge opcodes travel as their own request opcodes.
        ClanCreateRequestCommand netClanCreate = command as ClanCreateRequestCommand;
        if (netClanCreate != null)
        {
            NetSession.Ensure().SendClanCreateRequest(netClanCreate.Name);
            return true;
        }

        // RequestJoinPledge and ClanInviteRequestCommand are the same Mobius opcode: the clan
        // owner invites the target, and a clanless player asks the owner to take them in.
        ClanInviteRequestCommand netClanInvite = command as ClanInviteRequestCommand;
        if (netClanInvite != null)
        {
            NetSession.Ensure().SendClanInviteRequest(netClanInvite.TargetObjectId);
            return true;
        }

        RequestJoinPledgeCommand netClanJoin = command as RequestJoinPledgeCommand;
        if (netClanJoin != null)
        {
            NetSession.Ensure().SendClanInviteRequest(netClanJoin.ObjectId);
            return true;
        }

        ClanAnswerJoinRequestCommand netClanAnswer = command as ClanAnswerJoinRequestCommand;
        if (netClanAnswer != null)
        {
            if (netClanAnswer.RequestorObjectId > 0)
                NetSession.Ensure().SendClanAnswerJoinRequest(netClanAnswer.RequestorObjectId, netClanAnswer.Response);
            return true;
        }

        RequestWithdrawPledgeCommand netClanLeave = command as RequestWithdrawPledgeCommand;
        if (netClanLeave != null)
        {
            NetSession.Ensure().SendClanLeaveRequest();
            return true;
        }

        ClanLeaveRequestCommand netClanLeaveExplicit = command as ClanLeaveRequestCommand;
        if (netClanLeaveExplicit != null)
        {
            NetSession.Ensure().SendClanLeaveRequest();
            return true;
        }

        ClanKickRequestCommand netClanKick = command as ClanKickRequestCommand;
        if (netClanKick != null)
        {
            NetSession.Ensure().SendClanKickRequest(netClanKick.TargetObjectId);
            return true;
        }

        // RequestOustPledgeMember is the Interlude name and carries a name; the Net opcode
        // takes the object id, so resolve it through the roster the zone just sent.
        RequestOustPledgeMemberCommand netClanOust = command as RequestOustPledgeMemberCommand;
        if (netClanOust != null)
        {
            NetPledgeMember member;
            if (NetClanBook.Shared.TryMemberByName(netClanOust.MemberName, out member))
                NetSession.Ensure().SendClanKickRequest(member.ObjectId);
            return true;
        }

        // Mobius RequestTargetCanceld: an empty body. The client clears its own selection in
        // TargetManager; this is what tells the zone, so the next untargeted skill stops aiming there.
        if (command is RequestTargetCanceldCommand)
        {
            NetSession.Ensure().SendTargetCancel();
            return true;
        }

        ClanDissolveRequestCommand netClanDissolve = command as ClanDissolveRequestCommand;
        if (netClanDissolve != null)
        {
            NetSession.Ensure().SendClanDissolveRequest();
            return true;
        }

        ClanCancelDissolveRequestCommand netClanCancel = command as ClanCancelDissolveRequestCommand;
        if (netClanCancel != null)
        {
            NetSession.Ensure().SendClanCancelDissolveRequest();
            return true;
        }

        RequestTradeCommand netRequestTrade = command as RequestTradeCommand;        if (netRequestTrade != null)
        {
            NetSession.Ensure().SendTradeRequest(netRequestTrade.TargetObjectId);
            return true;
        }

        AnswerTradeRequestCommand netAnswerTrade = command as AnswerTradeRequestCommand;
        if (netAnswerTrade != null)
        {
            if (netAnswerTrade.RequestorObjectId > 0)
                NetSession.Ensure().SendTradeAnswerRequest(netAnswerTrade.RequestorObjectId, netAnswerTrade.Response);
            return true;
        }

        AddTradeItemCommand netAddTrade = command as AddTradeItemCommand;
        if (netAddTrade != null)
        {
            NetSession.Ensure().SendTradeAddItemRequest(netAddTrade.ObjectId, netAddTrade.Count);
            return true;
        }

        TradeDoneCommand netTradeDone = command as TradeDoneCommand;
        if (netTradeDone != null)
        {
            NetSession.Ensure().SendTradeDoneRequest(netTradeDone.Response);
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
        FineageLog.Warn(FineageLog.Category.GameNet, "TX dropped (no Net opcode): " + command.GetType().Name);
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
                NetSession.Ensure().SendPartyInviteRequest(targetId, 0);
                return true;
            case 17:
                NetSession.Ensure().SendPartyLeaveRequest();
                return true;
            case 18:
                if (targetId <= 0)
                    return true;
                NetSession.Ensure().SendPartyKickRequest(targetId);
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
        NetSession.Ensure().SendTradeRequest(targetId);
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

    public void BeginSession(string account, string password)
    {
        NetSession.Ensure().BeginLobby(account, password, registerIfMissing: true);
    }

    public void SendMove(Vector3 fromUnity, Vector3 toUnity)
    {
        Vector3 l2 = VectorUtils.ConvertPosUnityToL2j(toUnity);
        NetSession.Ensure().SendMove(
            Mathf.RoundToInt(l2.x),
            Mathf.RoundToInt(l2.y),
            Mathf.RoundToInt(l2.z));
    }

    public void Disconnect()
    {
        if (NetSession.Instance != null)
            UnityEngine.Object.Destroy(NetSession.Instance.gameObject);
    }
}

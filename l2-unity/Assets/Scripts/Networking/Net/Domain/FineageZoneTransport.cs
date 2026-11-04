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

    public bool BypassesInterludeLogin => FineageWireConfig.IsNet;

    public bool AllowsInterludeGameSocketAuth => !FineageWireConfig.IsNet;

    public bool Connected
    {
        get { return FineageWireConfig.IsNet && NetSession.Instance != null; }
    }

    public int SessionObjectId
    {
        get
        {
            if (!FineageWireConfig.IsNet)
                return 0;
            NetSession session = NetSession.Instance;
            return session != null ? session.SessionObjectId : 0;
        }
    }

    public bool TrySendGameCommand(INetworkCommand command)
    {
        if (FineageWireConfig.IsNet)
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

            RequestTradeCommand netRequestTrade = command as RequestTradeCommand;
            if (netRequestTrade != null)
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

            // Other Interlude commands stay no-ops until Net opcodes exist.
            return true;
        }

        return false;
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
        if (!FineageWireConfig.IsNet)
            return;

        NetSession.Ensure().BeginLobby(account, password, registerIfMissing: true);
    }

    public void SendMove(Vector3 fromUnity, Vector3 toUnity)
    {
        if (!FineageWireConfig.IsNet)
            return;

        Vector3 l2 = VectorUtils.ConvertPosUnityToL2j(toUnity);
        NetSession.Ensure().SendMove(
            Mathf.RoundToInt(l2.x),
            Mathf.RoundToInt(l2.y),
            Mathf.RoundToInt(l2.z));
    }

    public void Disconnect()
    {
        if (!FineageWireConfig.IsNet)
            return;

        if (NetSession.Instance != null)
            UnityEngine.Object.Destroy(NetSession.Instance.gameObject);
    }
}

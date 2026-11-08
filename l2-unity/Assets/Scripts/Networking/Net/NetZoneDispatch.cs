using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

/// <summary>
/// Zone inbound opcode dispatch extracted from <see cref="NetSession"/> (B4 / #131).
/// Apply bodies stay on the session; this type owns the switch only.
/// </summary>
public sealed class NetZoneDispatch
{
    readonly NetSession _session;
    readonly Func<Task> _enterWorldAsync;

    public NetZoneDispatch(NetSession session, Func<Task> enterWorldAsync)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
        _enterWorldAsync = enterWorldAsync ?? throw new ArgumentNullException(nameof(enterWorldAsync));
    }

    public void Handle(PacketEnvelope envelope)
    {

        switch (envelope.Opcode)
        {
            case NetOpcode.HelloAck:
                break;
            case NetOpcode.ZoneAuthResult:
            {
                var result = NetCodec.DecodeZoneAuthResult(envelope.Payload);
                if (!result.Ok)
                {
                    Debug.LogWarning($"[Net] zone auth failed {result.Code}: {result.Message}");
                    return;
                }

                _ = _enterWorldAsync();
                break;
            }
            case NetOpcode.EntitySpawn:
                _session.ApplyEntitySpawn(NetCodec.DecodeEntitySpawn(envelope.Payload));
                break;
            case NetOpcode.EntityDespawn:
                _session.ApplyEntityDespawn(NetCodec.DecodeEntityDespawn(envelope.Payload));
                break;
            case NetOpcode.PlayerStats:
                _session.ApplyPlayerStats(NetCodec.DecodePlayerStats(envelope.Payload));
                break;
            case NetOpcode.ItemList:
                _session.ApplyItemList(NetCodec.DecodeItemList(envelope.Payload));
                break;
            case NetOpcode.SkillList:
                _session.ApplySkillList(NetCodec.DecodeSkillList(envelope.Payload));
                break;
            case NetOpcode.UserInfo:
                _session.ApplyUserInfo(NetCodec.DecodeUserInfo(envelope.Payload));
                break;
            case NetOpcode.ShortCutInit:
                NetShortCutReceiver.ApplyInit(NetCodec.DecodeShortCutInit(envelope.Payload));
                break;
            case NetOpcode.ShortCutRegister:
                NetShortCutReceiver.ApplyRegister(NetCodec.DecodeShortCut(envelope.Payload));
                break;
            case NetOpcode.ShortCutDelete:
                NetShortCutReceiver.ApplyDelete(NetCodec.DecodeShortCutDel(envelope.Payload));
                break;
            case NetOpcode.EtcStatusUpdate:
                NetShortCutReceiver.ApplyEtcStatus(NetCodec.DecodeEtcStatusUpdate(envelope.Payload));
                break;
            case NetOpcode.MoveState:
                _session.ApplyMoveState(NetCodec.DecodeMoveState(envelope.Payload));
                break;
            case NetOpcode.ChangeMoveType:
                NetShortCutReceiver.ApplyChangeMoveType(NetCodec.DecodeChangeMoveType(envelope.Payload));
                break;
            case NetOpcode.ChangeWaitType:
                _session.ApplyChangeWaitType(NetCodec.DecodeChangeWaitType(envelope.Payload));
                break;
            case NetOpcode.SocialAction:
                _session.ApplySocialAction(NetCodec.DecodeSocialAction(envelope.Payload));
                break;
            case NetOpcode.TargetSelected:
                _session.ApplyTargetSelected(NetCodec.DecodeTargetSelected(envelope.Payload));
                break;
            case NetOpcode.TargetUnselected:
                _session.ApplyTargetUnselected(NetCodec.DecodeTargetUnselected(envelope.Payload));
                break;
            case NetOpcode.AttackState:
                _session.ApplyAttackState(NetCodec.DecodeAttackState(envelope.Payload));
                break;
            case NetOpcode.AutoAttackStart:
                _session.ApplyAutoAttackStart((int)NetCodec.DecodeAttackStance(envelope.Payload).EntityValue);
                break;
            case NetOpcode.AutoAttackStop:
                _session.ApplyAutoAttackStop((int)NetCodec.DecodeAttackStance(envelope.Payload).EntityValue);
                break;
            case NetOpcode.AutoSoulShot:
            {
                AutoSoulShotNetMsg shot = NetCodec.DecodeAutoSoulShot(envelope.Payload);
                NetAutoShot.Apply(shot.ItemId, shot.Enabled);
                // The hotbar shows the armed state on the shot slot itself.
                SkillbarWindow skillbar = SkillbarPacketServices.Skillbar;
                if (skillbar != null)
                    skillbar.RefreshAutoShotMarks();
                break;
            }
            case NetOpcode.StatusUpdate:
                _session.ApplyStatusUpdate(NetCodec.DecodeStatusUpdate(envelope.Payload));
                break;
            case NetOpcode.EntityDie:
                _session.ApplyEntityDie(NetCodec.DecodeEntityDie(envelope.Payload));
                break;
            case NetOpcode.DropItem:
                _session.ApplyDropItem(NetCodec.DecodeDropItem(envelope.Payload));
                break;
            case NetOpcode.InventoryUpdate:
                _session.ApplyInventoryUpdate(NetCodec.DecodeInventoryUpdate(envelope.Payload));
                break;
            case NetOpcode.CreatureSay:
                _session.ApplyCreatureSay(NetCodec.DecodeCreatureSay(envelope.Payload));
                break;
            case NetOpcode.SystemMessage:
                _session.ApplySystemMessage(NetCodec.DecodeSystemMessage(envelope.Payload));
                break;
            case NetOpcode.MagicSkillUse:
                _session.ApplyMagicSkillUse(NetCodec.DecodeMagicSkillUse(envelope.Payload));
                break;
            case NetOpcode.Revive:
                _session.ApplyRevive(NetCodec.DecodeRevive(envelope.Payload));
                break;
            case NetOpcode.TeleportToLocation:
                _session.ApplyTeleportToLocation(NetCodec.DecodeTeleportToLocation(envelope.Payload));
                break;
            case NetOpcode.AbnormalStatusUpdate:
                _session.ApplyAbnormalStatusUpdate(NetCodec.DecodeAbnormalStatusUpdate(envelope.Payload));
                break;
            case NetOpcode.NpcHtml:
                _session.ApplyNpcHtml(NetCodec.DecodeNpcHtml(envelope.Payload));
                break;
            case NetOpcode.BuyList:
                _session.ApplyBuyList(NetCodec.DecodeBuyList(envelope.Payload));
                break;
            case NetOpcode.SellList:
                _session.ApplySellList(NetCodec.DecodeSellList(envelope.Payload));
                break;
            case NetOpcode.MultiSellList:
                _session.ApplyMultiSellList(NetCodec.DecodeMultiSellList(envelope.Payload));
                break;
            case NetOpcode.ShopBalance:
                _session.ApplyShopBalance(NetCodec.DecodeShopBalance(envelope.Payload));
                break;
            case NetOpcode.WarehouseDepositList:
                _session.ApplyWarehouseDepositList(NetCodec.DecodeWarehouseDepositList(envelope.Payload));
                break;
            case NetOpcode.WarehouseWithdrawList:
                _session.ApplyWarehouseWithdrawList(NetCodec.DecodeWarehouseWithdrawList(envelope.Payload));
                break;
            case NetOpcode.QuestList:
                _session.ApplyQuestList(NetCodec.DecodeQuestList(envelope.Payload));
                break;
            case NetOpcode.AcquireSkillList:
                _session.ApplyAcquireSkillList(NetCodec.DecodeAcquireSkillList(envelope.Payload));
                break;
            case NetOpcode.AcquireSkillInfo:
                _session.ApplyAcquireSkillInfo(NetCodec.DecodeAcquireSkillInfo(envelope.Payload));
                break;
            case NetOpcode.EnchantResult:
                _session.ApplyEnchantResult(NetCodec.DecodeEnchantResult(envelope.Payload));
                break;
            case NetOpcode.PledgeStatus:
                _session.ApplyPledgeStatus(NetCodec.DecodePledgeStatus(envelope.Payload));
                break;
            case NetOpcode.PledgeMemberList:
                _session.ApplyPledgeMemberList(NetCodec.DecodePledgeMemberList(envelope.Payload));
                break;
            case NetOpcode.ClanAskJoin:
                _session.ApplyClanAskJoin(NetCodec.DecodeClanAskJoin(envelope.Payload));
                break;
            case NetOpcode.ClanJoinResult:
                _session.ApplyClanJoinResult(NetCodec.DecodeClanJoinResult(envelope.Payload));
                break;
            case NetOpcode.PartyAskJoin:
                _session.ApplyPartyAskJoin(NetCodec.DecodePartyAskJoin(envelope.Payload));
                break;
            case NetOpcode.PartyJoinResult:
                _session.ApplyPartyJoinResult(NetCodec.DecodePartyJoinResult(envelope.Payload));
                break;
            case NetOpcode.PartyMemberList:
                _session.ApplyPartyMemberList(NetCodec.DecodePartyMemberList(envelope.Payload));
                break;
            case NetOpcode.TradeAsk:
                _session.ApplyTradeAsk(NetCodec.DecodeTradeAsk(envelope.Payload));
                break;
            case NetOpcode.TradeRequestResult:
                _session.ApplyTradeRequestResult(NetCodec.DecodeTradeRequestResult(envelope.Payload));
                break;
            case NetOpcode.TradeStart:
                _session.ApplyTradeStart(NetCodec.DecodeTradeStart(envelope.Payload));
                break;
            case NetOpcode.TradeOfferUpdate:
                _session.ApplyTradeOfferUpdate(NetCodec.DecodeTradeOfferUpdate(envelope.Payload));
                break;
            case NetOpcode.TradeConfirmState:
                _session.ApplyTradeConfirmState(NetCodec.DecodeTradeConfirmState(envelope.Payload));
                break;
            case NetOpcode.TradeFinished:
                _session.ApplyTradeFinished(NetCodec.DecodeTradeFinished(envelope.Payload));
                break;
            case NetOpcode.PositionCorrection:
                _session.ApplyCorrection(NetCodec.DecodePositionCorrection(envelope.Payload));
                break;
            case NetOpcode.Reject:
            {
                var reject = NetCodec.DecodeReject(envelope.Payload);
                Debug.LogWarning($"[Net] zone reject {reject.Code}: {reject.Message}");
                NetSession.NoteReject(reject.Code, reject.Message);
                break;
            }
        }
    
    }
}

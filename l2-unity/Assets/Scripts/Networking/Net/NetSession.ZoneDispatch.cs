using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetSession
{
    void HandleZone(PacketEnvelope envelope)
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

                _ = EnterWorldAsync();
                break;
            }
            case NetOpcode.EntitySpawn:
                ApplyEntitySpawn(NetCodec.DecodeEntitySpawn(envelope.Payload));
                break;
            case NetOpcode.EntityDespawn:
                ApplyEntityDespawn(NetCodec.DecodeEntityDespawn(envelope.Payload));
                break;
            case NetOpcode.PlayerStats:
                ApplyPlayerStats(NetCodec.DecodePlayerStats(envelope.Payload));
                break;
            case NetOpcode.ItemList:
                ApplyItemList(NetCodec.DecodeItemList(envelope.Payload));
                break;
            case NetOpcode.SkillList:
                ApplySkillList(NetCodec.DecodeSkillList(envelope.Payload));
                break;
            case NetOpcode.UserInfo:
                ApplyUserInfo(NetCodec.DecodeUserInfo(envelope.Payload));
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
                ApplyMoveState(NetCodec.DecodeMoveState(envelope.Payload));
                break;
            case NetOpcode.ChangeMoveType:
                NetShortCutReceiver.ApplyChangeMoveType(NetCodec.DecodeChangeMoveType(envelope.Payload));
                break;
            case NetOpcode.ChangeWaitType:
                ApplyChangeWaitType(NetCodec.DecodeChangeWaitType(envelope.Payload));
                break;
            case NetOpcode.SocialAction:
                ApplySocialAction(NetCodec.DecodeSocialAction(envelope.Payload));
                break;
            case NetOpcode.TargetSelected:
                ApplyTargetSelected(NetCodec.DecodeTargetSelected(envelope.Payload));
                break;
            case NetOpcode.TargetUnselected:
                ApplyTargetUnselected(NetCodec.DecodeTargetUnselected(envelope.Payload));
                break;
            case NetOpcode.AttackState:
                ApplyAttackState(NetCodec.DecodeAttackState(envelope.Payload));
                break;
            case NetOpcode.AutoAttackStart:
                ApplyAutoAttackStart((int)NetCodec.DecodeAttackStance(envelope.Payload).EntityValue);
                break;
            case NetOpcode.AutoAttackStop:
                ApplyAutoAttackStop((int)NetCodec.DecodeAttackStance(envelope.Payload).EntityValue);
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
                ApplyStatusUpdate(NetCodec.DecodeStatusUpdate(envelope.Payload));
                break;
            case NetOpcode.EntityDie:
                ApplyEntityDie(NetCodec.DecodeEntityDie(envelope.Payload));
                break;
            case NetOpcode.DropItem:
                ApplyDropItem(NetCodec.DecodeDropItem(envelope.Payload));
                break;
            case NetOpcode.InventoryUpdate:
                ApplyInventoryUpdate(NetCodec.DecodeInventoryUpdate(envelope.Payload));
                break;
            case NetOpcode.CreatureSay:
                ApplyCreatureSay(NetCodec.DecodeCreatureSay(envelope.Payload));
                break;
            case NetOpcode.SystemMessage:
                ApplySystemMessage(NetCodec.DecodeSystemMessage(envelope.Payload));
                break;
            case NetOpcode.MagicSkillUse:
                ApplyMagicSkillUse(NetCodec.DecodeMagicSkillUse(envelope.Payload));
                break;
            case NetOpcode.Revive:
                ApplyRevive(NetCodec.DecodeRevive(envelope.Payload));
                break;
            case NetOpcode.TeleportToLocation:
                ApplyTeleportToLocation(NetCodec.DecodeTeleportToLocation(envelope.Payload));
                break;
            case NetOpcode.AbnormalStatusUpdate:
                ApplyAbnormalStatusUpdate(NetCodec.DecodeAbnormalStatusUpdate(envelope.Payload));
                break;
            case NetOpcode.NpcHtml:
                ApplyNpcHtml(NetCodec.DecodeNpcHtml(envelope.Payload));
                break;
            case NetOpcode.BuyList:
                ApplyBuyList(NetCodec.DecodeBuyList(envelope.Payload));
                break;
            case NetOpcode.SellList:
                ApplySellList(NetCodec.DecodeSellList(envelope.Payload));
                break;
            case NetOpcode.MultiSellList:
                ApplyMultiSellList(NetCodec.DecodeMultiSellList(envelope.Payload));
                break;
            case NetOpcode.ShopBalance:
                ApplyShopBalance(NetCodec.DecodeShopBalance(envelope.Payload));
                break;
            case NetOpcode.WarehouseDepositList:
                ApplyWarehouseDepositList(NetCodec.DecodeWarehouseDepositList(envelope.Payload));
                break;
            case NetOpcode.WarehouseWithdrawList:
                ApplyWarehouseWithdrawList(NetCodec.DecodeWarehouseWithdrawList(envelope.Payload));
                break;
            case NetOpcode.QuestList:
                ApplyQuestList(NetCodec.DecodeQuestList(envelope.Payload));
                break;
            case NetOpcode.AcquireSkillList:
                ApplyAcquireSkillList(NetCodec.DecodeAcquireSkillList(envelope.Payload));
                break;
            case NetOpcode.AcquireSkillInfo:
                ApplyAcquireSkillInfo(NetCodec.DecodeAcquireSkillInfo(envelope.Payload));
                break;
            case NetOpcode.EnchantResult:
                ApplyEnchantResult(NetCodec.DecodeEnchantResult(envelope.Payload));
                break;
            case NetOpcode.PledgeStatus:
                ApplyPledgeStatus(NetCodec.DecodePledgeStatus(envelope.Payload));
                break;
            case NetOpcode.PledgeMemberList:
                ApplyPledgeMemberList(NetCodec.DecodePledgeMemberList(envelope.Payload));
                break;
            case NetOpcode.ClanAskJoin:
                ApplyClanAskJoin(NetCodec.DecodeClanAskJoin(envelope.Payload));
                break;
            case NetOpcode.ClanJoinResult:
                ApplyClanJoinResult(NetCodec.DecodeClanJoinResult(envelope.Payload));
                break;
            case NetOpcode.PartyAskJoin:
                ApplyPartyAskJoin(NetCodec.DecodePartyAskJoin(envelope.Payload));
                break;
            case NetOpcode.PartyJoinResult:
                ApplyPartyJoinResult(NetCodec.DecodePartyJoinResult(envelope.Payload));
                break;
            case NetOpcode.PartyMemberList:
                ApplyPartyMemberList(NetCodec.DecodePartyMemberList(envelope.Payload));
                break;
            case NetOpcode.TradeAsk:
                ApplyTradeAsk(NetCodec.DecodeTradeAsk(envelope.Payload));
                break;
            case NetOpcode.TradeRequestResult:
                ApplyTradeRequestResult(NetCodec.DecodeTradeRequestResult(envelope.Payload));
                break;
            case NetOpcode.TradeStart:
                ApplyTradeStart(NetCodec.DecodeTradeStart(envelope.Payload));
                break;
            case NetOpcode.TradeOfferUpdate:
                ApplyTradeOfferUpdate(NetCodec.DecodeTradeOfferUpdate(envelope.Payload));
                break;
            case NetOpcode.TradeConfirmState:
                ApplyTradeConfirmState(NetCodec.DecodeTradeConfirmState(envelope.Payload));
                break;
            case NetOpcode.TradeFinished:
                ApplyTradeFinished(NetCodec.DecodeTradeFinished(envelope.Payload));
                break;
            case NetOpcode.PositionCorrection:
                ApplyCorrection(NetCodec.DecodePositionCorrection(envelope.Payload));
                break;
            case NetOpcode.Reject:
            {
                var reject = NetCodec.DecodeReject(envelope.Payload);
                Debug.LogWarning($"[Net] zone reject {reject.Code}: {reject.Message}");
                NoteReject(reject.Code, reject.Message);
                break;
            }
        }
    }
}

using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetZoneDispatch
{
    public void Handle(ZoneApplyPacket packet)
    {
        if (packet == null)
            return;

        switch (packet.Opcode)
        {
            case NetOpcode.HelloAck:
                break;
            case NetOpcode.ZoneAuthResult:
            {
                var result = (ZoneAuthResultMsg)packet.Body;
                if (!result.Ok)
                {
                    Debug.LogWarning($"[Net] zone auth failed {result.Code}: {result.Message}");
                    string line = string.IsNullOrWhiteSpace(result.Message)
                        ? (string.IsNullOrWhiteSpace(result.Code) ? "Zone refused the login." : result.Code)
                        : result.Message;
                    CharSelectEnterWorld.Failed(line);
                    return;
                }

                _ = _enterWorldAsync();
                break;
            }
            case NetOpcode.EntitySpawn:
                _session.ApplyEntitySpawn((EntitySpawnMsg)packet.Body);
                break;
            case NetOpcode.EntityDespawn:
                _session.ApplyEntityDespawn((EntityDespawnMsg)packet.Body);
                break;
            case NetOpcode.PlayerStats:
                _session.ApplyPlayerStats((PlayerStatsMsg)packet.Body);
                break;
            case NetOpcode.ItemList:
                _session.ApplyItemList((ItemListNetMsg)packet.Body);
                break;
            case NetOpcode.SkillList:
                _session.ApplySkillList((SkillListNetMsg)packet.Body);
                break;
            case NetOpcode.UserInfo:
                _session.ApplyUserInfo((UserInfoNetMsg)packet.Body);
                break;
            case NetOpcode.ShortCutInit:
                NetShortCutReceiver.ApplyInit((ShortCutInitNetMsg)packet.Body);
                break;
            case NetOpcode.ShortCutRegister:
                NetShortCutReceiver.ApplyRegister((ShortCutNetMsg)packet.Body);
                break;
            case NetOpcode.ShortCutDelete:
                NetShortCutReceiver.ApplyDelete((ShortCutDelNetMsg)packet.Body);
                break;
            case NetOpcode.EtcStatusUpdate:
                NetShortCutReceiver.ApplyEtcStatus((EtcStatusUpdateNetMsg)packet.Body);
                break;
            case NetOpcode.MoveState:
                _session.ApplyMoveState((MoveStateMsg)packet.Body);
                break;
            case NetOpcode.ChangeMoveType:
                NetShortCutReceiver.ApplyChangeMoveType((ChangeMoveTypeNetMsg)packet.Body);
                break;
            case NetOpcode.ChangeWaitType:
                _session.ApplyChangeWaitType((ChangeWaitTypeNetMsg)packet.Body);
                break;
            case NetOpcode.SocialAction:
                _session.ApplySocialAction((SocialActionNetMsg)packet.Body);
                break;
            case NetOpcode.TargetSelected:
                _session.ApplyTargetSelected((TargetSelectedNetMsg)packet.Body);
                break;
            case NetOpcode.TargetUnselected:
                _session.ApplyTargetUnselected((TargetUnselectedNetMsg)packet.Body);
                break;
            case NetOpcode.AttackState:
                _session.ApplyAttackState((AttackStateMsg)packet.Body);
                break;
            case NetOpcode.AutoAttackStart:
                _session.ApplyAutoAttackStart((int)((AttackStanceNetMsg)packet.Body).EntityValue);
                break;
            case NetOpcode.AutoAttackStop:
                _session.ApplyAutoAttackStop((int)((AttackStanceNetMsg)packet.Body).EntityValue);
                break;
            case NetOpcode.AutoSoulShot:
            {
                AutoSoulShotNetMsg shot = (AutoSoulShotNetMsg)packet.Body;
                NetAutoShot.Apply(shot.ItemId, shot.Enabled);
                SkillbarWindow skillbar = SkillbarPacketServices.Skillbar;
                if (skillbar != null)
                    skillbar.RefreshAutoShotMarks();
                break;
            }
            case NetOpcode.StatusUpdate:
                _session.ApplyStatusUpdate((StatusUpdateNetMsg)packet.Body);
                break;
            case NetOpcode.EntityDie:
                _session.ApplyEntityDie((EntityDieMsg)packet.Body);
                break;
            case NetOpcode.DropItem:
                _session.ApplyDropItem((DropItemNetMsg)packet.Body);
                break;
            case NetOpcode.InventoryUpdate:
                _session.ApplyInventoryUpdate((InventoryUpdateNetMsg)packet.Body);
                break;
            case NetOpcode.CreatureSay:
                _session.ApplyCreatureSay((CreatureSayNetMsg)packet.Body);
                break;
            case NetOpcode.SystemMessage:
                _session.ApplySystemMessage((SystemMessageNetMsg)packet.Body);
                break;
            case NetOpcode.MagicSkillUse:
                _session.ApplyMagicSkillUse((MagicSkillUseNetMsg)packet.Body);
                break;
            case NetOpcode.Revive:
                _session.ApplyRevive((ReviveNetMsg)packet.Body);
                break;
            case NetOpcode.TeleportToLocation:
                _session.ApplyTeleportToLocation((TeleportToLocationNetMsg)packet.Body);
                break;
            case NetOpcode.AbnormalStatusUpdate:
                _session.ApplyAbnormalStatusUpdate((AbnormalStatusUpdateNetMsg)packet.Body);
                break;
            case NetOpcode.NpcHtml:
                _session.ApplyNpcHtml((NpcHtmlNetMsg)packet.Body);
                break;
            case NetOpcode.BuyList:
                _session.ApplyBuyList((BuyListNetMsg)packet.Body);
                break;
            case NetOpcode.SellList:
                _session.ApplySellList((SellListNetMsg)packet.Body);
                break;
            case NetOpcode.MultiSellList:
                _session.ApplyMultiSellList((MultiSellListNetMsg)packet.Body);
                break;
            case NetOpcode.ShopBalance:
                _session.ApplyShopBalance((ShopBalanceNetMsg)packet.Body);
                break;
            case NetOpcode.WarehouseDepositList:
                _session.ApplyWarehouseDepositList((WarehouseDepositListNetMsg)packet.Body);
                break;
            case NetOpcode.WarehouseWithdrawList:
                _session.ApplyWarehouseWithdrawList((WarehouseWithdrawListNetMsg)packet.Body);
                break;
            case NetOpcode.QuestList:
                _session.ApplyQuestList((QuestListNetMsg)packet.Body);
                break;
            case NetOpcode.AcquireSkillList:
                _session.ApplyAcquireSkillList((AcquireSkillListNetMsg)packet.Body);
                break;
            case NetOpcode.AcquireSkillInfo:
                _session.ApplyAcquireSkillInfo((AcquireSkillInfoNetMsg)packet.Body);
                break;
            case NetOpcode.EnchantResult:
                _session.ApplyEnchantResult((EnchantResultNetMsg)packet.Body);
                break;
            case NetOpcode.PledgeStatus:
                _session.ApplyPledgeStatus((PledgeStatusNetMsg)packet.Body);
                break;
            case NetOpcode.PledgeMemberList:
                _session.ApplyPledgeMemberList((PledgeMemberListNetMsg)packet.Body);
                break;
            case NetOpcode.ClanAskJoin:
                _session.ApplyClanAskJoin((ClanAskJoinNetMsg)packet.Body);
                break;
            case NetOpcode.ClanJoinResult:
                _session.ApplyClanJoinResult((ClanJoinResultNetMsg)packet.Body);
                break;
            case NetOpcode.PartyAskJoin:
                _session.ApplyPartyAskJoin((PartyAskJoinNetMsg)packet.Body);
                break;
            case NetOpcode.PartyJoinResult:
                _session.ApplyPartyJoinResult((PartyJoinResultNetMsg)packet.Body);
                break;
            case NetOpcode.PartyMemberList:
                _session.ApplyPartyMemberList((PartyMemberListNetMsg)packet.Body);
                break;
            case NetOpcode.TradeAsk:
                _session.ApplyTradeAsk((TradeAskNetMsg)packet.Body);
                break;
            case NetOpcode.TradeRequestResult:
                _session.ApplyTradeRequestResult((TradeRequestResultNetMsg)packet.Body);
                break;
            case NetOpcode.TradeStart:
                _session.ApplyTradeStart((TradeStartNetMsg)packet.Body);
                break;
            case NetOpcode.TradeOfferUpdate:
                _session.ApplyTradeOfferUpdate((TradeOfferUpdateNetMsg)packet.Body);
                break;
            case NetOpcode.TradeConfirmState:
                _session.ApplyTradeConfirmState((TradeConfirmStateNetMsg)packet.Body);
                break;
            case NetOpcode.TradeFinished:
                _session.ApplyTradeFinished((TradeFinishedNetMsg)packet.Body);
                break;
            case NetOpcode.PositionCorrection:
                _session.ApplyCorrection((PositionCorrectionMsg)packet.Body);
                break;
            case NetOpcode.SiegeInfo:
                _ = (SiegeInfoNetMsg)packet.Body;
                break;
            case NetOpcode.OlympiadStatus:
                _ = (OlympiadStatusNetMsg)packet.Body;
                break;
            case NetOpcode.SevenSignsStatus:
                _ = (SevenSignsStatusNetMsg)packet.Body;
                break;
            case NetOpcode.DungeonEntered:
                _ = (DungeonEnteredNetMsg)packet.Body;
                break;
            case NetOpcode.PrivateStoreInfo:
                _session.ApplyPrivateStoreInfo((PrivateStoreInfoNetMsg)packet.Body);
                break;
            case NetOpcode.FriendList:
                _ = (FriendListNetMsg)packet.Body;
                break;
            case NetOpcode.BossStatus:
                _ = (BossStatusNetMsg)packet.Body;
                break;
            case NetOpcode.OlympiadMatch:
                _ = (OlympiadMatchNetMsg)packet.Body;
                break;
            case NetOpcode.OlympiadCountdown:
                _ = (OlympiadCountdownNetMsg)packet.Body;
                break;
            case NetOpcode.OlympiadResult:
                _ = (OlympiadResultNetMsg)packet.Body;
                break;
            case NetOpcode.SevenSignsSky:
                _ = (SevenSignsSkyNetMsg)packet.Body;
                break;
            case NetOpcode.PrivateStoreListSell:
                _ = (PrivateStoreListSellNetMsg)packet.Body;
                break;
            case NetOpcode.PrivateStoreListBuy:
                _ = (PrivateStoreListBuyNetMsg)packet.Body;
                break;
            case NetOpcode.RecipeBookList:
                _session.ApplyRecipeBookList((RecipeBookListNetMsg)packet.Body);
                break;
            case NetOpcode.RecipeItemMakeInfo:
                _session.ApplyRecipeItemMakeInfo((RecipeItemMakeInfoNetMsg)packet.Body);
                break;
            case NetOpcode.RecipeShopSellList:
                _session.ApplyRecipeShopSellList((RecipeShopSellListNetMsg)packet.Body);
                break;
            case NetOpcode.RecipeShopItemInfo:
                _session.ApplyRecipeShopItemInfo((RecipeShopItemInfoNetMsg)packet.Body);
                break;
            case NetOpcode.FriendInviteAsk:
                _ = (FriendInviteAskNetMsg)packet.Body;
                break;
            case NetOpcode.FriendUpdate:
                _ = (FriendUpdateNetMsg)packet.Body;
                break;
            case NetOpcode.BlockList:
                _ = (BlockListNetMsg)packet.Body;
                break;
            case NetOpcode.AllyAskJoin:
                _ = (AllyAskJoinNetMsg)packet.Body;
                break;
            case NetOpcode.AllyInfo:
                _ = (AllyInfoNetMsg)packet.Body;
                break;
            case NetOpcode.PledgeInfo:
                _session.ApplyPledgeInfo((PledgeInfoNetMsg)packet.Body);
                break;
            case NetOpcode.PledgeWarList:
                _session.ApplyPledgeWarList((PledgeWarListNetMsg)packet.Body);
                break;
            case NetOpcode.ClanHallList:
                _ = (ClanHallListNetMsg)packet.Body;
                break;
            case NetOpcode.CrestData:
                _ = (CrestDataNetMsg)packet.Body;
                break;
            case NetOpcode.RelationChanged:
                _ = (RelationChangedNetMsg)packet.Body;
                break;
            case NetOpcode.CastleInfo:
                _ = (CastleInfoNetMsg)packet.Body;
                break;
            case NetOpcode.DoorStatus:
                _session.ApplyDoorStatus((DoorStatusNetMsg)packet.Body);
                break;
            case NetOpcode.ManorInfo:
                _ = (ManorInfoNetMsg)packet.Body;
                break;
            case NetOpcode.DuelEvent:
                _ = (DuelEventNetMsg)packet.Body;
                break;
            case NetOpcode.DuelUserInfo:
                _ = (DuelUserInfoNetMsg)packet.Body;
                break;
            case NetOpcode.SummonInfo:
                _session.ApplySummonInfo((SummonInfoNetMsg)packet.Body);
                break;
            case NetOpcode.SummonGone:
                _session.ApplySummonGone((SummonGoneNetMsg)packet.Body);
                break;
            case NetOpcode.Ride:
                _session.ApplyRide((RideNetMsg)packet.Body);
                break;
            case NetOpcode.PlayerActionState:
                _session.ApplyPlayerActionState((PlayerActionStateNetMsg)packet.Body);
                break;
            case NetOpcode.WeddingEngageAsk:
                _session.ApplyWeddingEngageAsk((WeddingEngageAskNetMsg)packet.Body);
                break;
            // Item augmentation (variation) UI — Mobius Ex* handlers; no client UI yet.
            case NetOpcode.AugmentShowMakeWindow:
            case NetOpcode.AugmentShowCancelWindow:
            case NetOpcode.AugmentPutItemResult:
            case NetOpcode.AugmentPutRefinerResult:
            case NetOpcode.AugmentPutGemResult:
            case NetOpcode.AugmentVariationResult:
            case NetOpcode.AugmentPutCancelItemResult:
            case NetOpcode.AugmentCancelResult:
                break;
            case NetOpcode.CursedWeaponList:
            case NetOpcode.CursedWeaponLocation:
                break;
            case NetOpcode.PackageToList:
                _ = (PackageToListNetMsg)packet.Body;
                break;
            case NetOpcode.PackageSendableList:
                _ = (PackageSendableListNetMsg)packet.Body;
                break;
            case NetOpcode.MonRaceInfo:
                _ = (MonRaceInfoNetMsg)packet.Body;
                break;
            case NetOpcode.PetInventory:
                _ = (PetInventoryNetMsg)packet.Body;
                break;
            case NetOpcode.FishingEvent:
                _ = (FishingEventNetMsg)packet.Body;
                break;
            case NetOpcode.MagicSkillLaunched:
                _ = (MagicSkillLaunchedNetMsg)packet.Body;
                break;
            case NetOpcode.MagicSkillCanceled:
                _ = (MagicSkillCanceledNetMsg)packet.Body;
                break;
            case NetOpcode.PlaySound:
                NetQuestFxApply.ApplyPlaySound((PlaySoundNetMsg)packet.Body);
                break;
            case NetOpcode.QuestDirection:
                NetQuestFxApply.ApplyQuestDirection((QuestDirectionNetMsg)packet.Body);
                break;
            case NetOpcode.QuestNpcMarks:
                NetQuestFxApply.ApplyNpcMarks((QuestNpcMarksNetMsg)packet.Body);
                break;
            case NetOpcode.TutorialUi:
                NetQuestFxApply.ApplyTutorial((TutorialUiNetMsg)packet.Body);
                break;
            case NetOpcode.PartyMemberPosition:
            {
                var partyMap = (PartyMemberPositionNetMsg)packet.Body;
                PartyMapApplyIncomingPacketServices.ApplyPartyMemberPositions(partyMap);
                break;
            }
            case NetOpcode.PartyMemberUpdate:
                _ = (PartyMemberUpdateNetMsg)packet.Body;
                break;
            case NetOpcode.PartySpelled:
                _ = (AbnormalStatusUpdateNetMsg)packet.Body;
                break;
            case NetOpcode.Reject:
            {
                var reject = (RejectMsg)packet.Body;
                Debug.LogWarning($"[Net] zone reject {reject.Code}: {reject.Message}");
                NetSession.NoteReject(reject.Code, reject.Message);
                if (CharSelectEnterWorld.IsEntering)
                    CharSelectEnterWorld.Failed(LobbyCharacterText.ErrorMessage(reject.Code, reject.Message));
                break;
            }
            // Augmentation UI packets (Mobius variation make/cancel) — wired when UI lands.
            case NetOpcode.AugmentShowMakeWindow:
            case NetOpcode.AugmentShowCancelWindow:
            case NetOpcode.AugmentPutItemResult:
            case NetOpcode.AugmentPutRefinerResult:
            case NetOpcode.AugmentPutGemResult:
            case NetOpcode.AugmentVariationResult:
            case NetOpcode.AugmentPutCancelItemResult:
            case NetOpcode.AugmentCancelResult:
            case NetOpcode.CursedWeaponList:
            case NetOpcode.CursedWeaponLocation:
                break;
            default:
                Debug.LogWarning($"[Net] unhandled zone opcode 0x{packet.Opcode:X4}");
                break;
        }
    }
}
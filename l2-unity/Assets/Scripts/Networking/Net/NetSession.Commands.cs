using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;

/// <summary>
/// Zone command send wrappers (#131 B4 / NetSession decomposition).
/// Thin delegates onto <see cref="NetCommandSender"/>.
/// </summary>
public sealed partial class NetSession
{
    public Task SendMove(int destX, int destY, int destZ) =>
        _commandSender.SendMoveAsync(destX, destY, destZ);

    public Task SendSelectTarget(int targetObjectId) =>
        _commandSender.SendSelectTargetAsync(targetObjectId);

    public Task SendAttackRequest(int targetObjectId) =>
        _commandSender.SendAttackRequestAsync(targetObjectId);

    public Task SendPickupRequest(int groundObjectId)
    {
        if (_zone != null && _zone.Connected && groundObjectId > 0)
            _pendingPickupObjectId = groundObjectId;
        return _commandSender.SendPickupRequestAsync(groundObjectId);
    }

    /// <summary>
    /// True when <paramref name="groundObjectId"/> matches the outstanding pickup request.
    /// Clears the pending id so a later despawn of the same object is a normal delete.
    /// </summary>
    public bool TryTakePendingPickup(int groundObjectId, out int pickerObjectId)
    {
        pickerObjectId = SessionObjectId;
        if (groundObjectId <= 0 || groundObjectId != _pendingPickupObjectId || pickerObjectId <= 0)
            return false;
        _pendingPickupObjectId = 0;
        return true;
    }

    public Task SendDropItemRequest(int objectId, int count, int x, int y, int z) =>
        _commandSender.SendDropItemRequestAsync(objectId, count, x, y, z);

    /// <summary>Mobius <c>RequestDestroyItem</c>: delete the bag slot, no ground spawn.</summary>
    public Task SendDestroyItemRequest(int objectId, int count) =>
        _commandSender.SendDestroyItemRequestAsync(objectId, count);

    public Task SendUseItem(int objectId) =>
        _commandSender.SendUseItemAsync(objectId);

    /// <summary>
    /// Mobius <c>RequestAutoSoulShot</c>: right-clicking a shot on the hotbar turns automatic use on/off.
    /// <paramref name="itemId"/> is the shot template id the client carries.
    /// </summary>
    public Task SendRequestAutoSoulShot(int itemId, bool enabled) =>
        _commandSender.SendRequestAutoSoulShotAsync(itemId, enabled);

    public Task SendUnequipItem(int paperdollSlot) =>
        _commandSender.SendUnequipItemAsync(paperdollSlot);

    /// <summary>
    /// Registers one hotbar slot. <paramref name="worldSlot"/> is the client world slot
    /// (slot + page * 12); the zone echoes the result as <c>ShortCutRegister</c> or
    /// <c>ShortCutDelete</c>.
    /// </summary>
    public Task SendShortCutReg(int type, int worldSlot, int id, int level) =>
        _commandSender.SendShortCutRegAsync(type, worldSlot, id, level);

    /// <summary>Clears one hotbar slot (world slot = slot + page * 12).</summary>
    public Task SendShortCutDel(int worldSlot) =>
        _commandSender.SendShortCutDelAsync(worldSlot);

    /// <summary>
    /// Mobius <c>RequestActionUse</c>: 0 = sit/stand, 1 = walk/run, 12/13/14 = social gestures.
    /// The zone answers with ChangeWaitType / ChangeMoveType / SocialAction.
    /// </summary>
    public Task SendActionUse(int actionId, bool ctrlPressed = false, bool shiftPressed = false) =>
        _commandSender.SendActionUseAsync(actionId, ctrlPressed, shiftPressed);

    /// <summary>
    /// Position heartbeat (L2 coordinates + heading). The zone reconciles the client's
    /// position against its own copy; without it a walk drifts and then snaps back.
    /// </summary>
    public Task SendValidatePosition(int x, int y, int z, int heading) =>
        _commandSender.SendValidatePositionAsync(x, y, z, heading);

    public Task SendSay(string text, int chatType) =>
        _commandSender.SendSayAsync(text, chatType);

    public Task SendUseSkill(int skillId, int targetObjectId, bool ctrlPressed) =>
        _commandSender.SendUseSkillAsync(skillId, targetObjectId, ctrlPressed);

    public Task SendReviveRequest(int action) =>
        _commandSender.SendReviveRequestAsync(action);

    /// <summary>
    /// Mobius <c>RequestAcquireSkillInfo</c>: ask the zone for the confirmation page of one trainer offer.
    /// Called by the trainer window when a row is clicked.
    /// </summary>
    public Task SendRequestAcquireSkillInfo(int skillId, int level, int skillType) =>
        _commandSender.SendRequestAcquireSkillInfoAsync(skillId, level, skillType);

    /// <summary>Mobius <c>RequestAcquireSkill</c>: buy the offer (SP is spent by the zone).</summary>
    public Task SendRequestAcquireSkill(int skillId, int level, int skillType) =>
        _commandSender.SendRequestAcquireSkillAsync(skillId, level, skillType);

    /// <summary>Mobius <c>RequestEnchantItem</c>: the bag scroll the player clicked opens the window.</summary>
    public Task SendEnchantRequest(int scrollObjectId) =>
        _commandSender.SendEnchantRequestAsync(scrollObjectId);

    /// <summary>The item placed in the enchant slot; the zone rolls the scroll against it.</summary>
    public Task SendEnchantConfirm(int targetObjectId) =>
        _commandSender.SendEnchantConfirmAsync(targetObjectId);

    /// <summary>The player closed the window without rolling. The zone clears the scroll.</summary>
    public Task SendEnchantCancel() =>
        _commandSender.SendEnchantCancelAsync();

    public Task SendNpcTalk(int npcObjectId) =>
        _commandSender.SendNpcTalkAsync(npcObjectId);

    public Task SendBypassRequest(int npcObjectId, string command) =>
        _commandSender.SendBypassRequestAsync(npcObjectId, command);

    public void SendNpcLink(int npcObjectId, string command)
    {
        string list;
        int location;
        switch (NetBypass.Classify(command, out list, out location))
        {
            case NetBypass.Teleport:
            case NetBypass.SupportMagic:
            case NetBypass.GiveBlessing:
                // Same bypass verb as V2; specialized opcodes come later.
                _ = SendBypassRequest(npcObjectId, NetBypass.Normalize(command));
                return;
            default:
                _ = SendBypassRequest(npcObjectId, NetBypass.Normalize(command));
                return;
        }
    }

    public Task SendRequestBuyItem(int npcObjectId, int listId, NetIdCount[] items) =>
        _commandSender.SendRequestBuyItemAsync(npcObjectId, listId, items);

    public Task SendRequestSellItem(int npcObjectId, NetIdCount[] items) =>
        _commandSender.SendRequestSellItemAsync(npcObjectId, items);

    public Task SendRequestMultiSellChoose(int npcObjectId, int listId, int entryId, int amount) =>
        _commandSender.SendRequestMultiSellChooseAsync(npcObjectId, listId, entryId, amount);

    public Task SendWarehouseDeposit(int npcObjectId, NetIdCount[] lines) =>
        _commandSender.SendWarehouseDepositAsync(npcObjectId, lines);

    public Task SendWarehouseWithdraw(int npcObjectId, NetIdCount[] lines) =>
        _commandSender.SendWarehouseWithdrawAsync(npcObjectId, lines);

    public Task SendRequestQuestList() =>
        _commandSender.SendRequestQuestListAsync();

    public Task SendRequestQuestAbort(int questId) =>
        _commandSender.SendRequestQuestAbortAsync(questId);

    public Task SendPartyInviteRequest(int targetObjectId, int itemDistribution = 0) =>
        _commandSender.SendPartyInviteRequestAsync(targetObjectId, itemDistribution);

    public Task SendPartyAnswerJoinRequest(int requestorObjectId, int response) =>
        _commandSender.SendPartyAnswerJoinRequestAsync(requestorObjectId, response);

    public Task SendPartyLeaveRequest() =>
        _commandSender.SendPartyLeaveRequestAsync();

    public Task SendPartyKickRequest(int targetObjectId) =>
        _commandSender.SendPartyKickRequestAsync(targetObjectId);

    public Task SendTradeRequest(int targetObjectId) =>
        _commandSender.SendTradeRequestAsync(targetObjectId);

    public Task SendTradeAnswerRequest(int requestorObjectId, int response) =>
        _commandSender.SendTradeAnswerRequestAsync(requestorObjectId, response);

    public Task SendTradeAddItemRequest(int inventoryObjectId, int count) =>
        _commandSender.SendTradeAddItemRequestAsync(inventoryObjectId, count);

    public Task SendTradeDoneRequest(int response) =>
        _commandSender.SendTradeDoneRequestAsync(response);

    /// <summary>Mobius <c>RequestCreatePledge</c>: found a clan with this name.</summary>
    public Task SendClanCreateRequest(string name) =>
        _commandSender.SendClanCreateRequestAsync(name);

    /// <summary>Mobius <c>RequestJoinPledge</c>: ask the owner of this clan to enrol the player.</summary>
    public Task SendClanInviteRequest(int targetObjectId) =>
        _commandSender.SendClanInviteRequestAsync(targetObjectId);

    /// <summary>Mobius <c>RequestAnswerJoinPledge</c>. Response 0 declines.</summary>
    public Task SendClanAnswerJoinRequest(int requestorObjectId, int response) =>
        _commandSender.SendClanAnswerJoinRequestAsync(requestorObjectId, response);

    /// <summary>Mobius <c>RequestWithdrawalPledge</c>.</summary>
    public Task SendClanLeaveRequest() =>
        _commandSender.SendClanLeaveRequestAsync();

    /// <summary>Mobius <c>RequestOustPledgeMember</c>, by object id.</summary>
    public Task SendClanKickRequest(int targetObjectId) =>
        _commandSender.SendClanKickRequestAsync(targetObjectId);

    /// <summary>Mobius <c>RequestDissolvePledge</c>.</summary>
    public Task SendClanDissolveRequest() =>
        _commandSender.SendClanDissolveRequestAsync();

    /// <summary>Mobius <c>RequestPledgeReorganizeMember</c>-side cancel of a running dissolution.</summary>
    public Task SendClanCancelDissolveRequest() =>
        _commandSender.SendClanCancelDissolveRequestAsync();

    /// <summary>Mobius <c>RequestTargetCanceld</c>: drop the selection on the zone side too.</summary>
    public Task SendTargetCancel() =>
        _commandSender.SendTargetCancelAsync();
}

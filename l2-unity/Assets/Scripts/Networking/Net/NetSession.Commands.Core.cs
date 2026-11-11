using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;

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

    public Task SendDestroyItemRequest(int objectId, int count) =>
        _commandSender.SendDestroyItemRequestAsync(objectId, count);

    public Task SendUseItem(int objectId) =>
        _commandSender.SendUseItemAsync(objectId);

    public Task SendRequestAutoSoulShot(int itemId, bool enabled) =>
        _commandSender.SendRequestAutoSoulShotAsync(itemId, enabled);

    public Task SendUnequipItem(int paperdollSlot) =>
        _commandSender.SendUnequipItemAsync(paperdollSlot);

    public Task SendShortCutReg(int type, int worldSlot, int id, int level) =>
        _commandSender.SendShortCutRegAsync(type, worldSlot, id, level);

    public Task SendShortCutDel(int worldSlot) =>
        _commandSender.SendShortCutDelAsync(worldSlot);

    public Task SendActionUse(int actionId, bool ctrlPressed = false, bool shiftPressed = false) =>
        _commandSender.SendActionUseAsync(actionId, ctrlPressed, shiftPressed);

    public Task SendValidatePosition(int x, int y, int z, int heading) =>
        _commandSender.SendValidatePositionAsync(x, y, z, heading);

    public Task SendSay(string text, int chatType) =>
        _commandSender.SendSayAsync(text, chatType);

    public Task SendUseSkill(int skillId, int targetObjectId, bool ctrlPressed) =>
        _commandSender.SendUseSkillAsync(skillId, targetObjectId, ctrlPressed);

    public Task SendReviveRequest(int action) =>
        _commandSender.SendReviveRequestAsync(action);

    public Task SendRequestAcquireSkillInfo(int skillId, int level, int skillType) =>
        _commandSender.SendRequestAcquireSkillInfoAsync(skillId, level, skillType);

    public Task SendRequestAcquireSkill(int skillId, int level, int skillType) =>
        _commandSender.SendRequestAcquireSkillAsync(skillId, level, skillType);

    public Task SendEnchantRequest(int scrollObjectId) =>
        _commandSender.SendEnchantRequestAsync(scrollObjectId);

    public Task SendEnchantConfirm(int targetObjectId) =>
        _commandSender.SendEnchantConfirmAsync(targetObjectId);

    public Task SendEnchantCancel() =>
        _commandSender.SendEnchantCancelAsync();

    public Task SendNpcTalk(int npcObjectId) =>
        _commandSender.SendNpcTalkAsync(npcObjectId);

    public Task SendBypassRequest(int npcObjectId, string command) =>
        _commandSender.SendBypassRequestAsync(npcObjectId, command);
}

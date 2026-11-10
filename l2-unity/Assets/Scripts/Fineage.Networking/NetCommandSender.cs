using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;

public sealed partial class NetCommandSender
{
    readonly Func<NetTransport> _zone;
    readonly Func<uint> _nextSeq;

    public NetCommandSender(Func<NetTransport> zone, Func<uint> nextSeq)
    {
        _zone = zone ?? throw new ArgumentNullException(nameof(zone));
        _nextSeq = nextSeq ?? throw new ArgumentNullException(nameof(nextSeq));
    }

    public Task SendMoveAsync(int destX, int destY, int destZ)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.MoveInput,
            NetCodec.EncodeMoveInput(new MoveInputMsg { DestX = destX, DestY = destY, DestZ = destZ }),
            _nextSeq());
    }

    public Task SendSelectTargetAsync(int targetObjectId)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.SelectTarget,
            NetCodec.EncodeSelectTarget(new SelectTargetNetMsg { TargetObjectId = targetObjectId }),
            _nextSeq());
    }

    public Task SendAttackRequestAsync(int targetObjectId)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.AttackRequest,
            NetCodec.EncodeAttackRequest(new AttackRequestNetMsg { TargetObjectId = targetObjectId }),
            _nextSeq());
    }

    public Task SendUseSkillAsync(int skillId, int targetObjectId, bool ctrlPressed)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.UseSkill,
            NetCodec.EncodeUseSkill(new UseSkillNetMsg
            {
                SkillId = skillId,
                TargetObjectId = targetObjectId,
                CtrlPressed = ctrlPressed
            }),
            _nextSeq());
    }

    public Task SendPickupRequestAsync(int groundObjectId)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.PickupRequest,
            NetCodec.EncodePickupRequest(new PickupRequestNetMsg { GroundObjectId = groundObjectId }),
            _nextSeq());
    }

    public Task SendDropItemRequestAsync(int objectId, int count, int x, int y, int z)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.DropItemRequest,
            NetCodec.EncodeDropItemRequest(new DropItemRequestNetMsg
            {
                ObjectId = objectId,
                Count = count,
                X = x,
                Y = y,
                Z = z
            }),
            _nextSeq());
    }

    public Task SendUseItemAsync(int objectId)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.UseItem,
            NetCodec.EncodeUseItem(new UseItemNetMsg { ObjectId = objectId }),
            _nextSeq());
    }

    public Task SendUnequipItemAsync(int paperdollSlot)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.UnequipItem,
            NetCodec.EncodeUnequipItem(new UnequipItemNetMsg { PaperdollSlot = paperdollSlot }),
            _nextSeq());
    }

    public Task SendTargetCancelAsync()
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(NetOpcode.TargetCancelRequest, Array.Empty<byte>(), _nextSeq());
    }

    public Task SendDestroyItemRequestAsync(int objectId, int count)
    {
        if (objectId <= 0 || !TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.DestroyItemRequest,
            NetCodec.EncodeDestroyItemRequest(new DestroyItemRequestMsg { ObjectId = objectId, Count = count }),
            _nextSeq());
    }

    public Task SendRequestAutoSoulShotAsync(int itemId, bool enabled)
    {
        if (itemId <= 0 || !TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.RequestAutoSoulShot,
            NetCodec.EncodeAutoSoulShot(new AutoSoulShotNetMsg { ItemId = itemId, Enabled = enabled }),
            _nextSeq());
    }

    public Task SendShortCutRegAsync(int type, int worldSlot, int id, int level)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.RequestShortCutReg,
            NetCodec.EncodeShortCut(new ShortCutNetMsg
            {
                Type = type,
                Slot = worldSlot,
                Id = id,
                Level = level
            }),
            _nextSeq());
    }

    public Task SendShortCutDelAsync(int worldSlot)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.RequestShortCutDel,
            NetCodec.EncodeShortCutDel(new ShortCutDelNetMsg { Slot = worldSlot }),
            _nextSeq());
    }

    public Task SendActionUseAsync(int actionId, bool ctrlPressed, bool shiftPressed)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.RequestActionUse,
            NetCodec.EncodeActionUse(new ActionUseNetMsg
            {
                ActionId = actionId,
                CtrlPressed = ctrlPressed,
                ShiftPressed = shiftPressed
            }),
            _nextSeq());
    }

    public Task SendValidatePositionAsync(int x, int y, int z, int heading)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.ValidatePosition,
            NetCodec.EncodeValidatePosition(new ValidatePositionMsg
            {
                X = x,
                Y = y,
                Z = z,
                Heading = heading
            }),
            _nextSeq());
    }

    public Task SendSayAsync(string text, int chatType)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.Say,
            NetCodec.EncodeSay(new SayNetMsg { Text = text ?? "", ChatType = chatType }),
            _nextSeq());
    }

    public Task SendReviveRequestAsync(int action)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.ReviveRequest,
            NetCodec.EncodeReviveRequest(new ReviveRequestNetMsg { Action = action }),
            _nextSeq());
    }

    public Task SendRequestAcquireSkillInfoAsync(int skillId, int level, int skillType)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.RequestAcquireSkillInfo,
            NetCodec.EncodeAcquireSkillRequest(skillId, level, skillType),
            _nextSeq());
    }

    public Task SendRequestAcquireSkillAsync(int skillId, int level, int skillType)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.RequestAcquireSkill,
            NetCodec.EncodeAcquireSkillRequest(skillId, level, skillType),
            _nextSeq());
    }

    public Task SendEnchantRequestAsync(int scrollObjectId)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.EnchantRequest,
            NetCodec.EncodeEnchantRequest(new EnchantRequestNetMsg { ScrollObjectId = scrollObjectId }),
            _nextSeq());
    }

    public Task SendEnchantConfirmAsync(int targetObjectId)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.EnchantConfirm,
            NetCodec.EncodeEnchantConfirm(new EnchantConfirmNetMsg { TargetObjectId = targetObjectId }),
            _nextSeq());
    }

    public Task SendEnchantCancelAsync()
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(NetOpcode.EnchantCancel, Array.Empty<byte>(), _nextSeq());
    }

    public Task SendNpcTalkAsync(int npcObjectId)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.NpcTalk,
            NetCodec.EncodeNpcTalk(new NpcTalkNetMsg { NpcObjectId = npcObjectId }),
            _nextSeq());
    }

    public Task SendBypassRequestAsync(int npcObjectId, string command)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.BypassRequest,
            NetCodec.EncodeBypassRequest(new BypassRequestNetMsg
            {
                NpcObjectId = npcObjectId,
                Command = command ?? ""
            }),
            _nextSeq());
    }

    public Task SendRequestMultiSellChooseAsync(int npcObjectId, int listId, int entryId, int amount)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.RequestMultiSellChoose,
            NetCodec.EncodeRequestMultiSellChoose(new RequestMultiSellChooseNetMsg
            {
                NpcObjectId = npcObjectId,
                ListId = listId,
                EntryId = entryId,
                Amount = amount
            }),
            _nextSeq());
    }

    static ShopCountNet[] MapShopCounts(NetIdCount[] items)
    {
        if (items == null || items.Length == 0)
            return Array.Empty<ShopCountNet>();
        var rows = new ShopCountNet[items.Length];
        for (int i = 0; i < items.Length; i++)
            rows[i] = new ShopCountNet { ItemId = items[i].Id, Count = items[i].Count };
        return rows;
    }

    static ShopObjectCountNet[] MapShopObjectCounts(NetIdCount[] items)
    {
        if (items == null || items.Length == 0)
            return Array.Empty<ShopObjectCountNet>();
        var rows = new ShopObjectCountNet[items.Length];
        for (int i = 0; i < items.Length; i++)
            rows[i] = new ShopObjectCountNet { ObjectId = items[i].Id, Count = items[i].Count };
        return rows;
    }

    bool TryGetZone(out NetTransport zone)
    {
        zone = _zone();
        return zone != null && zone.Connected;
    }
}
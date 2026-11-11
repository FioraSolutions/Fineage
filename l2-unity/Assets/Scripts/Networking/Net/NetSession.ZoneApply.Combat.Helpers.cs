using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetSession
{
    internal void ApplyChangeWaitType(ChangeWaitTypeNetMsg msg)
    {
        ChangeWaitTypeDto dto = ChangeWaitTypeDto.FromNet((int)msg.EntityValue, msg.WaitType, msg.X, msg.Y, msg.Z);
        WorldApplyIncomingPacketServices.QueueWorld(apply => apply.ChangeWaitType(dto));
    }

    internal void ApplySocialAction(SocialActionNetMsg msg)
    {
        SocialActionDto dto = SocialActionDto.FromNet((int)msg.EntityValue, msg.ActionId);
        WorldApplyIncomingPacketServices.QueueWorld(apply => apply.SocialAction(dto));
    }

    internal void ApplyEntityDie(EntityDieMsg die)
    {
        DieDto dieDto = DieDto.FromZoneWire((int)die.EntityValue);
        if (_combatDie != null) _combatDie.Apply(dieDto);
        else DieApplyIncomingPacketServices.ApplyDie(dieDto);
    }

    internal void ApplyMagicSkillUse(MagicSkillUseNetMsg use)
    {
        var mapped = new NetMagicSkillUse
        {
            CasterObjectId = use.CasterObjectId,
            TargetObjectId = use.TargetObjectId,
            SkillId = use.SkillId,
            SkillLevel = use.SkillLevel,
            HitTimeMs = use.HitTimeMs,
            ReuseDelayMs = use.ReuseDelayMs
        };
        var dto = new MagicSkillUseDto();
        dto.InitFromNet(
            mapped.CasterObjectId,
            mapped.TargetObjectId,
            mapped.SkillId,
            mapped.SkillLevel,
            mapped.HitTimeMs,
            mapped.ReuseDelayMs);
        if (_skills != null) _skills.ApplyMagicSkillUse(dto);
        else MagicSkillUseApplyIncomingPacketServices.ApplyMagicSkillUse(dto);
    }

    internal void ApplyRevive(ReviveNetMsg revive)
    {
        var dto = new ReviveDto { ObjectId = revive.ObjectId };
        if (_presence != null) _presence.ApplyRevive(dto);
        else ReviveApplyIncomingPacketServices.ApplyRevive(dto);
    }

    internal void ApplyTeleportToLocation(TeleportToLocationNetMsg teleport)
    {
        TeleportToLocationDto dto = TeleportToLocationDto.FromZoneWire(
            teleport.ObjectId, teleport.X, teleport.Y, teleport.Z, teleport.Heading);
        if (_presence != null) _presence.ApplyTeleportToLocation(dto);
        else TeleportToLocationApplyIncomingPacketServices.ApplyTeleportToLocation(dto);
    }

    internal void ApplyAbnormalStatusUpdate(AbnormalStatusUpdateNetMsg update)
    {
        // FanoutAbnormal goes to owner + knownlist. Self → BufferPanel. Peers → entity TargetEffects
        // for TargetWindow (Mobius PartySpelled), never wipe the local buff tray.
        AbnormalEffectNet[] src = update.Effects ?? System.Array.Empty<AbnormalEffectNet>();
        var effects = new NetAbnormalEffect[src.Length];
        for (int i = 0; i < src.Length; i++)
        {
            AbnormalEffectNet e = src[i] ?? new AbnormalEffectNet();
            effects[i] = new NetAbnormalEffect
            {
                Id = e.Id,
                Value = e.Value,
                DurationSeconds = e.DurationSeconds,
                Slot = e.Slot ?? ""
            };
        }

        AbnormalStatusUpdateDto dto = AbnormalStatusUpdateDto.FromZoneEffects(update.ObjectId, effects);
        PlayerEntity local = DomainPacketServices.LocalPlayer;
        // EnterWorld can deliver AbnormalStatusUpdate before LocalPlayer is bound — use SessionObjectId.
        bool self = update.ObjectId == 0
            || (SessionObjectId != 0 && update.ObjectId == SessionObjectId)
            || (local != null && local.Identity != null && update.ObjectId == local.Identity.Id);
        if (self)
        {
            if (_status == null)
                throw new System.InvalidOperationException("NetSession StatusApplyService is not bound.");
            _status.ApplyAbnormalStatusUpdate(dto);
            return;
        }

        if (_status == null)
            throw new System.InvalidOperationException("NetSession StatusApplyService is not bound.");
        _status.ApplyPeerAbnormalStatusUpdate(dto);
    }
}

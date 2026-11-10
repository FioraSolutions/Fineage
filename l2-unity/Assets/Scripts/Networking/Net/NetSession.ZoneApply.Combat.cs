using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetSession
{

    internal void ApplyTargetSelected(TargetSelectedNetMsg msg)
    {
        MyTargetSelectDto dto = MyTargetSelectDto.FromNet(msg.TargetObjectId, msg.ColorHint);
        if (_combatTarget != null) _combatTarget.ApplyMyTargetSelected(dto);
        else MyTargetSelectedApplyIncomingPacketServices.ApplyMyTargetSelected(dto);
    }

    /// <summary>
    /// The zone dropped the selection. Clear the target window and nameplate without echoing another
    /// <c>RequestTargetCanceld</c> back: this arrived because that request was already answered.
    /// </summary>

    internal void ApplyTargetUnselected(TargetUnselectedNetMsg msg)
    {
        if (_combatTarget != null) _combatTarget.ClearTargetFromZone();
        else TargetPacketServices.Targets?.ClearTargetSilently();
    }


    internal void ApplyAttackState(AttackStateMsg attack)
    {
        var dto = new AttackDto();
        dto.InitFromNet(
            (int)attack.AttackerValue,
            (int)attack.TargetValue,
            attack.Damage,
            attack.Flags,
            attack.AttackerX,
            attack.AttackerY,
            attack.AttackerZ,
            attack.TargetX,
            attack.TargetY,
            attack.TargetZ);
        if (_combatAttack != null) _combatAttack.Apply(dto);
        else AttackApplyIncomingPacketServices.ApplyAttack(dto);
    }

    /// <summary>
    /// Mobius <c>AutoAttackStart</c>: the pawn raises its weapon. The zone was already fanning this,
    /// but with no case here a Net client never put an actor into attack stance.
    /// </summary>

    internal void ApplyAutoAttackStart(int entityId)
    {
        AutoAttackStartDto dto = AutoAttackStartDto.FromNet(entityId);
        WorldApplyIncomingPacketServices.QueueWorld(apply => apply.AutoAttackStart(dto));
    }

    /// <summary>Mobius <c>AutoAttackStop</c>: stance ended, so the pawn drops back to its wait pose.</summary>

    internal void ApplyAutoAttackStop(int entityId)
    {
        AutoAttackStopDto dto = AutoAttackStopDto.FromNet(entityId);
        WorldApplyIncomingPacketServices.QueueWorld(apply => apply.AutoAttackStop(dto));
    }



    internal void ApplyStatusUpdate(StatusUpdateNetMsg status)
    {
        var attributes = new System.Collections.Generic.List<StatusUpdate.Attribute>();
        int n = status.AttributeIds == null || status.Values == null
            ? 0
            : Mathf.Min(status.AttributeIds.Length, status.Values.Length);
        for (int i = 0; i < n; i++)
            attributes.Add(new StatusUpdate.Attribute(status.AttributeIds[i], status.Values[i]));
        if (_statusUpdate != null) _statusUpdate.Apply((int)status.EntityValue, attributes);
        else StatusUpdateApplyIncomingPacketServices.ApplyStatusUpdate((int)status.EntityValue, attributes);
    }

    /// <summary>Mobius ChangeWaitType: sit/stand on the actor, self included so a sit is acknowledged.</summary>

    internal void ApplyChangeWaitType(ChangeWaitTypeNetMsg msg)
    {
        ChangeWaitTypeDto dto = ChangeWaitTypeDto.FromNet((int)msg.EntityValue, msg.WaitType, msg.X, msg.Y, msg.Z);
        WorldApplyIncomingPacketServices.QueueWorld(apply => apply.ChangeWaitType(dto));
    }

    /// <summary>Mobius SocialAction: a gesture on the actor (15 = level up).</summary>

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
                DurationSeconds = e.DurationSeconds
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

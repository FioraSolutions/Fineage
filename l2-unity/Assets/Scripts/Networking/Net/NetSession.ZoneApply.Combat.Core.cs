using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;
using System.Collections.Generic;

public sealed partial class NetSession
{
    internal void ApplyTargetSelected(TargetSelectedNetMsg msg)
    {
        MyTargetSelectDto dto = MyTargetSelectDto.FromNet(msg.TargetObjectId, msg.ColorHint);
        if (_combatTarget != null) _combatTarget.ApplyMyTargetSelected(dto);
        else MyTargetSelectedApplyIncomingPacketServices.ApplyMyTargetSelected(dto);
    }

    internal void ApplyTargetUnselected(TargetUnselectedNetMsg msg)
    {
        if (_combatTarget != null) _combatTarget.ClearTargetFromZone();
        else TargetPacketServices.Targets?.ClearTargetSilently();
    }

    internal void ApplyAttackState(AttackStateMsg attack)
    {
        AttackHitNet[] extraSrc = attack.ExtraHits;
        int extraCount = extraSrc != null ? extraSrc.Length : 0;
        int[] extraDamage = null;
        int[] extraFlags = null;
        if (extraCount > 0)
        {
            extraDamage = new int[extraCount];
            extraFlags = new int[extraCount];
            for (int i = 0; i < extraCount; i++)
            {
                AttackHitNet row = extraSrc[i];
                extraDamage[i] = row != null ? row.Damage : 0;
                extraFlags[i] = row != null ? row.Flags : 0;
            }
        }

        int targetId = (int)attack.TargetValue;
        var dto = new AttackDto();
        dto.InitFromNet(
            (int)attack.AttackerValue,
            targetId,
            attack.Damage,
            attack.Flags,
            attack.AttackerX,
            attack.AttackerY,
            attack.AttackerZ,
            attack.TargetX,
            attack.TargetY,
            attack.TargetZ);
        if (extraCount > 0)
        {
            dto.SetHitList(AttackHitPresentation.BuildHitList(
                targetId,
                attack.Damage,
                attack.Flags,
                extraDamage,
                extraFlags));
        }
        if (_combatAttack != null) _combatAttack.Apply(dto);
        else AttackApplyIncomingPacketServices.ApplyAttack(dto);
    }

    internal void ApplyAutoAttackStart(int entityId)
    {
        AutoAttackStartDto dto = AutoAttackStartDto.FromNet(entityId);
        WorldApplyIncomingPacketServices.QueueWorld(apply => apply.AutoAttackStart(dto));
    }

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
}

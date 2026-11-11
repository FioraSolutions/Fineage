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

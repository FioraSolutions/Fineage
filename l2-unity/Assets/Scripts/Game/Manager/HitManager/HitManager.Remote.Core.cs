using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class HitManager
{
    public void RememberLocalAttack(AttackDto dto)
    {
        _localAttack = dto;
        _localAttackToken++;
        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player == null || dto == null)
            return;
        Entity target = player.GetTargetEntity();
        if (target == null)
            target = ResolveEntityFromWorld(dto.TargetId);
        SchedulePacketHits(player, target, dto);
    }

    public bool HasScheduledMeleeHits(Entity attacker)
    {
        if (attacker == null || attacker.Identity == null)
            return false;
        RemoteMeleeHit pending;
        if (!_remoteHits.TryGetValue(attacker.Identity.Id, out pending) || pending == null)
            return false;
        return pending.HitCount > 0 && pending.NextHitIndex < pending.HitCount;
    }

    /// <summary>
    /// One AttackShot animation notify → one row in the Attack packet hit list (dual/fist = two notifies).
    /// Arms per-hit soulshot charge and miss state; impact FX is SwordCollision OnHitCollider.
    /// </summary>
    public void PresentLocalAttackHit(Entity attacker, int hitIndex)
    {
        if (attacker == null || _localAttack == null)
            return;
        if (_localAttackToken == 0)
            return;

        Entity target = attacker.GetTargetEntity();
        TryPresentMeleeHitFromAttackPacket(
            attacker,
            target,
            _localAttack,
            hitIndex,
            playColliderImpactFx: false);
    }
}
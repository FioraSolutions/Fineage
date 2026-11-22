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

        Hit hit = AttackHitPresentation.ResolveHit(_localAttack, hitIndex);
        if (hit == null || hit.isMiss())
            return;

        if (attacker is PlayerEntity player)
            player.SetSelfHit(hit);

        if (hit.hasSoulshot())
        {
            attacker.IsSoulshotCharged = true;
            WeaponChargePresentation.PlayMeleeSoulshotConsume(attacker);
        }
        else
            attacker.IsSoulshotCharged = false;

        Entity target = attacker.GetTargetEntity();
        if (target == null && _localAttack != null)
            target = ResolveEntityFromWorld(_localAttack.TargetId);
        if (target != null && target != attacker)
        {
            CombatAudioHitContext.Set(attacker, hit.isCrit(), hit.isShielded());
            target.PlayHitReaction(hit.isCrit());
        }
    }
}
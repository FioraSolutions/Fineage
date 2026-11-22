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

        Entity target = attacker.GetTargetEntity();
        TryPresentMeleeHitFromAttackPacket(
            attacker,
            target,
            _localAttack,
            hitIndex,
            playColliderImpactFx: false);
    }
}
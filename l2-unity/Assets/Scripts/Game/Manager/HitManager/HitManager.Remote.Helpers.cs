using System;
using System.Collections.Generic;
using UnityEngine;

public partial class HitManager
{
    public void ArmRemoteMeleeHit(Entity attacker, Entity target, AttackDto dto)
    {
        Hit hit = dto != null ? dto.FirstHit : null;
        ArmRemoteMeleeHit(
            attacker,
            target,
            miss: hit != null && hit.isMiss(),
            soulshot: hit != null && hit.hasSoulshot(),
            hitTimeMs: 0,
            packet: dto);
    }

    public void ArmRemoteSkillHit(Entity attacker, Entity target, MagicSkillUseDto magic)
    {
        if (target == null || target == attacker)
            return;

        // Only true soulshot charge may arm the melee SS impact; spiritshot is weapon-only.
        ArmRemoteMeleeHit(
            attacker,
            target,
            miss: false,
            soulshot: attacker != null && attacker.IsSoulshotCharged && !attacker.IsSpiritshotCharged,
            hitTimeMs: magic != null ? magic.HitTime : 0);
    }

    void ArmRemoteMeleeHit(
        Entity attacker,
        Entity target,
        bool miss,
        bool soulshot,
        int hitTimeMs,
        AttackDto packet = null)
    {
        bool hasExtra = packet != null && packet.ArrHit != null && packet.ArrHit.Length > 0;
        if (attacker == null || attacker is PlayerEntity)
            return;
        if (target == null && !hasExtra)
            return;
        if (!(attacker is MonsterEntity) && !(attacker is UserEntity))
            return;
        if (attacker.Identity == null)
            return;

        if (miss && !hasExtra)
            return;

        if (IsBowWeapon(attacker))
        {
            DropRemoteMeleeHit(attacker.Identity.Id);
            if (soulshot)
                attacker.IsSoulshotCharged = true;
            return;
        }

        FlushRemoteMeleeHit(attacker);

        AnimationEventsBase events = attacker.GetAnimatorController();
        if (events == null)
            events = attacker.GetComponentInChildren<AnimationEventsBase>(true);

        bool waitForShot = events != null;
        float delay = waitForShot
            ? 4f
            : hitTimeMs > 0
                ? Mathf.Clamp(hitTimeMs / 1000f * 0.75f, 0.12f, 2.5f)
                : ResolveRemoteHitDelay(attacker);
        int attackerId = attacker.Identity.Id;
        RemoteMeleeHit pending = new RemoteMeleeHit
        {
            AttackerId = attackerId,
            Attacker = attacker,
            Target = target,
            Soulshot = soulshot,
            Miss = miss,
            Packet = packet,
            WaitForShot = waitForShot,
            FireAt = Time.time + delay
        };

        if (waitForShot)
        {
            pending.Events = events;
            pending.OnShot = _ => TryFireRemoteMeleeHit(attackerId);
            events.OnAnimationAttackShot += pending.OnShot;
        }

        _remoteHits[attackerId] = pending;
    }
}

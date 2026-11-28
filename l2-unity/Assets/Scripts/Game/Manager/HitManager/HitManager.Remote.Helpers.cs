using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class HitManager
{
    public void ArmRemoteMeleeHit(Entity attacker, Entity target, AttackDto dto)
    {
        Hit hit = dto != null ? AttackHitPresentation.ResolveHit(dto, 0) : null;
        ArmRemoteMeleeHit(
            attacker,
            target,
            miss: hit != null && hit.isMiss(),
            soulshot: hit != null && hit.hasSoulshot(),
            hitTimeMs: 0,
            packet: dto);
    }

    public void ArmRemoteMeleeHit(Entity attacker, Entity target, MagicSkillUseDto magic)
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
        int hitCount = AttackHitPresentation.CountHits(packet);
        if (attacker == null || attacker is PlayerEntity)
            return;
        if (target == null && hitCount == 0)
            return;
        if (!(attacker is MonsterEntity) && !(attacker is UserEntity))
            return;
        if (attacker.Identity == null)
            return;

        if (packet != null && hitCount > 0)
        {
            SchedulePacketHits(attacker, target, packet);
            return;
        }

        if (miss && hitCount <= 1)
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
            NextHitIndex = 0,
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

    void SchedulePacketHits(Entity attacker, Entity target, AttackDto packet)
    {
        if (attacker == null || attacker.Identity == null || packet == null)
            return;

        int hitCount = AttackHitPresentation.CountHits(packet);
        if (hitCount <= 0)
        {
            UnityEngine.Debug.LogError(
                "[Attack] packet has no hit rows attackerId=" + attacker.Identity.Id);
            return;
        }

        if (IsBowWeapon(attacker))
        {
            DropRemoteMeleeHit(attacker.Identity.Id);
            Hit first = AttackHitPresentation.ResolveHit(packet, 0);
            if (first != null && first.hasSoulshot())
                attacker.IsSoulshotCharged = true;
            return;
        }

        FlushRemoteMeleeHit(attacker);

        float cycleSec = AttackTimingHelper.ResolveAttackCycleMs(attacker, null) / 1000f;
        if (cycleSec < 0.05f)
        {
            UnityEngine.Debug.LogError(
                "[Attack] attack cycle unresolved attackerId=" + attacker.Identity.Id +
                " cycleSec=" + cycleSec.ToString("0.###"));
            return;
        }

        int attackerId = attacker.Identity.Id;
        float start = Time.time;
        if (CombatTrace.Enabled)
        {
            var rows = new System.Text.StringBuilder();
            for (int i = 0; i < hitCount; i++)
                rows.Append(i == 0 ? "" : ",").Append((cycleSec * AttackHitWireRows.HitCycleFraction(i, hitCount) * 1000f).ToString("F0"));
            CombatTrace.Write("hit.scheduled",
                "atk=" + attackerId + " tgt=" + packet.TargetId + " hits=" + hitCount +
                " cycleMs=" + (cycleSec * 1000f).ToString("F0") + " showAtMs=" + rows);
        }
        _remoteHits[attackerId] = new RemoteMeleeHit
        {
            AttackerId = attackerId,
            Attacker = attacker,
            Target = target,
            NextHitIndex = 0,
            HitCount = hitCount,
            CycleStart = start,
            CycleSec = cycleSec,
            Packet = packet,
            WaitForShot = false,
            FireAt = start + cycleSec * AttackHitWireRows.HitCycleFraction(0, hitCount)
        };
    }
}
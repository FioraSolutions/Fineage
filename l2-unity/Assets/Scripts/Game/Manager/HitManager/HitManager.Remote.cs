using System;
using System.Collections.Generic;
using UnityEngine;

public partial class HitManager
{
    sealed class RemoteMeleeHit
    {
        public int AttackerId;
        public Entity Attacker;
        public Entity Target;
        public bool Soulshot;
        public bool Fired;
        public float FireAt;
        public bool WaitForShot;
        public bool Miss;
        public AttackDto Packet;
        public AnimationEventsBase Events;
        public Action<string> OnShot;
    }

    public void RememberLocalAttack(AttackDto dto)
    {
        _localAttack = dto;
        _localAttackToken++;
    }

    public void FireRememberedExtraHits(Entity attacker)
    {
        if (attacker == null || _localAttack == null || _localAttackFiredToken == _localAttackToken)
            return;

        _localAttackFiredToken = _localAttackToken;
        FireExtraHits(attacker, _localAttack, _localAttack.TargetId);
    }

    /// <summary>
    /// Remote melee (Monster / CharInfo). Local PlayerEntity stays on AttackShot + sword collision.
    /// Impact waits for clip AttackShot; timer is only a late fallback.
    /// Bow: ignore AttackShot — the arrow has not left yet. Impact is ArrowStick Hit Time.
    /// </summary>
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

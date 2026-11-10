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

    public void FlushRemoteMeleeHit(Entity attacker)
    {
        if (attacker == null || attacker.Identity == null)
            return;
        TryFireRemoteMeleeHit(attacker.Identity.Id);
    }

    void TickRemoteMeleeHits()
    {
        if (_remoteHits.Count == 0)
            return;

        _remoteHitScratch.Clear();
        foreach (KeyValuePair<int, RemoteMeleeHit> kv in _remoteHits)
        {
            RemoteMeleeHit pending = kv.Value;
            if (pending == null || pending.Fired)
            {
                _remoteHitScratch.Add(kv.Key);
                continue;
            }

            if (pending.Attacker == null || pending.Attacker.IsDead())
            {
                _remoteHitScratch.Add(kv.Key);
                continue;
            }

            if (Time.time >= pending.FireAt)
                _remoteHitScratch.Add(kv.Key);
        }

        for (int i = 0; i < _remoteHitScratch.Count; i++)
        {
            int id = _remoteHitScratch[i];
            RemoteMeleeHit pending;
            if (!_remoteHits.TryGetValue(id, out pending) || pending == null)
                continue;
            if (pending.Fired || pending.Attacker == null || pending.Attacker.IsDead())
            {
                DropRemoteMeleeHit(id);
                continue;
            }

            if (pending.WaitForShot)
            {
                DropRemoteMeleeHit(id);
                continue;
            }

            TryFireRemoteMeleeHit(id);
        }
    }

    void TryFireRemoteMeleeHit(int attackerId)
    {
        RemoteMeleeHit pending;
        if (!_remoteHits.TryGetValue(attackerId, out pending) || pending == null || pending.Fired)
            return;

        pending.Fired = true;
        UnbindRemoteMeleeHit(pending);
        _remoteHits.Remove(attackerId);

        Entity attacker = pending.Attacker;
        Entity target = pending.Target;
        if (attacker == null)
            return;
        if (target == null && (pending.Packet == null || pending.Packet.ArrHit == null || pending.Packet.ArrHit.Length == 0))
            return;

        if (target != null && !pending.Miss)
        {
            attacker.IsSoulshotCharged = pending.Soulshot;
            Vector3 hitPoint = ResolveTargetImpactCenter(target);
            HandleHitCollider(attacker, attacker.transform, target, hitPoint, Vector3.zero);
        }

        int primaryId = target != null && target.Identity != null ? target.Identity.Id : 0;
        FireExtraHits(attacker, pending.Packet, primaryId);
    }

    void FireExtraHits(Entity attacker, AttackDto dto, int primaryTargetId)
    {
        if (attacker == null || dto == null || dto.ArrHit == null)
            return;

        bool savedCharge = attacker.IsSoulshotCharged;
        for (int i = 0; i < dto.ArrHit.Length; i++)
        {
            Hit hit = dto.ArrHit[i];
            if (hit == null || hit.isMiss() || hit.TargetId == primaryTargetId)
                continue;

            Entity extra = ResolveEntityFromWorld(hit.TargetId);
            if (extra == null &&
                DomainPacketServices.LocalPlayer != null &&
                DomainPacketServices.LocalPlayer.Identity != null &&
                DomainPacketServices.LocalPlayer.Identity.Id == hit.TargetId)
                extra = DomainPacketServices.LocalPlayer;
            if (extra == null || extra == attacker)
                continue;

            attacker.IsSoulshotCharged = hit.hasSoulshot();
            Vector3 extraPoint = ResolveTargetImpactCenter(extra);
            HandleHitCollider(attacker, attacker.transform, extra, extraPoint, Vector3.zero);
        }

        attacker.IsSoulshotCharged = savedCharge;
    }

    void DropRemoteMeleeHit(int attackerId)
    {
        RemoteMeleeHit pending;
        if (!_remoteHits.TryGetValue(attackerId, out pending))
            return;
        UnbindRemoteMeleeHit(pending);
        _remoteHits.Remove(attackerId);
    }

    void ClearRemoteMeleeHits()
    {
        _remoteHitScratch.Clear();
        foreach (KeyValuePair<int, RemoteMeleeHit> kv in _remoteHits)
            _remoteHitScratch.Add(kv.Key);
        for (int i = 0; i < _remoteHitScratch.Count; i++)
            DropRemoteMeleeHit(_remoteHitScratch[i]);
        _remoteHitScratch.Clear();
    }

    static void UnbindRemoteMeleeHit(RemoteMeleeHit pending)
    {
        if (pending == null || pending.Events == null)
            return;
        if (pending.OnShot != null)
            pending.Events.OnAnimationAttackShot -= pending.OnShot;
        pending.Events = null;
        pending.OnShot = null;
    }

    static bool IsBowWeapon(Entity attacker)
    {
        if (attacker == null)
            return false;

        UserEntity user = attacker as UserEntity;
        if (user != null)
            return string.Equals(user.WeaponAnim, "bow", StringComparison.OrdinalIgnoreCase);

        return attacker.Gear != null &&
               !string.IsNullOrEmpty(attacker.Gear.WeaponAnim) &&
               string.Equals(attacker.Gear.WeaponAnim, "bow", StringComparison.OrdinalIgnoreCase);
    }

    static float ResolveRemoteHitDelay(Entity attacker)
    {
        float pAtk = 333f;
        if (attacker != null && attacker.Stats != null)
        {
            if (attacker.Stats.PAtkRealSpeed > 1f)
                pAtk = attacker.Stats.PAtkRealSpeed;
            else if (attacker.Stats.PAtkSpd > 1)
                pAtk = attacker.Stats.PAtkSpd;
            else if (attacker.Stats.BasePAtkSpeed > 1f)
                pAtk = attacker.Stats.BasePAtkSpeed;
        }

        float cycleSec = TimeUtils.ConvertMsToSec(CalcBaseParam.CalculateTimeL2j(pAtk));
        return Mathf.Clamp(cycleSec * 0.75f, 0.12f, 1.5f);
    }
}

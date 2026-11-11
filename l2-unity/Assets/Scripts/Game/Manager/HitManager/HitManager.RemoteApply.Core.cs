using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class HitManager
{
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
}

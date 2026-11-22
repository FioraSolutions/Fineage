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
            if (pending == null)
            {
                _remoteHitScratch.Add(kv.Key);
                continue;
            }

            if (pending.Attacker == null || pending.Attacker.IsDead())
            {
                _remoteHitScratch.Add(kv.Key);
                continue;
            }

            if (pending.WaitForShot)
                continue;

            if (Time.time >= pending.FireAt)
                _remoteHitScratch.Add(kv.Key);
        }

        for (int i = 0; i < _remoteHitScratch.Count; i++)
        {
            int id = _remoteHitScratch[i];
            RemoteMeleeHit pending;
            if (!_remoteHits.TryGetValue(id, out pending) || pending == null)
                continue;
            if (pending.Attacker == null || pending.Attacker.IsDead())
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
        if (!_remoteHits.TryGetValue(attackerId, out pending) || pending == null)
            return;

        Entity attacker = pending.Attacker;
        if (attacker == null)
        {
            DropRemoteMeleeHit(attackerId);
            return;
        }

        int hitCount = AttackHitPresentation.CountHits(pending.Packet);
        if (hitCount == 0)
        {
            DropRemoteMeleeHit(attackerId);
            return;
        }

        if (pending.WaitForShot)
        {
            if (!PresentOnePacketHit(pending))
            {
                DropRemoteMeleeHit(attackerId);
                return;
            }

            if (pending.NextHitIndex >= hitCount)
            {
                UnbindRemoteMeleeHit(pending);
                _remoteHits.Remove(attackerId);
            }

            return;
        }

        while (pending.NextHitIndex < hitCount)
        {
            if (!PresentOnePacketHit(pending))
                break;
        }

        UnbindRemoteMeleeHit(pending);
        _remoteHits.Remove(attackerId);
    }

    bool PresentOnePacketHit(RemoteMeleeHit pending)
    {
        Entity attacker = pending.Attacker;
        AttackDto packet = pending.Packet;
        int hitIndex = pending.NextHitIndex;
        pending.NextHitIndex++;

        Entity target = ResolveAttackHitTarget(pending.Target, AttackHitPresentation.ResolveHit(packet, hitIndex));
        return TryPresentMeleeHitFromAttackPacket(attacker, target, packet, hitIndex, playColliderImpactFx: true);
    }
}
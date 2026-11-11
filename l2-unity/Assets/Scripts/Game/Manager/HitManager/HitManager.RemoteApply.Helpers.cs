using System;
using System.Collections.Generic;
using UnityEngine;

public partial class HitManager
{
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

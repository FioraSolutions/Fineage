using System;

using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class SwordCollisionManager
{
    public void RegisterSword(Transform swordBase, Transform swordTip, Transform target, float extraRange)
    {
        // Debug.LogWarning($"{TIMER_LOG} RegisterSword without entity ids is deprecated. Use RegisterSwordByEntityId.");
        RegisterSwordByEntityId(0, 0, swordBase, swordTip, target, extraRange);
    }

    public void RegisterSwordByEntityId(int attackerEntityId, int targetEntityId, Transform swordBase, Transform swordTip, Transform target, float extraRange)
    {
        if (swordBase == null || swordTip == null) return;
        if (target == null)
        {
            // Debug.LogWarning("SwordCollisionService: RegisterSword target is null");
            return;
        }

        // --- OLD wall-clock melee Hit (DurationMs * HitFraction). Disabled — use AttackShot. ---
        // if (_activeTimedHits.Exists(s => s.Tracked.basePt == swordBase))
        // {
        //     ResetHitRegistry(swordBase);
        //     RecreateTimedHit(attackerEntityId, targetEntityId, swordBase, swordTip, target, extraRange);
        //     return;
        // }
        //
        // ResetHitRegistry(swordBase);
        // CreateTimedHit(attackerEntityId, targetEntityId, swordBase, swordTip, target, extraRange);

        // Debug.LogWarning(
            // $"{CHAIN_LOG} RegisterSwordByEntityId ignored (melee Hit is AttackShot-only). " +
            // $"attackerId={attackerEntityId} sword={swordBase.name}");
    }

    public void ResetHitRegistry(Transform swordBase)
    {
        if (!_hitRegistry.ContainsKey(swordBase)) _hitRegistry[swordBase] = new HashSet<int>();
        _hitRegistry[swordBase].Clear();
    }

    public void UnregisterSword(Transform swordBase)
    {
        int idx = _activeTimedHits.FindIndex(s => s.Tracked.basePt == swordBase);
        if (idx >= 0)
        {
            _activeTimedHits.RemoveAt(idx);
            _hitRegistry.Remove(swordBase);
        }
    }

}

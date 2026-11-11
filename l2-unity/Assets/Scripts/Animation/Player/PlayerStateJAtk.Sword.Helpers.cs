using UnityEngine;

public partial class PlayerStateJAtk
{
    private void OnSwordColliderHit(Transform attacker, Transform target, Vector3 hitPointCollider, Vector3 hitDirection)
    {
        if (_startTime < 0f || _isSwordHitLogged || DomainPacketServices.LocalPlayer == null)
        {
            return;
        }

        _isSwordHitLogged = true;
        float elapsedMs = TimeUtils.ConvertSecToMs(Time.time - _startTime);
        float expectedMs = TimeUtils.ConvertSecToMs(_endTime);
        float wallNorm = _endTime > 0f ? (Time.time - _startTime) / _endTime : 0f;
        bool sameRoot = attacker != null &&
                        DomainPacketServices.LocalPlayer.transform != null &&
                        attacker.root == DomainPacketServices.LocalPlayer.transform.root;

        string attackerName = attacker != null ? attacker.name : "null";
        string targetName = target != null ? target.name : "null";
        // Debug.Log(
            // $"[ATK_HIT_CHAIN] 5.JAtkSawHit anim={parameterName} frame={Time.frameCount} t={Time.time:F3} " +
            // $"wallElapsedMs={elapsedMs:F1}/{expectedMs:F1} wallNorm={wallNorm:F3} " +
            // $"animNorm={_lastAnimNormalized:F3} sameRoot={sameRoot} " +
            // $"attacker={attackerName} target={targetName} hitPoint={hitPointCollider}");
        // Debug.Log(
            // $"{JATK_TIMING_LOG} SwordHit anim={parameterName} elapsedMs={elapsedMs:F1} " +
            // $"expectedMs={expectedMs:F1} wallNorm={wallNorm:F3} animNorm={_lastAnimNormalized:F3} " +
            // $"sameRoot={sameRoot} attacker={attackerName} target={targetName} hitPoint={hitPointCollider}");
        // UnityChainLog.OnJAtkHit(DomainPacketServices.LocalPlayer, parameterName, expectedMs, elapsedMs, _lastAnimNormalized, hitPointCollider);

        if (IsDual(parameterName))
        {
            // Debug.Log(
                // $"{JATK_EARLY_LOG} FirstSwordHit DUAL anim={parameterName} " +
                // $"wallElapsedMs={elapsedMs:F1}/{expectedMs:F1} animNorm={_lastAnimNormalized:F3} " +
                // $"target={targetName}");
        }
    }

    private float GetFullAttackCycleMs(int objectId, string animName)
    {
        Entity entity = null;
        World world = WorldApplyPacketServices.GameWorld;
        if (world != null && objectId > 0)
            entity = world.GetEntityNoLockSync(objectId);
        if (entity == null)
            entity = DomainPacketServices.LocalPlayer;

        return AttackTimingHelper.ResolveAttackCycleMs(entity, animName);
    }

    private static int ResolvePlayerEntityId()
    {
        if (DomainPacketServices.LocalPlayer == null || DomainPacketServices.LocalPlayer.Identity == null)
        {
            return 0;
        }

        return DomainPacketServices.LocalPlayer.Identity.Id;
    }
}

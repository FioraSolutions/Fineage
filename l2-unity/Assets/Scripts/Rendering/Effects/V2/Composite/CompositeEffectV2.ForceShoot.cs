using UnityEngine;

public partial class CompositeEffectV2 : CompositePrefabEffect
{
    /// <summary>
    /// SFX RELEASE / wall-clock backup: spawn+launch flying orb parts even if
    /// AnimationShoot never arrived.
    /// </summary>
    public void ForceShootNow(string channel)
    {
        if (!isActiveAndEnabled)
        {
            Debug.LogWarning(
                "[ShotOrb] ForceShootNow inactive go=" + name + " channel=" + channel);
            return;
        }

        RefreshV2Context();
        Debug.Log(
            "[ShotOrb] ForceShootNow channel=" + channel +
            " go=" + name +
            " pendingShoot=" + _pendingAnimationShoot.Count +
            " delayed=" + _pendingDelayed.Count +
            " spawned=" + _spawned.Count +
            " launched=" + _launchedProjectiles.Count +
            " parts=" + (_v2Parts != null ? _v2Parts.Length : 0) +
            " cast=" + (_castData != null) +
            " shoot=" + (_castData != null ? _castData.serverTimeToShoot.ToString("0.###") : "-") +
            " target=" + (_context != null && _context.TargetTransform != null
                ? _context.TargetTransform.name
                : "null"));
        ProcessV2Shoot(channel);

        // Delayed queue holds OnServerShootTime / OnHitTime / AnimationShoot backups —
        // flush due (or overdue) parts of any type so Aura Flare / Fire Vortex _ta also spawn.
        float now = Time.time;
        for (int i = _pendingDelayed.Count - 1; i >= 0; i--)
        {
            PendingV2Part pending = _pendingDelayed[i];
            if (pending == null || pending.Part == null)
            {
                _pendingDelayed.RemoveAt(i);
                continue;
            }

            if (now + 0.05f < pending.SpawnAtTime)
            {
                continue;
            }

            SpawnV2Part(pending.Part);
            _pendingDelayed.RemoveAt(i);
        }

        // Last resort: spawn every unlaunched shot part now.
        if (_v2Parts != null && HasUnlaunchedShotProjectile())
        {
            for (int i = 0; i < _v2Parts.Length; i++)
            {
                ShotProjectilePart shot = _v2Parts[i] as ShotProjectilePart;
                if (shot == null || !shot.IsLaunchedProjectile || _launchedProjectiles.Contains(shot))
                {
                    continue;
                }

                if (!_spawned.ContainsKey(shot))
                {
                    SpawnV2Part(shot);
                }

                if (_spawned.TryGetValue(shot, out BaseEffect instance))
                {
                    TryLaunchImmediate(shot, instance);
                    TryLaunchOnShoot(shot, instance);
                }
            }
        }
    }
}

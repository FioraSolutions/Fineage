using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using VContainer;

public partial class ProjectileManager
{
    private bool UpdateProjectile(ProjectileData projectile)
    {
        if (!projectile.isActive) return false;
        if (projectile.transform == null)
        {
            // Debug.LogWarning(
                // $"[ProjectileUpdate] destroyed transform id={projectile.id} prefabNull={(projectile.prefab == null)} " +
                // $"targetNull={(projectile.targetTransform == null)} impactType={projectile.impactType} now={Time.time:F3}");
            return false;
        }

       
        CalcNewTargetPosition(projectile);

        float elapsed = Time.time - projectile.startTime;
        float timeProgress = elapsed / Mathf.Max(projectile.flytime, 0.01f);

        // NArrow + skill: Accel=dir*3000 from rest → path (t/T)² (not linear).
        float journeyProgress = ProjectileFlightTimeCalculator.CalculateL2AccelJourneyProgress(
            elapsed, projectile.flytime);

        Vector3 currentPosition = GetCurrentPosition(projectile, journeyProgress);
  
        RefreshHitPosition(projectile);

        // Hit Time = end of scheduled flytime (wall clock), not collider.
        if (timeProgress >= 1f)
        {
            // ArrowStick / EffectOnly: Hit Time = end of flytime. No NPC collider dependency.
            SetPosition(projectile, projectile.targetPosition);
            projectile.lastPosition = currentPosition;
            if (projectile.impactType == ProjectileImpactType.EffectOnly)
            {
                // NSkillProjectile hands its explosion the projectile collision
                // point. Keep the orb endpoint and target effect on that same
                // point instead of resolving a second, potentially distant bone.
                RefreshHitPosition(projectile);
            }
            else
            {
                RefreshHitPositionFromAnchor(projectile);
            }
            projectile.hitPointCollider = projectile.hitPoint;
            LogProjectileImpactTiming(projectile);

            // skillsoundgrp *_explotion when the orb reaches the target (not MagicSkillLaunched/release).
            try
            {
                SkillCastSound.OnProjectileImpact(projectile.attackerEntityId, projectile.hitPoint);
            }
            catch (Exception) { }

            if (projectile.impactType == ProjectileImpactType.EffectOnly)
            {
                bool hasSubscribers = OnHitEffectProjectile != null;
                float gs = projectile.castStartTimeSnapshot > 0f ? Time.time - projectile.castStartTimeSnapshot : -1f;
                // Debug.Log(
                    // $"{PROJECTILE_TIMER_LOG} EffectOnlyTimeHit id={projectile.id} " +
                    // $"elapsedSec={(Time.time - projectile.startTime):F3} flyTimeSec={projectile.flytime:F3} " +
                    // $"eventSubscribers={hasSubscribers} hitPoint={projectile.hitPoint}");
                // Debug.Log(
                    // $"{MagicProjectileSyncTag} EFFECT_ONLY_HIT_FIRE id={projectile.id} " +
                    // $"globalSinceCast={gs:F3}s serverHit={projectile.castServerHitSnapshot:F3}s " +
                    // $"deltaToServerHit={(gs >= 0f ? gs - projectile.castServerHitSnapshot : -1f):F3}s " +
                    // $"elapsedSinceLaunch={(Time.time - projectile.startTime):F3}s");

                // The client removes NSkillProjectile before spawning the target
                // effect. Destroy() is deferred until the end of Unity's frame,
                // so hide and clear the orb now while keeping its object alive
                // for hit ownership/direction resolution in the subscribers.
                CompositeProjectileLaunchHelper.SetPartVisualsVisible(
                    projectile.prefab != null ? projectile.prefab.transform : null,
                    false);

                OnHitEffectProjectile?.Invoke(
                    projectile.prefab,
                    projectile.targetTransform,
                    projectile.hitPoint,
                    projectile.hitDirection,
                    projectile.attackerEntityId);
            }
            else
            {
                // Bow: Soulshot / shoot impact on Hit Time, then stick arrow.
                // Debug.Log(
                    // $"{PROJECTILE_TIMER_LOG} ArrowStickTimeHit id={projectile.id} " +
                    // $"elapsedSec={elapsed:F3} flyTimeSec={projectile.flytime:F3} " +
                    // $"hitPoint={projectile.hitPoint}");

                OnHitMonster?.Invoke(
                    projectile.prefab,
                    projectile.targetTransform,
                    projectile.hitPoint,
                    projectile.hitDirection,
                    projectile.attackerEntityId);
            }
            return false;
        }

        SetPosition(projectile, currentPosition);

        if (projectile.targetTransform != null)
        {
            Vector3 dir = projectile.targetPosition - projectile.transform.position;
            Quaternion rotation = Quaternion.LookRotation(dir);
            rotation *= Quaternion.Euler(0, 90, 0);
            SetRotation(projectile , rotation);
        }



        projectile.lastPosition = currentPosition;
        return true;
    }
}

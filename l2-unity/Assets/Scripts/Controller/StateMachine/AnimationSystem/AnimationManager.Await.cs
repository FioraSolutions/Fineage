using System;
using System.Threading.Tasks;
using UnityEngine;

public partial class AnimationManager
{
    public async Task<bool> AsyncPlayAnimationTrigger(int objectId, string triggerName)
    {
        float startedAt = Time.time;
        string expectedFinishName = GetFinalNameAnim(objectId, triggerName);

        if (_tcsMap.TryGetValue(objectId, out var oldTcs))
        {
            oldTcs.TrySetResult(false);
        }

        var tcs = new TaskCompletionSource<bool>();
        _tcsMap[objectId] = tcs;
        _expectedFinishNameByObjectId[objectId] = expectedFinishName;

        AnimationModel model = GetModel(objectId);

        EnsureAwaitSubscribed(objectId, model);

        // Debug.Log($"[AnimAwait] START objectId={objectId} trigger='{triggerName}' mode=default now={Time.time:F3}");
        PlayerAnimationTrigger(objectId, triggerName);

        bool completed = await tcs.Task;
        FinishOverrideAwait(objectId, tcs, expectedFinishName, completed);
        // Debug.Log(
            // $"[AnimAwait] {(completed ? "END" : "CANCELLED")} objectId={objectId} trigger='{triggerName}' " +
            // $"mode=default elapsed={Time.time - startedAt:F3}s now={Time.time:F3}");
        return completed;
    }

    public async Task<bool> AsyncPlayAnimationRaceOverrides(int objectId, string triggerName , string overrideAnimationName)
    {
        float startedAt = Time.time;
        string expectedFinishName = SkillAnimationDatabase.ResolveOverrideCompletionEventName(triggerName);
        if (_tcsMap.TryGetValue(objectId, out var oldTcs))
        {
            // Debug.Log(
                // $"[AnimAwait] SUPERSEDE objectId={objectId} newTrigger='{triggerName}' " +
                // $"override='{overrideAnimationName}' (prev await → false, no WAIT_RETURN)");
            oldTcs.TrySetResult(false);
        }

        var tcs = new TaskCompletionSource<bool>();
        _tcsMap[objectId] = tcs;
        _expectedFinishNameByObjectId[objectId] = expectedFinishName;
        AnimationModel model = GetModel(objectId);
        BaseAnimationController castBody = model.GetController() as BaseAnimationController;
        bool playedOnGraph = false;
        if (castBody != null)
        {
            using (MagicSkillUseTrace.Measure("anim.playable"))
                playedOnGraph = castBody.TryPlayCastPhase(overrideAnimationName, triggerName);
        }

        if (SkillAnimationRunner.Trace)
        {
            BaseAnimationController body = castBody;
            Debug.Log(
                "[SUMMON_ANIM] RaceOverride trigger=" + triggerName +
                " override=" + overrideAnimationName +
                " playedOnGraph=" + playedOnGraph +
                " controller=" + (body != null ? body.GetType().Name : "null") +
                " animator=" + (body != null ? body.GetAnimatorName() : "none") +
                " expectedFinish=" + expectedFinishName);
        }

        if (!playedOnGraph)
        {
            //Root Animator FDarkElf
            string animName = SkillAnimationDatabase.GetAnimationClipName(triggerName, "FDarkElf");
            using (MagicSkillUseTrace.Measure("anim.replaceClip"))
                model.GetController().ReplaceAnimClip(animName, overrideAnimationName);
        }

        EnsureAwaitSubscribed(objectId, model);

        // Debug.Log(
            // $"[AnimAwait] START objectId={objectId} trigger='{triggerName}' override='{overrideAnimationName}' " +
            // $"expectedFinish='{expectedFinishName}' mode=override now={Time.time:F3}");

        _ = WarnIfAwaitStuck(objectId, triggerName, expectedFinishName, startedAt);

        if (!playedOnGraph)
        {
            using (MagicSkillUseTrace.Measure("anim.trigger"))
                PlayerAnimationTrigger(objectId, triggerName, false);
        }

        bool completed = await tcs.Task;
        FinishOverrideAwait(objectId, tcs, expectedFinishName, completed);
        // Debug.Log(
            // $"[AnimAwait] {(completed ? "END" : "CANCELLED")} objectId={objectId} trigger='{triggerName}' " +
            // $"override='{overrideAnimationName}' mode=override elapsed={Time.time - startedAt:F3}s now={Time.time:F3}");
        return completed;
    }

    public async Task<bool> AsyncAwaitOverrideAnimationFinish(int objectId, string expectedFinishNameOrTrigger)
    {
        float startedAt = Time.time;
        string sanitizedExpected = SkillAnimationDatabase.ResolveOverrideCompletionEventName(
            string.IsNullOrWhiteSpace(expectedFinishNameOrTrigger)
                ? string.Empty
                : expectedFinishNameOrTrigger.Trim());

        if (_tcsMap.TryGetValue(objectId, out TaskCompletionSource<bool> oldTcs))
        {
            oldTcs.TrySetResult(false);
        }

        var tcs = new TaskCompletionSource<bool>();
        _tcsMap[objectId] = tcs;
        _expectedFinishNameByObjectId[objectId] = sanitizedExpected;

        AnimationModel model = GetModel(objectId);
        EnsureAwaitSubscribed(objectId, model);

        // Debug.Log(
            // $"[AnimAwait] START objectId={objectId} expectedFinish='{sanitizedExpected}' mode=await_finish now={Time.time:F3}");

        _ = WarnIfAwaitStuck(objectId, "await_finish", sanitizedExpected, startedAt);

        bool completed = await tcs.Task;
        FinishOverrideAwait(objectId, tcs, sanitizedExpected, completed);

        // Debug.Log(
            // $"[AnimAwait] {(completed ? "END" : "CANCELLED")} objectId={objectId} expectedFinish='{sanitizedExpected}' " +
            // $"mode=await_finish elapsed={Time.time - startedAt:F3}s now={Time.time:F3}");
        return completed;
    }

    /// <summary>
    /// After await: clear maps only if this TCS still owns the slot / expected name.
    /// Superseded awaits must not wipe the newer cast's expectedFinish.
    /// </summary>
    private void FinishOverrideAwait(
        int objectId,
        TaskCompletionSource<bool> tcs,
        string expectedFinishName,
        bool completed)
    {
        if (_tcsMap.TryGetValue(objectId, out TaskCompletionSource<bool> live) &&
            ReferenceEquals(live, tcs))
        {
            _tcsMap.Remove(objectId);
        }

        if (!completed)
        {
            return;
        }

        if (_expectedFinishNameByObjectId.TryGetValue(objectId, out string exp) &&
            string.Equals(exp, expectedFinishName, StringComparison.Ordinal))
        {
            _expectedFinishNameByObjectId.Remove(objectId);
        }
    }

    private async Task WarnIfAwaitStuck(int objectId, string triggerName, string expectedFinishName, float startedAt)
    {
        await Task.Delay(OVERRIDE_AWAIT_WARN_TIMEOUT_MS);

        if (_expectedFinishNameByObjectId.TryGetValue(objectId, out string currentExpected) &&
            string.Equals(currentExpected, expectedFinishName, StringComparison.Ordinal))
        {
            // Debug.LogWarning(
                // $"[AnimAwait] STUCK? objectId={objectId} trigger='{triggerName}' expectedFinish='{expectedFinishName}' " +
                // $"elapsed={Time.time - startedAt:F3}s action=waiting_for_OnAnimationFinished");
        }
    }


    private void EnsureAwaitSubscribed(int objectId, AnimationModel model)
    {
        if (_awaitSubscribedObjectIds.Contains(objectId))
        {
            return;
        }

        model.SubscribeToInternalEvents();
        model.OnAnimationFinishedWithId += OnAnimationFinished;
        _awaitSubscribedObjectIds.Add(objectId);
    }
    public void OnAnimationFinished(string name, int objectId)
    {
        TryCompleteAwaitFromPhaseFinish(objectId, name, "clip_complete");
    }

    /// <summary>
    /// SMB-driven phase end (Cast/MagicShot / SpAtk). Completes runner await
    /// without relying on clip OnAnimationComplete.
    /// </summary>
    public void NotifyMagicPhaseFinished(int objectId, string phaseName)
    {
        MagicGaitTrace.EventId(objectId, "NotifyMagicPhaseFinished", "phase=" + phaseName);
        TryCompleteAwaitFromPhaseFinish(objectId, phaseName, "smb_phase");
    }

    private bool TryCompleteAwaitFromPhaseFinish(int objectId, string name, string source)
    {
        // Debug.Log(
            // $"[AnimFinishedEvent] objectId={objectId} finishedName='{name}' source={source} now={Time.time:F3}");

        // SpAtk / jatk / Cast / MagicShot awaits are completed by SMB NotifyMagicPhaseFinished.
        // Early clip OnAnimationComplete must not finish the skill runner mid-dual.
        if (string.Equals(source, "clip_complete", StringComparison.Ordinal) &&
            IsSmbOwnedCombatOrMagicPhase(name))
        {
            // Debug.Log(
                // $"[AnimFinishedEvent] IGNORE clip_complete objectId={objectId} finishedName='{name}' " +
                // $"(SMB owns phase finish)");
            return false;
        }

        if (_expectedFinishNameByObjectId.TryGetValue(objectId, out string expectedName))
        {
            if (string.Equals(source, "clip_complete", StringComparison.Ordinal) &&
                IsSmbOwnedCombatOrMagicPhase(expectedName))
            {
                // Debug.Log(
                    // $"[AnimFinishedEvent] IGNORE clip_complete objectId={objectId} " +
                    // $"finishedName='{name}' expected='{expectedName}' (SMB owns phase finish)");
                return false;
            }

            if (!string.Equals(name, expectedName, StringComparison.Ordinal) &&
                !string.Equals(SkillAnimationDatabase.ResolveOverrideCompletionEventName(name), expectedName, StringComparison.Ordinal))
            {
                // Debug.Log(
                    // $"[AnimFinishedEvent] IGNORE objectId={objectId} finishedName='{name}' expected='{expectedName}' " +
                    // $"source={source} now={Time.time:F3}");
                return false;
            }
        }

        if (_tcsMap.TryGetValue(objectId, out var tcs))
        {
            _tcsMap.Remove(objectId);
            tcs.TrySetResult(true);
            return true;
        }

        return false;
    }

    private static bool IsSmbOwnedCombatOrMagicPhase(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }

        return name.StartsWith("SpAtk", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith("jatk", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith("CastMid", StringComparison.Ordinal)
            || name.StartsWith("CastEnd", StringComparison.Ordinal)
            || name.StartsWith("MagicShot", StringComparison.Ordinal);
    }

    public void PlayerAnimationTrigger(int objectId , string animationName , bool useFinalName = true)
    {
        IAnimationController controller = GetRegisteredController(objectId);
        if (controller == null)
        {
            if (SkillAnimationRunner.Trace)
                Debug.Log("[SUMMON_ANIM] trigger skip controller=null name=" + animationName + " objectId=" + objectId);
            return;
        }

        if (SkillAnimationRunner.Trace)
            Debug.Log("[SUMMON_ANIM] trigger name=" + animationName + " useFinal=" + useFinalName + " objectId=" + objectId);

        if(useFinalName) animationName = GetFinalNameAnim(objectId, animationName);

        Entity entity = GetEntity(objectId);
        DesableLastPlayerAnimationElseTrue(objectId , controller);

        bool isMagicShotTrigger = IsMagicShotTrigger(animationName);
        if (isMagicShotTrigger)
        {
            MagicCastData castData = entity != null ? entity.GetMagicCastData() : null;
            if (!MagicShotCoordinator.TryStartShot(objectId, castData, SHOT_SOURCE_RUNNER, out string coordinatorMessage))
            {
                // Debug.Log($"{coordinatorMessage} trigger='{animationName}' action=skip");
                return;
            }

            // Debug.Log($"{coordinatorMessage} trigger='{animationName}' action=run");
        }

        if (IsMagicCastTrigger(animationName))
        {
            // Prevent stale cast triggers from keeping previous cast states alive.
            controller.ResetAnimationTrigger(CAST_TRIGGER_MID);
            controller.ResetAnimationTrigger(CAST_TRIGGER_END);
            controller.ResetAnimationTrigger(CAST_TRIGGER_SHOT);
            controller.ResetAnimationTrigger(CAST_TRIGGER_END_2P);
            controller.ResetAnimationTrigger(CAST_TRIGGER_SHOT_2P);
        }

        // Skill runner / async path — same CrossFade for jatk / SpAtk as PlayAnimationTrigger.
        if (TryPlayCombatCrossFade(objectId, controller, entity, animationName))
        {
            return;
        }

        controller.ToggleAnimationTrigger(animationName);
        SetRecentName(objectId, animationName);

        // Debug.Log($"AnimationManager> start Async AnimationTrigger(  {entity} animation  {animationName}");

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (IsMagicShotTrigger(animationName) && entity != null)
        {
            MagicCastData cd = entity.GetMagicCastData();
            if (cd != null)
            {
                float globalSinceCast = Time.time - cd.StartTime;
                // Debug.Log(
                    // "[MAGIC_PROJECTILE_SYNC] AnimatorTriggerMagicShot " +
                    // $"globalSinceCast={globalSinceCast:F3}s serverShoot={cd.serverTimeToShoot:F3}s " +
                    // $"deltaToShoot={globalSinceCast - cd.serverTimeToShoot:F3}s trigger={animationName}");
            }
        }
#endif
    }

    private static void ResetMagicCastTriggers(IAnimationController controller)
    {
        if (controller == null)
        {
            return;
        }

        controller.ResetAnimationTrigger(CAST_TRIGGER_MID);
        controller.ResetAnimationTrigger(CAST_TRIGGER_END);
        controller.ResetAnimationTrigger(CAST_TRIGGER_SHOT);
        controller.ResetAnimationTrigger(CAST_TRIGGER_END_2P);
        controller.ResetAnimationTrigger(CAST_TRIGGER_SHOT_2P);
    }

    private static bool IsMagicCastTrigger(string animationName)
    {
        return animationName == CAST_TRIGGER_MID ||
               animationName == CAST_TRIGGER_END ||
               animationName == CAST_TRIGGER_SHOT ||
               animationName == CAST_TRIGGER_END_2P ||
               animationName == CAST_TRIGGER_SHOT_2P;
    }

    private static bool IsMagicShotTrigger(string animationName)
    {
        return animationName == CAST_TRIGGER_SHOT ||
               animationName == CAST_TRIGGER_SHOT_2P ||
               animationName.StartsWith(CAST_TRIGGER_SHOT, StringComparison.Ordinal);
    }
}

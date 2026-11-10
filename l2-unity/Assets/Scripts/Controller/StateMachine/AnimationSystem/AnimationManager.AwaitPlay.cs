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

}

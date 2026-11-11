using System;
using System.Threading.Tasks;
using UnityEngine;

public partial class AnimationManager
{
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

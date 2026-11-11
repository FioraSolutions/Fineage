using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CooldownAnimationService
{
    public IEnumerator CooldownCoroutine(VisualElement reuseElement, VisualElement rechargeMaskBg, float durationMs)
    {
        if (reuseElement == null)
            yield break;

        float duration = TimeUtils.ConvertMsToSec(durationMs);
        if (duration <= 0.05f)
            yield break;

        reuseElement.style.display = DisplayStyle.Flex;
        reuseElement.style.opacity = 1f;

        int cooltimeCount = IconPacketServices.Icons != null ? IconPacketServices.Icons.GetSizeOtherCoolTime() : 0;
        if (cooltimeCount <= 1)
        {
            // Fallback: darken the slot for the reuse window when cooltime frames failed to load.
            reuseElement.style.backgroundColor = new Color(0f, 0f, 0f, 0.65f);
            reuseElement.style.backgroundImage = null;
            yield return new WaitForSeconds(duration);
            CleanupCompletionFrame(reuseElement, rechargeMaskBg);
            yield break;
        }

        // Cap frame count so short CDs (e.g. Wind Strike 1s) still show a readable sweep.
        int frameCount = Mathf.Clamp(Mathf.RoundToInt(duration * 18f), 8, cooltimeCount);
        float timePerImage = duration / frameCount;
        float elapsed = 0f;

        for (int step = 0; step < frameCount; step++)
        {
            int frameId = 1 + (step * (cooltimeCount - 1)) / Mathf.Max(1, frameCount - 1);
            var icon = IconPacketServices.Icons.GetOtherIconByType(frameId, 0);
            if (icon != null)
                reuseElement.style.backgroundImage = icon;

            float waitTime = step < frameCount - 1 ? timePerImage : Mathf.Max(0f, duration - elapsed);
            if (waitTime > 0f)
                yield return new WaitForSeconds(waitTime);
            elapsed += waitTime;
        }

        float completionDuration = 0.35f;
        float fadeOutDuration = 0.25f;
        float elapsedCompletion = 0f;

        SetupCompletionFrame(reuseElement, rechargeMaskBg);

        while (elapsedCompletion < completionDuration)
        {
            float progress = elapsedCompletion / completionDuration;
            float maskOpacity = elapsedCompletion < fadeOutDuration
                ? 1f - (elapsedCompletion / fadeOutDuration)
                : 0f;
            float elementOpacity = 1f - progress;

            reuseElement.style.opacity = elementOpacity;
            if (rechargeMaskBg != null)
                rechargeMaskBg.style.opacity = maskOpacity;

            yield return null;
            elapsedCompletion += Time.deltaTime;
        }

        CleanupCompletionFrame(reuseElement, rechargeMaskBg);
    }
}

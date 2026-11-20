using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Shared UITK resource-bar width fill (HP/MP/CP/EXP). Prefer resolved background width;
/// pass <paramref name="fallbackWidth"/> when layout is not ready yet or Interlude UXML uses fixed chrome.
/// </summary>
public static class L2ResourceBarView
{
    public static void SetFill(
        VisualElement bar,
        VisualElement barBg,
        float ratio,
        float fallbackWidth = 0f)
    {
        if (bar == null || barBg == null)
            return;

        float clamped = Mathf.Clamp01(ratio);

        // Authored percentage fill: the TargetStatus plate stretches horizontally, so its gauge must be
        // expressed as a percentage of the container. Pixel math there drifts from the stretched
        // background on every resize. The flag is checked before layout resolves, so the first frame
        // is already correct instead of showing a stale 100% bar.
        if (UsesPercentFill(bar))
        {
            bar.style.width = Length.Percent(clamped * 100f);
            return;
        }

        float bgWidth = barBg.resolvedStyle.width;
        if (float.IsNaN(bgWidth) || bgWidth <= 1f || !float.IsFinite(bgWidth))
            bgWidth = fallbackWidth;

        if (bgWidth <= 0f || !float.IsFinite(bgWidth))
            return;

        float width = bgWidth * clamped;
        bar.style.width = width > 0f ? width : 0f;
    }

    /// <summary>
    /// Authored percentage fill: UXML marks the gauge's fill element with <c>l2-bar-percent-fill</c>
    /// when the bar lives in a container whose width is the plate width. Such a bar is never
    /// measured in pixels, so resizing the window cannot desynchronise fill from the background.
    /// </summary>
    const string PercentFillClass = "l2-bar-percent-fill";

    static bool UsesPercentFill(VisualElement bar)
    {
        for (VisualElement e = bar; e != null; e = e.parent)
        {
            if (e.ClassListContains(PercentFillClass))
                return true;
        }
        return false;
    }

    /// <summary>Interlude CharacterInfo uses authored chrome widths instead of resolvedStyle.</summary>
    public static void SetFillFixed(VisualElement bar, VisualElement barBg, float ratio, float fixedBgWidth)
    {
        if (bar == null || fixedBgWidth <= 0f)
            return;

        float clamped = Mathf.Clamp01(ratio);
        bar.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
        bar.style.width = fixedBgWidth * clamped;
        if (barBg != null)
        {
            barBg.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
            barBg.style.width = fixedBgWidth;
        }
    }
}

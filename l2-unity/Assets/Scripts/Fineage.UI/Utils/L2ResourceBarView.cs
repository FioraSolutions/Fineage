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

        float bgWidth = barBg.resolvedStyle.width;
        if (float.IsNaN(bgWidth) || bgWidth <= 1f || !float.IsFinite(bgWidth))
            bgWidth = fallbackWidth;

        if (bgWidth <= 0f || !float.IsFinite(bgWidth))
            return;

        float clamped = Mathf.Clamp01(ratio);
        float width = bgWidth * clamped;
        bar.style.width = width > 0f ? width : 0f;
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

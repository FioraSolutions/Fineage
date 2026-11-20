using UnityEngine;

/// <summary>
/// Shared screen-space layout for overhead nameplates and CreatureSay boxes.
/// Top → bottom stack: optional title, name line, cast bar below the name.
/// </summary>
public static partial class L2NameplateScreenLayout
{
    /// <summary>Matches default nameplate bitmap font line height at pixel scale 1.</summary>
    public const float FallbackLineHeightPx = 14f;

    /// <summary>
    /// Top of the name glyph row in top-down screen pixels (same convention as nameplate batch).
    /// </summary>
    public static float ComputeYNameTop(
        float screenHeightPx,
        float anchorScreenYFromBottom,
        float nameLineHeightPx)
    {
        return screenHeightPx - anchorScreenYFromBottom - nameLineHeightPx;
    }

    public static float ComputeYTitleTop(
        float yNameTop,
        float nameLineHeightPx,
        float titleGapPx)
    {
        return yNameTop - nameLineHeightPx - Mathf.Max(0f, titleGapPx);
    }

    /// <summary>
    /// Bottom of an overhead say rect (pivot at bottom center) using the same anchor as the name row.
    /// </summary>
    public static Vector2 ResolveSayBottomLeft(
        float screenHeightPx,
        float anchorScreenXFromBottom,
        float anchorScreenYFromBottom,
        float nameLineHeightPx)
    {
        float yNameTop = ComputeYNameTop(screenHeightPx, anchorScreenYFromBottom, nameLineHeightPx);
        return new Vector2(anchorScreenXFromBottom, screenHeightPx - yNameTop);
    }
}
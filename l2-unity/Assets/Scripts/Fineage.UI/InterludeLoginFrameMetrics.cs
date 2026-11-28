using UnityEngine;

/// <summary>
/// Geometry of the status / error line inside the login frame (NCAuthWnd).
/// The retail frame is 256x108 with Login/Exit at y=72..95 (docs/ui-original/Login.json).
/// Fineage adds one wrapped text line under the buttons and grows the frame downward to fit it.
/// </summary>
public static class InterludeLoginFrameMetrics
{
    public const float FrameWidth = 256f;
    public const float BaseHeight = 108f;
    public const float ButtonsBottom = 95f;
    public const float StatusTop = 99f;
    public const float StatusLeft = 14f;
    public const float StatusWidth = 228f;
    public const float StatusBottomPadding = 9f;
    public const int SliceBorder = 5;

    /// <summary>Frame height for a status text of the given laid-out height. Zero or less means no message.</summary>
    public static float FrameHeight(float statusTextHeight)
    {
        if (float.IsNaN(statusTextHeight) || statusTextHeight <= 0f)
            return BaseHeight;
        float needed = StatusTop + Mathf.Ceil(statusTextHeight) + StatusBottomPadding;
        return Mathf.Max(BaseHeight, needed);
    }
}

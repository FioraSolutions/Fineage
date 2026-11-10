using UnityEngine;

/// <summary>
/// One nameplate ready to project and draw this frame.
/// Screen layout (top → bottom): title → name → optional cast gauge.
/// </summary>
public struct NameplatePaintItem
{
    public int Id;
    public Vector3 World;
    public string Name;
    public string Title;
    public Color NameColor;
    public Color TitleColor;
    public bool IsLocalPlayer;
    public L2TargetRenderType BubbleType;
    /// <summary>Local cast remaining 0..1; -1 when idle / not local.</summary>
    public float CastRemaining01;
    public bool ScreenValid;
    public float Depth;
    /// <summary>Snapped screen X of the overhead world anchor (character/head axis).</summary>
    public float AnchorX;
    public float NameW;
    public float XName;
    public float YNameTop;
    /// <summary>Canvas Y of the name line bottom (top-down). Cast gauge starts below this.</summary>
    public float YNameBottom;
}

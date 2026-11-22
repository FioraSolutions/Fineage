using System.Collections.Generic;

/// <summary>
/// HUD window specs sourced from Interlude <c>interface.xdat</c> (L2UI_CH3 / l2ui.utx).
/// Layout numbers are mirrored in matching UXML comments; runtime applies rects via
/// <see cref="L2InterludeHudSkinPipeline"/>.
/// </summary>
public static class InterludeHudWindowCatalog
{
    public const string SmallWndBack1 = "Data/UI/Target/Interlude/Target_Interlude_SmallWnd_Back1";
    public const string SmallWndBack2 = "Data/UI/Target/Interlude/Target_Interlude_SmallWnd_Back2";
    public const string SmallWndBack3Ex = "Data/UI/Target/Interlude/Target_Interlude_SmallWnd_Back3_Ex";

    public static readonly InterludeHudWindowEntry ChatWnd = new InterludeHudWindowEntry(
        "ChatWnd",
        348f,
        187f,
        new[]
        {
            "Data/UI/Assets/Chat/chatting_back1",
            "Data/UI/Assets/Chat/chatting_back2",
            "Data/UI/Assets/Chat/chatting_back3",
            "Data/UI/Assets/Chat/chatting_back4",
            "Data/UI/Assets/Chat/chatting_imeen",
            "Data/UI/Assets/Chat/sizecontrol_icon",
        });

    public static readonly InterludeHudWindowEntry StatusWnd = new InterludeHudWindowEntry(
        "StatusWnd",
        176f,
        84f,
        new[]
        {
            SmallWndBack1,
            SmallWndBack2,
            SmallWndBack3Ex,
            "Data/UI/Status/Interlude/Status_Interlude_Ps_CpBar_Back",
            "Data/UI/Status/Interlude/Status_Interlude_Ps_CpBar",
            "Data/UI/Status/Interlude/Status_Interlude_Ps_HpBar_Back",
            "Data/UI/Status/Interlude/Status_Interlude_Ps_HpBar",
            "Data/UI/Status/Interlude/Status_Interlude_Ps_MpBar_Back",
            "Data/UI/Status/Interlude/Status_Interlude_Ps_MpBar",
            "Data/UI/Status/Interlude/Status_Interlude_Ps_ExpBar_Back",
            "Data/UI/Status/Interlude/Status_Interlude_Ps_ExpBar",
            "Data/UI/Status/ps_levelback",
            "Data/UI/Assets/Chat/sizecontrol_icon",
        });

    public static readonly InterludeHudWindowEntry ShortcutWnd = new InterludeHudWindowEntry(
        "ShortcutWnd",
        508f,
        48f,
        new[]
        {
            SmallWndBack1,
            SmallWndBack2,
            SmallWndBack3Ex,
            "Data/UI/Assets/SkillBar/ShortcutWnd_DF_NumberGroupBox",
            "Data/UI/ShortCut/footer/Back_ShortCut_Footer",
        });

    public static readonly InterludeHudWindowEntry RadarWnd = new InterludeHudWindowEntry(
        "RadarContainerWnd",
        102f,
        102f,
        new[]
        {
            "Data/UI/Map/Radar/radar_arrow",
            "Data/UI/Map/Radar/radar_n",
            "Data/UI/Map/Radar/radar_myself",
            "Data/UI/Map/Radar/radar_target",
        });

    /// <summary>Interlude BuffWnd / SkillTray abnormal row beside StatusWnd.</summary>
    public static readonly InterludeHudWindowEntry BuffWnd = new InterludeHudWindowEntry(
        "BuffWnd",
        0f,
        0f,
        new[]
        {
            SmallWndBack1,
            SmallWndBack2,
            SmallWndBack3Ex,
        });

    /// <summary>CH3 Minimap.mapbutton_zoom* — validated at runtime; import from L2UI_CH3 if missing.</summary>
    public static readonly string[] RadarZoomTexturePaths =
    {
        "Data/UI/Map/Radar/mapbutton_zoomin1",
        "Data/UI/Map/Radar/mapbutton_zoomin1_over",
        "Data/UI/Map/Radar/mapbutton_zoomout1",
        "Data/UI/Map/Radar/mapbutton_zoomout1_over",
    };

    public static readonly InterludeHudWindowEntry MinimapWnd = new InterludeHudWindowEntry(
        "MinimapWnd",
        334f,
        413f,
        new[]
        {
            "Data/UI/Map/MapBack",
            "Data/UI/Map/MinimapPos",
            "Data/UI/Map/WorldMap_Black",
        });

    /// <summary>Chat input row relative to ChatWnd root (interface.xdat ChattingChatInput / InnerBar).</summary>
    public static readonly InterludeHudControlRect ChatInputBar = new InterludeHudControlRect(
        left: 22f,
        right: 6f,
        bottom: 13f,
        height: 18f);

    public static readonly InterludeHudControlRect ChatOptions = new InterludeHudControlRect(
        left: 3f,
        bottom: 4f,
        width: 16f,
        height: 36f);

    public static readonly InterludeHudControlRect MapViewport = new InterludeHudControlRect(
        left: 3f,
        top: 51f,
        width: 328f,
        height: 328f);

    public static readonly InterludeHudControlRect TownMapLayer = new InterludeHudControlRect(
        left: 0f,
        top: 0f,
        width: 330f,
        height: 328f);

    public static IEnumerable<InterludeHudWindowEntry> AllEntries()
    {
        yield return ChatWnd;
        yield return StatusWnd;
        yield return ShortcutWnd;
        yield return RadarWnd;
        yield return MinimapWnd;
        yield return BuffWnd;
    }

    /// <summary>Later-chronicle frame packages that must not appear on the in-game HUD.</summary>
    public static bool IsHighFiveChromeResourcePath(string resourcesPath)
    {
        if (string.IsNullOrEmpty(resourcesPath))
            return false;

        return resourcesPath.Contains("Windows_DF")
            || resourcesPath.Contains("Frame_DF")
            || resourcesPath.Contains("Windows/Windows_DF")
            || resourcesPath.Contains("normal_alpha_texture/Windows_DF");
    }
}

public readonly struct InterludeHudWindowEntry
{
    public readonly string XdatName;
    public readonly float Width;
    public readonly float Height;
    public readonly string[] TextureResourcePaths;

    public InterludeHudWindowEntry(string xdatName, float width, float height, string[] textureResourcePaths)
    {
        XdatName = xdatName;
        Width = width;
        Height = height;
        TextureResourcePaths = textureResourcePaths ?? new string[0];
    }
}

public readonly struct InterludeHudControlRect
{
    public readonly float Left;
    public readonly float Top;
    public readonly float Right;
    public readonly float Bottom;
    public readonly float Width;
    public readonly float Height;

    public InterludeHudControlRect(
        float left = float.NaN,
        float top = float.NaN,
        float right = float.NaN,
        float bottom = float.NaN,
        float width = float.NaN,
        float height = float.NaN)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
        Width = width;
        Height = height;
    }
}
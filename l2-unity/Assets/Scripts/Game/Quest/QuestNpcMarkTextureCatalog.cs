using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interlude NPC quest markers from LineageDecosTex:
/// <c>Q_t00</c> (yellow ?) and <c>e_t00</c> (cyan !), each a 128x256 two-frame sheet.
/// Paths are the Unity Resources stems (no fallback textures).
/// </summary>
public static class QuestNpcMarkTextureCatalog
{
    static readonly Dictionary<byte, string> Paths = new()
    {
        { QuestNpcMarkKind.Available, "Data/SysTextures/LineageDecos/Q_t00" },
        { QuestNpcMarkKind.InProgress, "Data/SysTextures/LineageDecos/e_t00" },
        { QuestNpcMarkKind.Return, "Data/SysTextures/LineageDecos/Q_t00" },
    };

    static readonly HashSet<string> _missingLogged = new();

    public static Texture2D Load(byte mark)
    {
        if (!Paths.TryGetValue(mark, out string path))
            return null;
        var tex = Resources.Load<Texture2D>(path);
        if (tex == null && _missingLogged.Add(path))
            Debug.LogError($"[QuestNpcMark] missing marker texture path={path} mark={mark}. Import LineageDecosTex from Interlude client.");
        return tex;
    }

    /// <summary>
    /// Available and Return share <c>Q_t00</c>; tints keep them distinct.
    /// In-progress uses the original cyan <c>e_t00</c> un-tinted.
    /// </summary>
    public static Color Tint(byte mark) => mark switch
    {
        QuestNpcMarkKind.Available => new Color(0.45f, 0.72f, 1f),
        QuestNpcMarkKind.InProgress => Color.white,
        QuestNpcMarkKind.Return => new Color(1f, 0.92f, 0.35f),
        _ => Color.white
    };
}

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interlude NPC quest markers: LineageDecosTex <c>e_t00</c> (!) and <c>Q_t00</c> (?).
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
        {
            if (_missingLogged.Add("mark:" + mark))
                Debug.LogError($"[QuestNpcMark] unknown mark id={mark} (expected Available/InProgress/Return).");
            return null;
        }
        var tex = Resources.Load<Texture2D>(path);
        if (tex == null && _missingLogged.Add(path))
            Debug.LogError($"[QuestNpcMark] missing marker texture path={path} mark={mark}. Import LineageDecosTex from Interlude client.");
        return tex;
    }

    /// <summary>Interlude tints: blue available, yellow in-progress/return question marks.</summary>
    public static Color Tint(byte mark) => mark switch
    {
        QuestNpcMarkKind.Available => new Color(0.45f, 0.72f, 1f),
        QuestNpcMarkKind.InProgress => Color.white,
        QuestNpcMarkKind.Return => new Color(1f, 0.92f, 0.35f),
        _ => UnknownMarkTint(mark)
    };

    static Color UnknownMarkTint(byte mark)
    {
        if (_missingLogged.Add("tint:" + mark))
            Debug.LogError($"[QuestNpcMark] unknown mark id={mark} for tint (using white).");
        return Color.white;
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// Deep anim/gait trace for Human Magic CharInfo only (MMagic / FMagic).
/// Covers MoveToPawn AND post-MagicShot return (cast playable → FinishRemoteCast → idle/gait).
/// Filter console: [MAGIC_GAIT]
/// File: logs/diag/magic-gait.txt (via L2DiagPaths)
/// </summary>
public static partial class MagicGaitTrace
{
    static string LogPath => L2DiagPaths.DiagFile("magic-gait.txt");

    // Per-id throttle only for high-frequency sync ticks.
    static readonly Dictionary<int, float> LastSyncAt = new Dictionary<int, float>(32);
    static readonly Dictionary<int, float> PrevNorm = new Dictionary<int, float>(32);
    // After MagicShot finish — denser sync for a few seconds to catch freeze.
    static readonly Dictionary<int, float> WatchUntil = new Dictionary<int, float>(32);
    static readonly StringBuilder Sb = new StringBuilder(1024);

    public static bool Enabled { get; set; } = false;

}

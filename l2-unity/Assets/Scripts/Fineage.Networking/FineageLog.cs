using System;
using System.IO;
using UnityEngine;

public static partial class FineageLog
{
    const long MaxFileBytes = 3 * 1024 * 1024;
    const int MaxRotations = 64;

    /// <summary>Runtime toggle for Debug (also enabled by FINEAGE_LOG_DEBUG define).</summary>
    public static bool DebugEnabled;

    /// <summary>
    /// Per-play audio trace. Also enabled by the environment variable FINEAGE_AUDIO_TRACE=1.
    /// </summary>
    public static bool AudioTraceEnabled;

    static bool DebugAllowed
    {
        get
        {
#if FINEAGE_LOG_DEBUG
            return true;
#else
            return DebugEnabled;
#endif
        }
    }









}

using System;
using System.IO;
using UnityEngine;

public static partial class FineageLog
{
    const long MaxFileBytes = 3 * 1024 * 1024;

    /// <summary>Runtime toggle for Debug (also enabled by FINEAGE_LOG_DEBUG define).</summary>
    public static bool DebugEnabled;

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

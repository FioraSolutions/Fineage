using System;
using System.IO;
using UnityEngine;

public static partial class FineageLog
{
    public static void Error(FineageLogCategory category, string message) =>
        Write(FineageLogLevel.Error, category, message);

    public static void Warn(FineageLogCategory category, string message) =>
        Write(FineageLogLevel.Warn, category, message);

    public static void Info(FineageLogCategory category, string message) =>
        Write(FineageLogLevel.Info, category, message);

    public static void Debug(FineageLogCategory category, string message)
    {
        if (!DebugAllowed)
            return;
        Write(FineageLogLevel.Debug, category, message);
    }
}

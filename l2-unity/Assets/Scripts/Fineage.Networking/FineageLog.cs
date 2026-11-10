using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Small leveled logger for networking / world milestones.
/// Console + rotating file under <c>logs/diag/</c> (max ~3MB, keep 1 backup).
/// Debug is off unless <c>FINEAGE_LOG_DEBUG</c> is defined or <see cref="DebugEnabled"/> is set.
/// </summary>
public static class FineageLog
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

    public static void Exception(FineageLogCategory category, string where, Exception ex)
    {
        Write(FineageLogLevel.Error, category, "EXCEPTION at " + where + "\n" + ex);
    }

    public static string Prefix(FineageLogCategory category)
    {
        switch (category)
        {
            case FineageLogCategory.Lobby:
                return "[LOBBY]";
            case FineageLogCategory.LoginNet:
                return "[LoginNet]";
            case FineageLogCategory.GameNet:
                return "[GameNet]";
            case FineageLogCategory.Net:
                return "[Net]";
            default:
                return "[World]";
        }
    }

    static void Write(FineageLogLevel level, FineageLogCategory category, string message)
    {
        string line = Prefix(category) + " " + message;

        switch (level)
        {
            case FineageLogLevel.Error:
                UnityEngine.Debug.LogError(line);
                break;
            case FineageLogLevel.Warn:
                UnityEngine.Debug.LogWarning(line);
                break;
            default:
                UnityEngine.Debug.Log(line);
                break;
        }

        try
        {
            string path = L2DiagPaths.DiagFile(FileNameFor(category));
            RotateIfNeeded(path);
            File.AppendAllText(
                path,
                DateTime.Now.ToString("HH:mm:ss.fff") + " " + level + " " + line + "\n");
        }
        catch
        {
            /* diag file is best-effort */
        }
    }

    static string FileNameFor(FineageLogCategory category)
    {
        switch (category)
        {
            case FineageLogCategory.Lobby:
            case FineageLogCategory.LoginNet:
                return "lobby-flow.txt";
            case FineageLogCategory.GameNet:
                return "game-net.txt";
            case FineageLogCategory.Net:
                return "net.txt";
            default:
                return "world.txt";
        }
    }

    static void RotateIfNeeded(string path)
    {
        if (!File.Exists(path))
            return;
        FileInfo info = new FileInfo(path);
        if (info.Length < MaxFileBytes)
            return;

        string bak = path + ".1";
        try
        {
            if (File.Exists(bak))
                File.Delete(bak);
            File.Move(path, bak);
        }
        catch
        {
            try { File.WriteAllText(path, string.Empty); } catch { /* ignore */ }
        }
    }
}

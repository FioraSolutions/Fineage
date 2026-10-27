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
    public enum Level
    {
        Error = 0,
        Warn = 1,
        Info = 2,
        Debug = 3
    }

    public enum Category
    {
        Lobby,
        LoginNet,
        GameNet,
        ZoneV2,
        World
    }

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

    public static void Error(Category category, string message) => Write(Level.Error, category, message);

    public static void Warn(Category category, string message) => Write(Level.Warn, category, message);

    public static void Info(Category category, string message) => Write(Level.Info, category, message);

    public static void Debug(Category category, string message)
    {
        if (!DebugAllowed)
            return;
        Write(Level.Debug, category, message);
    }

    public static void Exception(Category category, string where, Exception ex)
    {
        Write(Level.Error, category, "EXCEPTION at " + where + "\n" + ex);
    }

    public static string Prefix(Category category)
    {
        switch (category)
        {
            case Category.Lobby:
                return "[LOBBY]";
            case Category.LoginNet:
                return "[LoginNet]";
            case Category.GameNet:
                return "[GameNet]";
            case Category.ZoneV2:
                return "[ZoneV2]";
            default:
                return "[World]";
        }
    }

    static void Write(Level level, Category category, string message)
    {
        string line = Prefix(category) + " " + message;

        switch (level)
        {
            case Level.Error:
                UnityEngine.Debug.LogError(line);
                break;
            case Level.Warn:
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

    static string FileNameFor(Category category)
    {
        switch (category)
        {
            case Category.Lobby:
            case Category.LoginNet:
                return "lobby-flow.txt";
            case Category.GameNet:
                return "game-net.txt";
            case Category.ZoneV2:
                return "zone-v2.txt";
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

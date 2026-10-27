namespace Fineage.ZoneServer;

/// <summary>
/// Leveled milestone logger for ZoneServer v2.
/// Console + rotating file under <c>logs/zone.log</c> (~3MB, one .1 backup).
/// Debug via <c>--verbose</c> / <c>-v</c> or env <c>FINEAGE_ZONE_DEBUG=1</c>.
/// </summary>
static class ZoneLog
{
    public enum Level
    {
        Error = 0,
        Warn = 1,
        Info = 2,
        Debug = 3
    }

    const long MaxFileBytes = 3 * 1024 * 1024;
    const string Prefix = "[ZONE]";

    static readonly object Gate = new();
    static string? _filePath;
    static bool _configured;

    public static bool Verbose { get; private set; }

    public static void Configure(bool verboseFlag)
    {
        bool env =
            string.Equals(Environment.GetEnvironmentVariable("FINEAGE_ZONE_DEBUG"), "1", StringComparison.Ordinal) ||
            string.Equals(Environment.GetEnvironmentVariable("FINEAGE_ZONE_DEBUG"), "true", StringComparison.OrdinalIgnoreCase);
        Verbose = verboseFlag || env;
        try
        {
            string dir = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            Directory.CreateDirectory(dir);
            _filePath = Path.Combine(dir, "zone.log");
        }
        catch
        {
            _filePath = null;
        }

        _configured = true;
    }

    public static string Stamp() => DateTime.Now.ToString("HH:mm:ss.fff");

    public static void Error(string message) => Write(Level.Error, message);

    public static void Warn(string message) => Write(Level.Warn, message);

    public static void Info(string message) => Write(Level.Info, message);

    public static void Debug(string message)
    {
        if (!Verbose)
            return;
        Write(Level.Debug, message);
    }

    static void Write(Level level, string message)
    {
        if (!_configured)
            Configure(false);

        string line = $"{Prefix} {message}";
        string stamped = $"[{Stamp()}] {level.ToString().ToUpperInvariant(),-5} {line}";
        Console.WriteLine(stamped);

        string? path = _filePath;
        if (path == null)
            return;

        lock (Gate)
        {
            try
            {
                RotateIfNeeded(path);
                File.AppendAllText(path, stamped + "\n");
            }
            catch
            {
                /* file sink is best-effort */
            }
        }
    }

    static void RotateIfNeeded(string path)
    {
        if (!File.Exists(path))
            return;
        var info = new FileInfo(path);
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
            try { File.WriteAllText(path, string.Empty); }
            catch { /* ignore */ }
        }
    }
}

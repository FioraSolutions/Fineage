using System;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

/// <summary>
/// Melee timing trace, one line per step of a swing on the client: Attack / StatusUpdate packet received,
/// attack animation started, AttackShot notify, hit label shown, target HP bar changed.
/// Written to logs/diag/combat-trace.txt (not the Unity console). Every line carries the wall clock
/// <c>wall</c> (unix ms) — the same clock the server's <c>Fineage.Zone.CombatTrace</c> lines use, so a local
/// session lines up line by line — plus the frame number.
/// On by default in the Editor and development builds; <c>FINEAGE_COMBAT_TRACE=0</c> turns it off,
/// <c>=1</c> turns it on in a release player; <see cref="Enabled"/> toggles it at runtime.
/// </summary>
public static class CombatTrace
{
    public const string FileName = "combat-trace.txt";
    public const string Variable = "FINEAGE_COMBAT_TRACE";

    static bool? _enabled;
    static string _path;
    static int _mainThreadId = -1;
    static readonly object Gate = new object();

    // Unity APIs (paths, build flags, frame count) are main-thread only; packets may be traced from the
    // network thread, so everything Unity-side is captured once at startup.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void CaptureMainThread()
    {
        _mainThreadId = Thread.CurrentThread.ManagedThreadId;
        _path = L2DiagPaths.DiagFile(FileName);
        _enabled = ResolveDefault(Environment.GetEnvironmentVariable(Variable), Application.isEditor || Debug.isDebugBuild);
    }

    static bool OnMainThread => Thread.CurrentThread.ManagedThreadId == _mainThreadId;

    public static bool Enabled
    {
        get
        {
            if (!_enabled.HasValue)
                CaptureMainThread();
            return _enabled.Value;
        }
        set { _enabled = value; }
    }

    public static bool ResolveDefault(string value, bool debugBuild)
    {
        if (!string.IsNullOrEmpty(value))
        {
            string v = value.Trim().ToLowerInvariant();
            return v == "1" || v == "true" || v == "on" || v == "yes";
        }
        return debugBuild;
    }

    public static long WallUnixMs() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    static readonly System.Collections.Generic.Dictionary<int, long> DieWall = new System.Collections.Generic.Dictionary<int, long>();

    /// <summary>Remember when Die for <paramref name="objectId"/> arrived so death anim / sound lines carry <c>sinceDieMs</c>.</summary>
    public static void NoteDie(int objectId)
    {
        if (!Enabled)
            return;
        lock (DieWall)
        {
            if (DieWall.Count > 512)
                DieWall.Clear();
            DieWall[objectId] = WallUnixMs();
        }
    }

    /// <summary>Wall ms since Die for <paramref name="objectId"/>, or -1 when no Die was seen (or it is older than 10 s).</summary>
    public static long SinceDieMs(int objectId)
    {
        if (objectId == 0)
            return -1;
        lock (DieWall)
        {
            if (!DieWall.TryGetValue(objectId, out long at))
                return -1;
            long d = WallUnixMs() - at;
            return d > 10000 ? -1 : d;
        }
    }

    public static void ForgetDie(int objectId)
    {
        lock (DieWall)
            DieWall.Remove(objectId);
    }

    /// <summary><paramref name="fields"/> is "key=value key=value"; keep it allocation-light at call sites behind <see cref="Enabled"/>.</summary>
    public static void Write(string eventName, string fields)
    {
        if (!Enabled)
            return;
        var sb = new StringBuilder(160);
        sb.Append(DateTime.Now.ToString("HH:mm:ss.fff"))
            .Append(" wall=").Append(WallUnixMs())
            .Append(" frame=").Append(OnMainThread ? Time.frameCount : -1)
            .Append(' ').Append(eventName);
        if (!string.IsNullOrEmpty(fields))
            sb.Append(' ').Append(fields);
        sb.Append('\n');
        try
        {
            lock (Gate)
                File.AppendAllText(_path, sb.ToString());
        }
        catch (Exception ex)
        {
            _enabled = false;
            Debug.LogError("[CombatTrace] cannot write " + _path + ": " + ex.Message + " — trace turned off.");
        }
    }
}

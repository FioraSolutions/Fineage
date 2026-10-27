using UnityEngine;

/// <summary>
/// Feature flag for Interlude stub vs Fineage ZoneServer v2.
/// Inspector toggle on <see cref="GameManager"/> wins when set; else PlayerPrefs.
/// </summary>
public static class FineageWireConfig
{
    public const string PrefKey = "Fineage.WireMode";

    public enum Mode
    {
        Interlude = 0,
        ZoneV2 = 1
    }

    static Mode? _override;

    public static string ZoneHost = "127.0.0.1";
    public static int ZonePort = 9777;

    public static void SetOverride(Mode mode)
    {
        _override = mode;
    }

    public static void ClearOverride()
    {
        _override = null;
    }

    public static Mode Current
    {
        get
        {
            if (_override.HasValue)
                return _override.Value;
            return (Mode)PlayerPrefs.GetInt(PrefKey, (int)Mode.Interlude);
        }
        set
        {
            PlayerPrefs.SetInt(PrefKey, (int)value);
            PlayerPrefs.Save();
        }
    }

    public static bool IsZoneV2
    {
        get { return Current == Mode.ZoneV2; }
    }
}

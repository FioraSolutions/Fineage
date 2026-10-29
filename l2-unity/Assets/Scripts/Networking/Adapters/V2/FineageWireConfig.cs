using UnityEngine;

/// <summary>
/// Wire protocol selection (legacy Interlude stub vs Fineage ZoneServer v2) and the active endpoints.
/// <see cref="FineageConnection.Initialize"/> calls <see cref="Use"/> once at startup; after that every
/// connection reads <see cref="Endpoints"/>. Nothing else writes hosts or ports.
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

    /// <summary>Active login/zone endpoints. Public RP until <see cref="Use"/> says otherwise.</summary>
    public static FineageEndpoints Endpoints { get; private set; } = FineageEndpoints.PublicDefault();

    /// <summary>Select the Fineage-Backend (login lobby + handoff + ZoneV2) on the given endpoints.</summary>
    public static void Use(FineageEndpoints endpoints)
    {
        Endpoints = endpoints ?? FineageEndpoints.PublicDefault();
        SetOverride(Mode.ZoneV2);
    }

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

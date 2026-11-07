/// <summary>
/// Server profile selection. Public RP is the only profile compiled into players:
/// the Local profile reads EditorPrefs inside <c>#if UNITY_EDITOR</c>, so every
/// Standalone / GitHub release-clients build resolves <see cref="IsLocalActive"/> == false.
/// Switch in Editor: Fineage/Play Local or Fineage/Play RP.
/// </summary>
public static class FineageServerProfile
{
    public const string EditorPrefKey = "Fineage.ServerProfile.Local";
    public const string LocalHost = "127.0.0.1";
    public const int LocalLoginPort = 9106;
    public const int LocalGamePort = 9777;

    static bool _local;

    /// <summary>Main thread (GameManager.Awake). Player builds: always false.</summary>
    public static void Refresh()
    {
#if UNITY_EDITOR
        _local = UnityEditor.EditorPrefs.GetBool(EditorPrefKey, false);
#else
        _local = false;
#endif
    }

    public static bool IsLocalActive
    {
#if UNITY_EDITOR
        get { return _local; }
#else
        get { return false; }
#endif
    }

    /// <summary>Public host always; loopback only when the Editor Local profile is active.</summary>
    public static bool IsAllowedHost(string host)
    {
        if (FineageWireConfig.IsPublicLoginHost(host))
            return true;
        return IsLocalActive && !string.IsNullOrWhiteSpace(host);
    }
}

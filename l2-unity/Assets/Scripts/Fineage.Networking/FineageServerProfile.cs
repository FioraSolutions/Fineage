/// <summary>
/// The environment this run dials, resolved once at startup through
/// <see cref="FineageBackendSelector"/>.
///
/// <para>
/// Defaults: <b>Local in the Editor</b> (debug against your own Fineage-Backend — the
/// long-standing Editor behaviour) and <b>Remote in a player build</b>. Either can be
/// overridden: Fineage/Play Remote / Play Local remembers the Editor choice, and a dev
/// player honours <c>FINEAGE_BACKEND=local</c> (legacy <c>FINEAGE_NET_CLIENT=1</c>).
/// </para>
///
/// <para>
/// A release-clients build is compiled without <c>FINEAGE_LOCAL_CAPABLE</c>
/// (<see cref="LocalOverridesAllowed"/> is false), so no environment variable, command line
/// or preference can point it at loopback. The dev Mac player
/// (<c>Fineage/Build Dev Mac</c>) does carry the define and ships <c>launch.sh</c> (remote)
/// and <c>launch-local.sh</c> (local).
/// </para>
/// </summary>
public static class FineageServerProfile
{
    /// <summary>EditorPrefs flag holding the menu choice. Absent means "no choice yet".</summary>
    public const string EditorPrefKey = "Fineage.ServerProfile.Local";

    static bool _resolved;
    static FineageBackend _current = FineageBackend.Remote;

    /// <summary>Re-reads the command line, environment and (in the Editor) the stored choice.</summary>
    public static void Refresh()
    {
        _current = FineageBackendSelector.Resolve(
            System.Environment.GetCommandLineArgs(),
            System.Environment.GetEnvironmentVariable(FineageBackendSelector.BackendEnv),
            System.Environment.GetEnvironmentVariable(FineageBackendSelector.LegacyEnv),
            StoredChoice(),
            DefaultBackend,
            LocalOverridesAllowed);
        _resolved = true;
    }

    /// <summary>
    /// The default when nothing asks otherwise: Local in the Editor (debug against your own
    /// backend, which is how the Editor has always behaved), Remote in a player build.
    /// </summary>
    public static FineageBackend DefaultBackend
    {
        get
        {
#if UNITY_EDITOR
            return FineageBackend.Local;
#else
            return FineageBackend.Remote;
#endif
        }
    }

    /// <summary>
    /// Whether this host may ever dial loopback: the Editor, or a build that opted in with
    /// <c>FINEAGE_LOCAL_CAPABLE</c> (Fineage → Build Dev Mac).
    ///
    /// <para>
    /// Deliberately not keyed on <c>DEVELOPMENT_BUILD</c> or on any setting a player could
    /// influence: a shipped release-clients player has no switch at all, so a stray
    /// <c>FINEAGE_BACKEND</c> in someone's shell cannot steal their login away from the
    /// public server.
    /// </para>
    /// </summary>
    public static bool LocalOverridesAllowed
    {
        get
        {
#if UNITY_EDITOR || FINEAGE_LOCAL_CAPABLE
            return true;
#else
            return false;
#endif
        }
    }

    public static FineageBackend Current
    {
        get
        {
            if (!_resolved)
                Refresh();
            return _current;
        }
    }

    public static bool IsLocalActive
    {
        get { return Current == FineageBackend.Local; }
    }

    public static bool IsRemoteActive
    {
        get { return Current == FineageBackend.Remote; }
    }

    /// <summary>The stored menu choice, or null when the menu was never used. Editor only.</summary>
    public static bool? StoredChoice()
    {
#if UNITY_EDITOR
        if (!UnityEditor.EditorPrefs.HasKey(EditorPrefKey))
            return null;
        return UnityEditor.EditorPrefs.GetBool(EditorPrefKey);
#else
        return null;
#endif
    }

    /// <summary>Records the Editor menu choice for the next Play. A no-op in a player.</summary>
    public static void StoreLocalPreference(bool local)
    {
#if UNITY_EDITOR
        UnityEditor.EditorPrefs.SetBool(EditorPrefKey, local);
#endif
        _current = local && LocalOverridesAllowed ? FineageBackend.Local : FineageBackend.Remote;
        _resolved = true;
    }
}

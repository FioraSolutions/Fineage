using Xunit;

namespace Fineage.Client.Domain.Tests;

/// <summary>
/// Two environments — remote playtest and local Fineage-Backend.
///
/// <para>
/// The regression these pin: a release-clients player must never reach loopback. It is
/// compiled without <c>FINEAGE_LOCAL_CAPABLE</c>, so the selector has to answer Remote even
/// when the environment, the command line or a stray preference says local.
/// </para>
/// </summary>
public class BackendSelectorTests
{
    static FineageBackend Resolve(
        string[] args = null,
        string backend = null,
        string legacy = null,
        bool? storedChoice = null,
        FineageBackend fallback = FineageBackend.Remote,
        bool localAllowed = true)
    {
        return FineageBackendSelector.Resolve(args, backend, legacy, storedChoice, fallback, localAllowed);
    }

    [Fact]
    public void Nothing_set_uses_the_host_default()
    {
        Assert.Equal(FineageBackend.Remote, Resolve());
        Assert.Equal(FineageBackend.Local, Resolve(fallback: FineageBackend.Local));
        Assert.Equal(FineageBackend.Remote, Resolve(storedChoice: false, fallback: FineageBackend.Local));
    }

    [Fact]
    public void A_release_player_without_the_capability_stays_remote()
    {
        // Every route to loopback is closed at compile time, not by configuration.
        Assert.Equal(FineageBackend.Remote, Resolve(backend: "local", localAllowed: false));
        Assert.Equal(FineageBackend.Remote, Resolve(legacy: "1", localAllowed: false));
        Assert.Equal(FineageBackend.Remote, Resolve(new[] { "-fineageBackend", "local" }, localAllowed: false));
        Assert.Equal(FineageBackend.Remote, Resolve(storedChoice: true, localAllowed: false));
        Assert.Equal(
            FineageBackend.Remote,
            Resolve(fallback: FineageBackend.Local, localAllowed: false));
    }

    [Fact]
    public void Environment_selects_the_local_backend()
    {
        Assert.Equal(FineageBackend.Local, Resolve(backend: "local"));
        Assert.Equal(FineageBackend.Local, Resolve(backend: "  LOCAL  "));
        Assert.Equal(FineageBackend.Local, Resolve(backend: "loopback"));
        Assert.Equal(FineageBackend.Remote, Resolve(backend: "remote", fallback: FineageBackend.Local));
        Assert.Equal(FineageBackend.Remote, Resolve(backend: "public", fallback: FineageBackend.Local));
    }

    [Fact]
    public void Legacy_net_client_switch_still_works()
    {
        Assert.Equal(FineageBackend.Local, Resolve(legacy: "1"));
        Assert.Equal(FineageBackend.Local, Resolve(legacy: "true"));
        Assert.Equal(FineageBackend.Local, Resolve(legacy: "local"));
        Assert.Equal(FineageBackend.Remote, Resolve(legacy: "0", fallback: FineageBackend.Local));
        Assert.Equal(FineageBackend.Remote, Resolve(legacy: "false", fallback: FineageBackend.Local));
    }

    [Fact]
    public void Unrecognised_values_do_not_override_the_default()
    {
        // A typo must not silently retarget anything.
        Assert.Equal(FineageBackend.Remote, Resolve(backend: "lcoal"));
        Assert.Equal(FineageBackend.Local, Resolve(backend: "lcoal", fallback: FineageBackend.Local));
        Assert.Equal(FineageBackend.Remote, Resolve(legacy: "yes"));
        Assert.Equal(FineageBackend.Local, Resolve(storedChoice: true));
    }

    [Fact]
    public void Command_line_beats_environment()
    {
        Assert.Equal(
            FineageBackend.Remote,
            Resolve(new[] { "-fineageBackend", "remote" }, backend: "local", fallback: FineageBackend.Local));

        Assert.Equal(
            FineageBackend.Local,
            Resolve(new[] { "--fineage-backend=local" }, backend: "remote"));

        Assert.Equal(
            FineageBackend.Local,
            Resolve(new[] { "Fineage", "-fineageBackend=local" }, legacy: "0"));
    }

    [Fact]
    public void Command_line_key_without_a_value_is_ignored()
    {
        Assert.Equal(
            FineageBackend.Remote,
            Resolve(new[] { "-fineageBackend", "-batchmode" }));

        Assert.Equal(
            FineageBackend.Remote,
            Resolve(new[] { "-fineageBackend" }));
    }

    [Fact]
    public void The_stored_editor_choice_beats_the_default_but_not_the_environment()
    {
        Assert.Equal(FineageBackend.Local, Resolve(storedChoice: true));
        Assert.Equal(FineageBackend.Remote, Resolve(storedChoice: false, fallback: FineageBackend.Local));
        Assert.Equal(FineageBackend.Remote, Resolve(storedChoice: true, backend: "remote"));
        Assert.Equal(FineageBackend.Local, Resolve(storedChoice: false, legacy: "1"));
    }

    [Fact]
    public void The_editor_default_is_local_and_a_player_default_is_remote()
    {
#if UNITY_EDITOR
        Assert.Equal(FineageBackend.Local, FineageServerProfile.DefaultBackend);
#else
        Assert.Equal(FineageBackend.Remote, FineageServerProfile.DefaultBackend);
#endif
    }

    [Fact]
    public void Storing_a_choice_cannot_reach_loopback_without_the_capability()
    {
        FineageServerProfile.StoreLocalPreference(true);
        Assert.Equal(
            FineageServerProfile.LocalOverridesAllowed ? FineageBackend.Local : FineageBackend.Remote,
            FineageServerProfile.Current);

        FineageServerProfile.StoreLocalPreference(false);
        Assert.True(FineageServerProfile.IsRemoteActive);
    }
}

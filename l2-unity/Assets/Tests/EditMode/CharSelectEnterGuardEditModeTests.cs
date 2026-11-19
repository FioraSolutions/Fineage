using NUnit.Framework;

public sealed class CharSelectEnterGuardEditModeTests
{
    [Test]
    public void TryBegin_then_Fail_clears_entering_so_Start_can_be_enabled()
    {
        var guard = new CharSelectEnterGuard();
        Assert.IsTrue(guard.TryBegin());
        Assert.IsTrue(guard.IsEntering);
        Assert.IsFalse(guard.TryBegin());
        guard.Fail();
        Assert.IsFalse(guard.IsEntering);
        Assert.IsTrue(guard.TryBegin());
    }

    [Test]
    public void Succeed_clears_entering_without_allowing_a_stale_timeout()
    {
        var guard = new CharSelectEnterGuard();
        Assert.IsTrue(guard.TryBegin());
        guard.Succeed();
        Assert.IsFalse(guard.IsEntering);
        Assert.IsFalse(guard.HasTimedOut(CharSelectEnterGuard.TimeoutSeconds + 1f));
    }

    [Test]
    public void HasTimedOut_only_after_the_grace_period_while_still_entering()
    {
        var guard = new CharSelectEnterGuard();
        Assert.IsFalse(guard.HasTimedOut(CharSelectEnterGuard.TimeoutSeconds));
        Assert.IsTrue(guard.TryBegin());
        Assert.IsFalse(guard.HasTimedOut(CharSelectEnterGuard.TimeoutSeconds - 0.01f));
        Assert.IsTrue(guard.HasTimedOut(CharSelectEnterGuard.TimeoutSeconds));
        Assert.IsTrue(guard.HasTimedOut(CharSelectEnterGuard.TimeoutSeconds + 5f));
    }

    [Test]
    public void Lobby_close_after_handoff_does_not_abort_world_enter()
    {
        Assert.IsFalse(CharSelectEnterGuard.ShouldAbortEnterOnLobbyClose(hasHandoff: true));
        Assert.IsTrue(CharSelectEnterGuard.ShouldAbortEnterOnLobbyClose(hasHandoff: false));
    }
}

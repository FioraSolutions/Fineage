using NUnit.Framework;

/// <summary>EditMode coverage for global NPC ChrSound cap (#1604).</summary>
public sealed class NpcVoicePlaybackGateEditModeTests
{
    [SetUp]
    public void SetUp()
    {
        NpcVoicePlaybackGate.Reset();
        NpcVoicePlaybackGate.MaxConcurrent = 2;
    }

    [Test]
    public void TryBegin_respects_max_concurrent_voices()
    {
        Assert.IsTrue(NpcVoicePlaybackGate.TryBegin(101, NpcVoicePriority.SocialIdle));
        Assert.IsTrue(NpcVoicePlaybackGate.TryBegin(102, NpcVoicePriority.SocialIdle));
        Assert.IsFalse(NpcVoicePlaybackGate.TryBegin(103, NpcVoicePriority.IdleAmbient));
        Assert.AreEqual(2, NpcVoicePlaybackGate.ActiveCount);

        NpcVoicePlaybackGate.End(101);
        Assert.IsTrue(NpcVoicePlaybackGate.TryBegin(103, NpcVoicePriority.IdleAmbient));
    }

    [Test]
    public void Targeted_priority_can_evict_idle_voice()
    {
        Assert.IsTrue(NpcVoicePlaybackGate.TryBegin(201, NpcVoicePriority.IdleAmbient));
        Assert.IsTrue(NpcVoicePlaybackGate.TryBegin(202, NpcVoicePriority.IdleAmbient));
        Assert.IsTrue(NpcVoicePlaybackGate.TryBegin(203, NpcVoicePriority.Targeted));
        Assert.AreEqual(2, NpcVoicePlaybackGate.ActiveCount);
        Assert.IsFalse(NpcVoicePlaybackGate.TryBegin(201, NpcVoicePriority.IdleAmbient));
    }
}
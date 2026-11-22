using NUnit.Framework;

public sealed class AnimChannelEditModeTests
{
    [TearDown]
    public void TearDown()
    {
        AnimChannel.Clear();
    }

    [Test]
    public void Cast_channel_blocks_social_until_cleared()
    {
        AnimChannel.Begin(AnimChannel.Kind.Cast);
        Assert.IsFalse(AnimChannel.CanPlay(AnimChannel.Kind.Social));
        AnimChannel.End(AnimChannel.Kind.Cast);
        Assert.IsTrue(AnimChannel.CanPlay(AnimChannel.Kind.Social));
    }

    [Test]
    public void Clear_releases_any_held_channel()
    {
        AnimChannel.Begin(AnimChannel.Kind.Cast);
        AnimChannel.Clear();
        Assert.AreEqual(AnimChannel.Kind.None, AnimChannel.Held);
    }
}
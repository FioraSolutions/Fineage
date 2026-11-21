using NUnit.Framework;

public sealed class MovementTimePolicyEditModeTests
{
    [Test]
    public void Maximum_delta_time_matches_TimeManager_asset()
    {
        Assert.AreEqual(0.1f, FineageMovementTimePolicy.ProjectMaximumDeltaTime);
    }
}
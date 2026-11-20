using Xunit;

public sealed class L2PawnHeadingStepTests
{
    [Fact]
    public void Rotation_rate_matches_interlude_heading_units_per_second()
    {
        const float deltaTime = 1f / 30f;
        float maxStep = L2PawnHeadingMath.DegreesPerSecond(L2PawnHeadingMath.RotationRateYaw) * deltaTime;
        Assert.InRange(maxStep, 11.8f, 12.0f);
    }

    [Fact]
    public void StepDegrees_clamps_to_max_step_toward_desired_yaw()
    {
        float next = L2PawnHeadingMath.StepDegrees(0f, 90f, 15f);
        Assert.Equal(15f, next, 3);
    }

    [Fact]
    public void StepDegrees_snaps_when_within_max_step()
    {
        float next = L2PawnHeadingMath.StepDegrees(80f, 90f, 15f);
        Assert.Equal(90f, next, 3);
    }

    [Fact]
    public void StepDegrees_steps_shortest_arc_across_wrap()
    {
        float next = L2PawnHeadingMath.StepDegrees(350f, 10f, 20f);
        Assert.Equal(10f, next, 3);
    }
}
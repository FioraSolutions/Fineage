using NUnit.Framework;

public sealed class GravityStepEditModeTests
{
    [Test]
    public void Gravity_marker_registered_in_roadmap_list()
    {
        Assert.AreEqual("Fineage.Gravity.Step", FineagePerfProfiling.GravityStepName);
    }
}
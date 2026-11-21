using NUnit.Framework;

public sealed class NameplateOverlapEditModeTests
{
    [Test]
    public void Overlap_buffer_size_matches_nonalloc_contract()
    {
        Assert.GreaterOrEqual(L2NameplateScreenLayout.OverlapSphereBufferSize, 64);
    }
}
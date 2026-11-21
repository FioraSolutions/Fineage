using NUnit.Framework;

public sealed class TileStreamingManifestEditModeTests
{
    [Test]
    public void Manifest_resolves_seeded_tile()
    {
        Assert.IsTrue(TileStreamingManifest.Contains("16_25"));
        Assert.IsFalse(TileStreamingManifest.Contains("99_99"));
    }
}
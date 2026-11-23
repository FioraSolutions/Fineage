using NUnit.Framework;

public sealed class MapTileLoadPolicyEditModeTests
{
    [Test]
    public void CatalogTileLoadsFromThePrefab()
    {
        Assert.IsTrue(MapTileLoadPolicy.IsMapTileName("22_22"));
        Assert.IsTrue(MapTileLoadPolicy.ShouldAttempt("22_22", sceneInBuild: false, manifestListed: true));
        Assert.AreEqual("Data/Maps/22_22/22_22", MapTileLoadPolicy.ExpectedPrefabResourcePath("22_22"));
    }

    [Test]
    public void BuildSceneDoesNotLoadAMapTile()
    {
        Assert.IsFalse(MapTileLoadPolicy.ShouldAttempt("19_21", sceneInBuild: true, manifestListed: false));
        Assert.IsTrue(MapTileLoadPolicy.ShouldAttempt("Game", sceneInBuild: true, manifestListed: false));
        Assert.IsFalse(MapTileLoadPolicy.IsMapTileName("Game"));
        Assert.IsFalse(MapTileLoadPolicy.ShouldAttempt("not_a_tile", sceneInBuild: false, manifestListed: false));
    }

    [Test]
    public void MissingTileThrowsNamingTheCatalog()
    {
        MapTileMissingException ex = Assert.Throws<MapTileMissingException>(() =>
        {
            throw MapTileLoadPolicy.Missing("99_99");
        });
        Assert.AreEqual("99_99", ex.TileId);
        Assert.AreEqual("Data/Maps/99_99/99_99", ex.ExpectedPath);
        Assert.AreEqual(MapTileLoadPolicy.Catalog, ex.Catalog);
        StringAssert.Contains("99_99", ex.Message);
        StringAssert.Contains("Data/Maps/99_99/99_99", ex.Message);
        StringAssert.Contains(MapTileLoadPolicy.Catalog, ex.Message);
    }

    [Test]
    public void SameHourDifferentMapIsNotCurrentLighting()
    {
        Assert.IsTrue(MapTileLoadPolicy.LightingIsCurrent(true, 12f, 12f, "17_25", "17_25"));
        Assert.IsFalse(MapTileLoadPolicy.LightingIsCurrent(true, 12f, 12f, "19_21", "17_25"));
        Assert.IsFalse(MapTileLoadPolicy.LightingIsCurrent(false, 12f, 12f, "19_21", "19_21"));
    }
}

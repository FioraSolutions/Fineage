using NUnit.Framework;

public sealed class MapTileLoadPolicyEditModeTests
{
    [Test]
    public void PrefabOnlyFieldTileIsStillAttempted()
    {
        // Giran 22_22 is a Resources map prefab and is not a Build Settings scene.
        Assert.IsTrue(MapTileLoadPolicy.IsMapTileName("22_22"));
        Assert.IsTrue(MapTileLoadPolicy.ShouldAttempt("22_22", sceneInBuild: false, manifestListed: false));
    }

    [Test]
    public void BuildSceneDoesNotExemptAMapTileFromThePrefabPass()
    {
        // Gludio 19_21 is in Build Settings as a stub. The streamer must still load it,
        // and the loader pairs that scene with Resources/Data/Maps/19_21.
        Assert.IsTrue(MapTileLoadPolicy.ShouldAttempt("19_21", sceneInBuild: true, manifestListed: false));
    }

    [Test]
    public void NonTileNameWithoutASceneIsNotAMapLoad()
    {
        Assert.IsFalse(MapTileLoadPolicy.IsMapTileName("Game"));
        Assert.IsFalse(MapTileLoadPolicy.ShouldAttempt("not_a_tile", sceneInBuild: false, manifestListed: false));
        Assert.IsTrue(MapTileLoadPolicy.ShouldAttempt("Game", sceneInBuild: true, manifestListed: false));
    }

    [Test]
    public void SameHourDifferentMapIsNotCurrentLighting()
    {
        Assert.IsTrue(MapTileLoadPolicy.LightingIsCurrent(true, 12f, 12f, "17_25", "17_25"));
        Assert.IsFalse(MapTileLoadPolicy.LightingIsCurrent(true, 12f, 12f, "19_21", "17_25"));
        Assert.IsFalse(MapTileLoadPolicy.LightingIsCurrent(false, 12f, 12f, "19_21", "19_21"));
    }
}

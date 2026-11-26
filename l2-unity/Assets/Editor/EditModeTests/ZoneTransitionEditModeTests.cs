using NUnit.Framework;

public sealed class ZoneTransitionEditModeTests
{
    [Test]
    public void MapRegionCatalog_resolves_elven_village_cell()
    {
        ClientMapRegionCatalog.EnsureLoaded();
        const float l2X = 46890f;
        const float l2Y = 51531f;
        string town = ClientMapRegionCatalog.ResolveTownName(l2X, l2Y);
        Assert.AreEqual("Elven Town", town);
        VectorUtils.GetSwMapTile(new UnityEngine.Vector3(l2X, l2Y, 0f), out int cx, out int cy);
        Assert.IsTrue(ClientMapRegionCatalog.TryResolveCell(cx, cy, out string fromCell));
        Assert.AreEqual(town, fromCell);
    }

    [Test]
    public void MapRegionCatalog_falls_back_for_unknown_ocean_cell()
    {
        ClientMapRegionCatalog.EnsureLoaded();
        string town = ClientMapRegionCatalog.ResolveTownName(0f, 0f);
        Assert.IsFalse(string.IsNullOrEmpty(town));
    }
}
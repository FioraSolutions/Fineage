using NUnit.Framework;
using UnityEngine;

public sealed class DetailedMapProjectionEditModeTests
{
    static DetailedMapZoneRecord ElvenVillage() => new DetailedMapZoneRecord
    {
        id = "elven_village",
        originL2X = 39168f,
        originL2Y = 42688f,
        scale = 0.05f,
        mapPixelWidth = 633,
        mapPixelHeight = 675
    };

    [Test]
    public void ProjectDetailedMapPixel_usesOriginAndScale()
    {
        var zone = ElvenVillage();
        Vector2 px = DetailedMapProjection.ProjectDetailedMapPixel(zone, 46890f, 51531f);
        Assert.AreEqual(386.1f, px.x, 0.2f);
        Assert.AreEqual(442.15f, px.y, 0.2f);
    }

    [Test]
    public void ContainsWorldPosition_respectsMapPixelBounds()
    {
        var zone = ElvenVillage();
        Assert.IsTrue(DetailedMapProjection.ContainsWorldPosition(zone, 46890f, 51531f));
        Assert.IsFalse(DetailedMapProjection.ContainsWorldPosition(zone, 30000f, 30000f));
    }

    [Test]
    public void ProjectWorldMapPixel_giranGatekeeperBand()
    {
        Vector2 px = DetailedMapProjection.ProjectWorldMapPixel(83551f, 147928f);
        Assert.Greater(px.x, 1000f);
        Assert.Greater(px.y, 1800f);
    }

    [Test]
    public void Catalog_findsElvenVillage_byPosition()
    {
        DetailedMapCatalog.EnsureLoaded();
        var zone = DetailedMapCatalog.FindByWorldPosition(46890f, 51531f);
        Assert.NotNull(zone);
        Assert.AreEqual("elven_village", zone.id);
    }
}
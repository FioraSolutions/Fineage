using NUnit.Framework;
using UnityEngine;

public sealed class InterludeWorldMapBoundsEditModeTests
{
    [Test]
    public void Loads_interlude_world_map_plane_from_streaming_assets()
    {
        InterludeWorldMapBounds.ResetForEditModeTests();
        Assert.AreEqual(1812f, InterludeWorldMapBounds.PixelWidth, 0.01f);
        Assert.AreEqual(2620f, InterludeWorldMapBounds.PixelHeight, 0.01f);
        Assert.AreEqual(-131072f, InterludeWorldMapBounds.L2MinX, 1f);
        Assert.AreEqual(228607f, InterludeWorldMapBounds.L2MaxX, 1f);
    }
}
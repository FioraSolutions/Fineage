using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// WorldPlacement kinds (#1802): ItemDrop searches floor; pawn kinds keep server Y authority.
/// </summary>
public sealed class WorldPlacementEditModeTests
{
    [Test]
    public void Only_item_drop_requires_floor_search()
    {
        Assert.IsTrue(WorldPlacement.RequiresFloorSearch(WorldPlacement.PlacementKind.ItemDrop));
        Assert.IsFalse(WorldPlacement.RequiresFloorSearch(WorldPlacement.PlacementKind.WalkValidate));
        Assert.IsFalse(WorldPlacement.RequiresFloorSearch(WorldPlacement.PlacementKind.NpcSpawn));
        Assert.IsFalse(WorldPlacement.RequiresFloorSearch(WorldPlacement.PlacementKind.MonsterSpawn));
        Assert.IsFalse(WorldPlacement.RequiresFloorSearch(WorldPlacement.PlacementKind.PlayerTeleport));
    }

    [Test]
    public void Item_drop_floor_pipeline_picks_deck_beyond_snap_tolerance()
    {
        float serverY = 20f;
        float deck = serverY - WorldGrounding.SnapToleranceMeters - 1f;
        var ys = new List<float> { deck };
        Assert.AreEqual(serverY, WorldGrounding.PickHeight(serverY, ys), 0.001f);
        Assert.IsTrue(WorldGrounding.TryPickFloorBelow(serverY, ys, out float floor));
        Assert.AreEqual(deck, floor, 0.001f);
    }

    [Test]
    public void Resnap_reference_uses_server_y_not_visual_y()
    {
        float serverY = 18f;
        float visualY = 18.01f;
        float floor = 12f;
        var reference = new Vector3(100f, serverY, 200f);
        var ys = new List<float> { floor };
        Assert.IsTrue(WorldGrounding.TryPickFloorBelow(reference.y, ys, out float picked));
        Assert.AreEqual(floor, picked, 0.001f);
        Assert.Greater(visualY - picked, WorldGrounding.SnapToleranceMeters);
    }
}
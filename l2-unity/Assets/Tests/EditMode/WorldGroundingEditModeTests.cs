using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Pure grounding picker: server Y is authority, nearest walkable within
/// tolerance wins, near-ties prefer the higher deck, a miss keeps server Y.
/// </summary>
public sealed class WorldGroundingEditModeTests
{
    [Test]
    public void Empty_or_null_keeps_server_y()
    {
        Assert.AreEqual(10f, WorldGrounding.PickHeight(10f, null));
        Assert.AreEqual(10f, WorldGrounding.PickHeight(10f, new List<float>()));
    }

    [Test]
    public void Nearest_hit_within_tolerance_is_chosen()
    {
        var ys = new List<float> { 1f, 9.4f, 20f };
        Assert.AreEqual(9.4f, WorldGrounding.PickHeight(10f, ys), 0.001f);
    }

    [Test]
    public void Hit_outside_tolerance_is_ignored()
    {
        float server = 10f;
        var ys = new List<float> { server - WorldGrounding.SnapToleranceMeters - 0.5f };
        Assert.AreEqual(server, WorldGrounding.PickHeight(server, ys), 0.001f);
    }

    [Test]
    public void Hit_on_tolerance_is_accepted()
    {
        float server = 10f;
        float hit = server - WorldGrounding.SnapToleranceMeters;
        Assert.AreEqual(hit, WorldGrounding.PickHeight(server, new List<float> { hit }), 0.001f);
    }

    [Test]
    public void Near_tie_prefers_the_higher_deck()
    {
        float server = 10f;
        float lower = 9.7f;
        float higher = 10.2f;
        Assert.LessOrEqual(Mathf.Abs(higher - lower), WorldGrounding.NearTieMeters);
        float picked = WorldGrounding.PickHeight(server, new List<float> { lower, higher });
        Assert.AreEqual(higher, picked, 0.001f);
    }

    [Test]
    public void Pick_floor_below_selects_highest_deck_under_server_y()
    {
        float server = 20f;
        var ys = new List<float> { 10f, 12f, 25f };
        Assert.IsTrue(WorldGrounding.TryPickFloorBelow(server, ys, out float floor));
        Assert.AreEqual(12f, floor, 0.001f);
    }

    [Test]
    public void Pick_floor_below_rejects_surfaces_above_body_height()
    {
        float server = 10f;
        var ys = new List<float> { 15f, 18f };
        Assert.IsFalse(WorldGrounding.TryPickFloorBelow(server, ys, out _));
    }

    [Test]
    public void Nearest_walkable_reports_closest_hit_for_diagnostics()
    {
        var ys = new List<float> { 1f, 9f, 30f };
        Assert.IsTrue(WorldGrounding.TryGetNearestWalkableY(10f, ys, out float nearest));
        Assert.AreEqual(9f, nearest, 0.001f);
    }

    [Test]
    public void Nameplate_height_from_collision_height_is_stable()
    {
        // npcgrp already in meters: 0.5 m half-height → 1 m + name offset above feet.
        float nameOffset = L2NameplateAnchor.DefaultHeadHeightOffsetMeters;
        float fromFeet = L2NameplateAnchor.HeightFromFeet(0.5f, nameOffset);
        Assert.AreEqual(1f + nameOffset, fromFeet, 0.001f);

        // Raw Interlude UU half-height 26.25 → 0.5 m.
        float fromUu = L2NameplateAnchor.HeightFromFeet(26.25f, nameOffset);
        Assert.AreEqual(fromFeet, fromUu, 0.001f);
    }
}

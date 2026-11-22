using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public sealed class NameplateOcclusionEditModeTests
{
    [Test]
    public void Clear_line_of_sight_when_no_hit()
    {
        Assert.IsTrue(L2NameplateOcclusionDecision.IsHeadAnchorVisible(
            hitDistanceMeters: 0f,
            anchorDistanceMeters: 12f,
            hitCollider: null,
            ownerRoot: null));
    }

    [Test]
    public void Occluded_when_hit_is_closer_than_anchor()
    {
        var wall = new GameObject("wall");
        var cap = wall.AddComponent<CapsuleCollider>();
        try
        {
            Assert.IsFalse(L2NameplateOcclusionDecision.IsHeadAnchorVisible(
                hitDistanceMeters: 4f,
                anchorDistanceMeters: 10f,
                hitCollider: cap,
                ownerRoot: null));
        }
        finally
        {
            Object.DestroyImmediate(wall);
        }
    }

    [Test]
    public void Ignores_owner_collider_at_same_depth()
    {
        var owner = new GameObject("owner");
        var body = owner.AddComponent<CapsuleCollider>();
        try
        {
            Assert.IsTrue(L2NameplateOcclusionDecision.IsHeadAnchorVisible(
                hitDistanceMeters: 9.9f,
                anchorDistanceMeters: 10f,
                hitCollider: body,
                ownerRoot: owner.transform));
        }
        finally
        {
            Object.DestroyImmediate(owner);
        }
    }

    [Test]
    public void Offscreen_hide_does_not_mutate_the_roster_during_enumeration()
    {
        var camGo = new GameObject("nameplate-cam");
        var camera = camGo.AddComponent<Camera>();
        var target = new GameObject("offscreen");
        target.transform.position = new Vector3(0f, -100f, 0f);
        var entries = new Dictionary<int, NameplateEntry>
        {
            {
                7,
                new NameplateEntry
                {
                    Id = 7,
                    Target = target.transform,
                    Visible = true,
                }
            }
        };

        try
        {
            Assert.DoesNotThrow(() =>
                L2NameplateOcclusionBatch.ApplyHeadOcclusion(entries, camera, 0, null));
            Assert.IsFalse(entries[7].Visible);
        }
        finally
        {
            Object.DestroyImmediate(target);
            Object.DestroyImmediate(camGo);
        }
    }

    [Test]
    public void Required_physics_layers_exist()
    {
        Assert.IsTrue(L2NameplateOcclusionMask.TryGetMask(out LayerMask mask));
        Assert.Greater(mask.value, 0);
    }
}
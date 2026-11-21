using NUnit.Framework;
using UnityEngine;

public sealed class CrowdTierVisualPolicyEditModeTests
{
    [Test]
    public void ShrinkInflatedSkinnedBoundsIfNeeded_only_clamps_oversized_bounds()
    {
        var mesh = new Mesh { name = "test_body_m00" };
        mesh.bounds = new Bounds(Vector3.zero, Vector3.one);

        var go = new GameObject("tier-policy-test");
        var smr = go.AddComponent<SkinnedMeshRenderer>();
        smr.sharedMesh = mesh;
        smr.localBounds = new Bounds(Vector3.zero, Vector3.one * 8f);

        CrowdTierVisualPolicy.ShrinkInflatedSkinnedBoundsIfNeeded(smr);
        Assert.LessOrEqual(smr.localBounds.size.x, mesh.bounds.size.x * 1.1f);

        smr.localBounds = mesh.bounds;
        Bounds before = smr.localBounds;
        CrowdTierVisualPolicy.ShrinkInflatedSkinnedBoundsIfNeeded(smr);
        Assert.AreEqual(before.size, smr.localBounds.size);

        Object.DestroyImmediate(go);
        Object.DestroyImmediate(mesh);
    }
}
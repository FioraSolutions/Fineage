using NUnit.Framework;
using UnityEngine;

public sealed class CrowdTierVisualPolicyEditModeTests
{
    [Test]
    public void ClampInflatedSkinnedLocalBounds_only_clamps_oversized_bounds()
    {
        var mesh = new Mesh { name = "test_body_m00" };
        mesh.bounds = new Bounds(Vector3.zero, Vector3.one);

        var go = new GameObject("tier-policy-test");
        var smr = go.AddComponent<SkinnedMeshRenderer>();
        smr.sharedMesh = mesh;
        smr.localBounds = new Bounds(Vector3.zero, Vector3.one * 8f);

        L2ActorMeshPolicy.ClampInflatedSkinnedLocalBounds(smr);
        Assert.LessOrEqual(smr.localBounds.size.x, mesh.bounds.size.x * 1.1f);

        smr.localBounds = mesh.bounds;
        Bounds before = smr.localBounds;
        L2ActorMeshPolicy.ClampInflatedSkinnedLocalBounds(smr);
        Assert.AreEqual(before.size, smr.localBounds.size);

        Object.DestroyImmediate(go);
        Object.DestroyImmediate(mesh);
    }

    [Test]
    public void Visible_tier_keeps_the_skinned_mesh_when_bounds_are_stale()
    {
        var profile = ScriptableObject.CreateInstance<CrowdLodProfile>();
        var mesh = new Mesh { name = "body_m00" };
        mesh.bounds = new Bounds(Vector3.zero, Vector3.one);
        var go = new GameObject("pawn");
        var animator = go.AddComponent<Animator>();
        var smr = go.AddComponent<SkinnedMeshRenderer>();
        smr.sharedMesh = mesh;
        smr.updateWhenOffscreen = false;

        CrowdTierVisualPolicy.Apply(go.transform, CrowdDetailTier.T3Far, profile);
        Assert.IsTrue(smr.updateWhenOffscreen);
        Assert.AreEqual(AnimatorCullingMode.CullUpdateTransforms, animator.cullingMode);

        CrowdTierVisualPolicy.Apply(go.transform, CrowdDetailTier.TCulled, profile);
        Assert.IsFalse(smr.updateWhenOffscreen);
        Assert.AreEqual(AnimatorCullingMode.CullCompletely, animator.cullingMode);

        Object.DestroyImmediate(go);
        Object.DestroyImmediate(mesh);
        Object.DestroyImmediate(profile);
    }
}

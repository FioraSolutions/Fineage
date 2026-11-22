using NUnit.Framework;
using UnityEngine;

public class L2Ue2FovEditModeTests
{
    [Test]
    public void Horizontal_50_matches_ue2_reference_aspects()
    {
        // UE2 FovAngle is horizontal; vertical derives from aspect (Vert-).
        Assert.AreEqual(38.5526f, L2Ue2Fov.VerticalForHorizontal(50f, 4f / 3f), 1e-3f);
        Assert.AreEqual(29.3950f, L2Ue2Fov.VerticalForHorizontal(50f, 16f / 9f), 1e-3f);
    }

    [Test]
    public void Square_aspect_keeps_horizontal_and_vertical_equal()
    {
        Assert.AreEqual(50f, L2Ue2Fov.VerticalForHorizontal(50f, 1f), 1e-4f);
    }

    [Test]
    public void Wider_aspect_sees_less_vertically()
    {
        float narrow = L2Ue2Fov.VerticalForHorizontal(50f, 4f / 3f);
        float wide = L2Ue2Fov.VerticalForHorizontal(50f, 21f / 9f);
        Assert.Less(wide, narrow);
    }

    [Test]
    public void Apply_sets_camera_vertical_from_its_aspect()
    {
        var go = new GameObject("fov-test");
        try
        {
            Camera cam = go.AddComponent<Camera>();
            cam.aspect = 4f / 3f;
            L2Ue2Fov.Apply(cam, L2Ue2Fov.OfficialHfovDeg);
            Assert.AreEqual(38.5526f, cam.fieldOfView, 1e-3f);
        }
        finally
        {
            Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void Bad_aspect_falls_back_to_square_not_nan()
    {
        float v = L2Ue2Fov.VerticalForHorizontal(50f, 0f);
        Assert.AreEqual(50f, v, 1e-4f);
    }
}

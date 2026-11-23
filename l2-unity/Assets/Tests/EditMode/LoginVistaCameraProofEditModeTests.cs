using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// The login vista is InterpolationPoint5 (the point Logon_Warp warps to).
/// Camera25 is the UnrealEd viewport U2Viewport2 and is not the login camera.
/// The baked Login camera must match InterpolationPoint5. The eye is above the
/// grove meshes in front of that point and below the one-sided heightmap sheet.
/// </summary>
public class LoginVistaCameraProofEditModeTests
{
    [Test]
    public void Login_camera_is_InterpolationPoint5_above_the_grove()
    {
        string t3d = File.ReadAllText(Path.Combine(
            Application.dataPath, "Resources/Data/Maps/l2_lobby/Meta/l2_lobby.t3d"));
        Assert.IsTrue(t3d.Contains("Tag=U2Viewport2"), "Camera25 editor viewport tag missing");

        var cam = Regex.Match(
            t3d,
            @"Begin Actor Class=InterpolationPoint Name=InterpolationPoint5\s+"
            + @"Location=\(X=([-\d.]+),Y=([-\d.]+),Z=([-\d.]+)\)\s+"
            + @"Rotation=\(Pitch=(\d+),Yaw=(\d+)\)");
        Assert.IsTrue(cam.Success, "InterpolationPoint5 block missing from l2_lobby.t3d");

        var ue = new Vector3(
            float.Parse(cam.Groups[1].Value, CultureInfo.InvariantCulture),
            float.Parse(cam.Groups[2].Value, CultureInfo.InvariantCulture),
            float.Parse(cam.Groups[3].Value, CultureInfo.InvariantCulture));
        var rot = new Vector3(
            float.Parse(cam.Groups[4].Value, CultureInfo.InvariantCulture),
            float.Parse(cam.Groups[5].Value, CultureInfo.InvariantCulture),
            0f);

        Vector3 unity = VectorUtils.ConvertPosToUnity(ue);
        Vector3 euler = VectorUtils.ConvertRotToUnity(rot);
        float yaw = Mathf.Repeat(euler.y, 360f);
        float pitch = euler.x;
        if (pitch > 180f) pitch -= 360f;
        if (pitch < -180f) pitch += 360f;

        string prefab = File.ReadAllText(Path.Combine(
            Application.dataPath, "Resources/Data/Maps/l2_lobby/Cameras.prefab"));
        int nameAt = prefab.IndexOf("m_Name: Login", System.StringComparison.Ordinal);
        Assert.Greater(nameAt, 0);
        int posAt = prefab.IndexOf("m_LocalPosition:", nameAt, System.StringComparison.Ordinal);
        var pos = Regex.Match(prefab.Substring(posAt, 160),
            @"m_LocalPosition: \{x: ([-\d.]+), y: ([-\d.]+), z: ([-\d.]+)\}");
        Assert.IsTrue(pos.Success);
        Assert.AreEqual(unity.x, float.Parse(pos.Groups[1].Value, CultureInfo.InvariantCulture), 0.02f);
        Assert.AreEqual(unity.y, float.Parse(pos.Groups[2].Value, CultureInfo.InvariantCulture), 0.02f);
        Assert.AreEqual(unity.z, float.Parse(pos.Groups[3].Value, CultureInfo.InvariantCulture), 0.02f);

        Assert.AreEqual(180f, yaw, 0.02f);
        Assert.AreEqual(0.154f, pitch, 0.02f);
        Assert.AreEqual(38.5526f, L2Ue2Fov.VerticalForHorizontal(50f, 4f / 3f), 0.01f);

        float surface = HeightAt(ue.x, ue.y);
        Assert.Greater(surface, ue.z, "the one-sided sheet is above the eye");
        Assert.AreEqual(135f, surface - ue.z, 15f);

        var grass = Regex.Match(
            t3d,
            @"Begin Actor Class=StaticMeshActor Name=StaticMeshActor1353\s+"
            + @"StaticMesh=StaticMesh'[^']+'\s+"
            + @"Location=\(X=([-\d.]+),Y=([-\d.]+),Z=([-\d.]+)\)");
        Assert.IsTrue(grass.Success, "grove grass StaticMeshActor1353 missing");
        float grassZ = float.Parse(grass.Groups[3].Value, CultureInfo.InvariantCulture);
        Assert.Greater(ue.z, grassZ, "the eye is above the grove mesh");
        Assert.AreEqual(68f, ue.z - grassZ, 5f);
    }

    [Test]
    public void Ambient_event_keeps_the_group()
    {
        string[] rain = L2AmbientEventPaths.Candidates("AmbSound", "Weather", "rain_01");
        CollectionAssert.Contains(rain, "event:/AmbSound/AmbSound/Weather/rain_01");
        CollectionAssert.Contains(rain, "event:/AmbSound/AmbSound/rain_01");

        string[] jura = L2AmbientEventPaths.Candidates("AmbSound2", "jura", "jura_01");
        CollectionAssert.Contains(jura, "event:/AmbSound/AmbSound2/jura/jura_01");
        CollectionAssert.Contains(jura, "event:/AmbSound/AmbSound/jura_01");
        Assert.AreEqual(
            "Data/Sounds/AmbSound2/jura/jura_01",
            L2AmbientEventPaths.WaveResource("AmbSound2", "jura", "jura_01"));
        Assert.IsTrue(File.Exists(Path.Combine(
            Application.dataPath, "Resources/Data/Sounds/AmbSound2/jura/jura_01.wav")));
        Assert.IsTrue(File.Exists(Path.Combine(
            Application.dataPath, "Resources/Data/Sounds/AmbSound/Weather/rain_01.wav")));
    }

    static float HeightAt(float ueX, float ueY)
    {
        string bmp = Path.Combine(
            Application.dataPath, "Resources/Data/Textures/lobby_terrain_t/Height.Height.bmp");
        byte[] bytes = File.ReadAllBytes(bmp);
        const float locX = -16384f;
        const float locY = 245760f;
        const float locZ = 160.651276f;
        const float scale = 64f;
        const float half = 256f * scale * 0.5f;
        float ix = (ueX - (locX - half)) / scale;
        float iy = (ueY - (locY - half)) / scale;
        int sx = Mathf.Clamp(Mathf.RoundToInt(ix), 0, 255);
        int sy = 255 - Mathf.Clamp(Mathf.RoundToInt(iy), 0, 255);
        int offset = 54 + (sy * 256 + sx) * 2;
        int sample = bytes[offset] | (bytes[offset + 1] << 8);
        return locZ - 16384f + (sample / 65535f) * 32768f;
    }
}

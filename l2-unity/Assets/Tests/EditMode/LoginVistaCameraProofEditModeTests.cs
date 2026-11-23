using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Camera25 in Lobby.unr is the login vista. The baked Login camera must match
/// the t3d location, rotation, and the UE2 50° horizontal FOV. The heightmap
/// at that cell is above the camera, which is why the lower frame is the
/// culled underside of the sheet rather than a missing BSP floor.
/// </summary>
public class LoginVistaCameraProofEditModeTests
{
    [Test]
    public void Login_camera_matches_Camera25_and_sits_under_the_heightmap()
    {
        string t3d = File.ReadAllText(Path.Combine(
            Application.dataPath, "Resources/Data/Maps/l2_lobby/Meta/l2_lobby.t3d"));
        var cam = Regex.Match(
            t3d,
            @"Begin Actor Class=Camera Name=Camera25\s+"
            + @"Location=\(X=([-\d.]+),Y=([-\d.]+),Z=([-\d.]+)\)\s+"
            + @"Rotation=\(Pitch=(\d+),Yaw=(\d+)\)\s+"
            + @"Tag=U2Viewport2\s+"
            + @"FovAngle=([-\d.]+)");
        Assert.IsTrue(cam.Success, "Camera25 block missing from l2_lobby.t3d");

        var ue = new Vector3(
            float.Parse(cam.Groups[1].Value, CultureInfo.InvariantCulture),
            float.Parse(cam.Groups[2].Value, CultureInfo.InvariantCulture),
            float.Parse(cam.Groups[3].Value, CultureInfo.InvariantCulture));
        var rot = new Vector3(
            float.Parse(cam.Groups[4].Value, CultureInfo.InvariantCulture),
            float.Parse(cam.Groups[5].Value, CultureInfo.InvariantCulture),
            0f);
        float hfov = float.Parse(cam.Groups[6].Value, CultureInfo.InvariantCulture);
        Assert.AreEqual(50f, hfov, 1e-3f);

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
        var pos = Regex.Match(prefab.Substring(posAt, 120),
            @"m_LocalPosition: \{x: ([-\d.]+), y: ([-\d.]+), z: ([-\d.]+)\}");
        Assert.IsTrue(pos.Success);
        Assert.AreEqual(unity.x, float.Parse(pos.Groups[1].Value, CultureInfo.InvariantCulture), 0.02f);
        Assert.AreEqual(unity.y, float.Parse(pos.Groups[2].Value, CultureInfo.InvariantCulture), 0.02f);
        Assert.AreEqual(unity.z, float.Parse(pos.Groups[3].Value, CultureInfo.InvariantCulture), 0.02f);

        float vertical = L2Ue2Fov.VerticalForHorizontal(hfov, 4f / 3f);
        Assert.AreEqual(38.5526f, vertical, 0.01f);
        Assert.AreEqual(185.076f, yaw, 0.02f);
        Assert.AreEqual(9.58f, pitch, 0.02f);

        float surface = HeightAt(ue.x, ue.y);
        Assert.Greater(surface, ue.z, "Camera25 is under the official heightmap at its cell");
        Assert.AreEqual(137f, surface - ue.z, 15f);
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

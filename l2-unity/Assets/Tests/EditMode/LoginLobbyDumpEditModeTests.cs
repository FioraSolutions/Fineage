using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class LoginLobbyDumpEditModeTests
{
    [Test]
    public void LobbyBack_scale_is_draw_scale_times_draw_scale_3d_after_yaw_fix()
    {
        L2Ue2ActorXform.Pose pose = L2Ue2ActorXform.Evaluate(
            new Vector3(-9790f, 238363f, -6520f),
            new Vector3(0f, -49152f, 0f),
            Vector3.zero,
            4f,
            new Vector3(1.3f, 1f, 1.35f),
            true,
            90f,
            Quaternion.identity,
            Vector3.one,
            "StaticMeshActor73");

        Assert.AreEqual(238363f / 52.5f, pose.position.x, 0.02f);
        Assert.AreEqual(-6520f / 52.5f, pose.position.y, 0.02f);
        Assert.AreEqual(-9790f / 52.5f, pose.position.z, 0.02f);
        Assert.AreEqual(5.2f, pose.localScale.x, 0.001f);
        Assert.AreEqual(5.4f, pose.localScale.y, 0.001f);
        Assert.AreEqual(4.0f, pose.localScale.z, 0.001f);
    }

    [Test]
    public void Emitter_scale_uses_the_same_converter_without_the_mesh_yaw_fix()
    {
        L2Ue2ActorXform.Pose pose = L2Ue2ActorXform.Evaluate(
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            2f,
            new Vector3(1f, 2f, 3f),
            true,
            0f,
            Quaternion.identity,
            Vector3.one,
            "emitter");

        Assert.AreEqual(4f, pose.localScale.x, 0.0001f);
        Assert.AreEqual(6f, pose.localScale.y, 0.0001f);
        Assert.AreEqual(2f, pose.localScale.z, 0.0001f);
    }

    [Test]
    public void PrePivot_is_subtracted_in_actor_space()
    {
        L2Ue2ActorXform.Pose pose = L2Ue2ActorXform.Evaluate(
            Vector3.zero,
            Vector3.zero,
            new Vector3(52.5f, 0f, 0f),
            1f,
            Vector3.one,
            false,
            0f,
            Quaternion.identity,
            Vector3.one,
            "pivot");

        Assert.AreEqual(0f, pose.position.x, 0.0001f);
        Assert.AreEqual(0f, pose.position.y, 0.0001f);
        Assert.AreEqual(-1f, pose.position.z, 0.0001f);
    }

    [Test]
    public void Skewed_mesh_basis_logs_and_leaves_that_axis_unscaled()
    {
        LogAssert.Expect(LogType.Error, new Regex("DrawScale X does not land on a single local axis"));
        LogAssert.Expect(LogType.Error, new Regex("DrawScale Y does not land on a single local axis"));
        L2Ue2ActorXform.Pose pose = L2Ue2ActorXform.Evaluate(
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            1f,
            new Vector3(2f, 3f, 4f),
            true,
            0f,
            Quaternion.Euler(0f, 45f, 0f),
            Vector3.one,
            "skew");

        Assert.AreEqual(1f, pose.localScale.x, 0.0001f);
        Assert.AreEqual(4f, pose.localScale.y, 0.0001f);
        Assert.AreEqual(1f, pose.localScale.z, 0.0001f);
    }

    [Test]
    public void Horizontal_fov_50_converts_for_the_retail_aspect()
    {
        float wide = L2Ue2ActorXform.VerticalFovDegrees(50f, 16f / 9f);
        float fourThree = L2Ue2ActorXform.VerticalFovDegrees(50f, 4f / 3f);
        Assert.AreEqual(29.395f, wide, 0.02f);
        Assert.AreEqual(38.553f, fourThree, 0.02f);
        Assert.Greater(Mathf.Abs(wide - 35f), 1f);
    }

    [Test]
    public void Vista_file_matches_the_lobby_dump_and_the_presenter_has_no_fov_35()
    {
        string assets = Application.dataPath;
        string json = File.ReadAllText(Path.Combine(assets, "Resources/Data/Maps/l2_lobby/Meta/LoginVista.json"));
        L2LoginVistaFile file = L2LoginVistaPose.Parse(json);
        Assert.IsNotNull(file);
        Assert.AreEqual("InterpolationPoint5", file.cameraActor);
        Assert.IsFalse(file.actionsObjectArray);
        Assert.AreEqual("ZoneInfo0", file.zoneActor);
        Assert.AreEqual(50f, file.horizontalFov, 0.001f);
        Assert.AreEqual(210, file.fog.r);
        Assert.AreEqual(500f, file.fog.startUu, 0.01f);
        Assert.AreEqual(10000f, file.fog.endUu, 0.01f);
        Assert.AreEqual(64f, file.terrain.scale.x, 0.01f);
        Assert.AreEqual(128f, file.terrain.scale.z, 0.01f);
        Assert.AreEqual(10f, file.terrain.drawScaleIgnored, 0.01f);

        string dump = File.ReadAllText(Path.GetFullPath(Path.Combine(assets, "../docs/interlude-ref/world/env/Lobby.json")));
        StringAssert.Contains("\"name\": \"InterpolationPoint5\"", dump);
        StringAssert.Contains("-8316.73828", dump);
        StringAssert.Contains("Logon_Warp", dump);
        StringAssert.Contains("\"name\": \"ZoneInfo0\"", dump);

        L2Ue2ActorXform.Pose pose = L2LoginVistaPose.CameraPose(file);
        Assert.AreEqual(file.location.y / 52.5f, pose.position.x, 0.01f);
        Assert.AreEqual(file.location.z / 52.5f, pose.position.y, 0.01f);
        Assert.AreEqual(file.location.x / 52.5f, pose.position.z, 0.01f);
        Assert.AreEqual(29.395f, L2LoginVistaPose.VerticalFov(file, 16f / 9f), 0.02f);

        string presenter = File.ReadAllText(Path.Combine(assets, "Scripts/Rendering/LoginForestPresenter.cs"));
        StringAssert.DoesNotContain("const float Fov = 35f", presenter);
        StringAssert.Contains("L2LoginVista.Apply", presenter);
    }

    [Test]
    public void Terrain_sheet_matches_a_far_base_and_stays_above_the_login_eye()
    {
        string assets = Application.dataPath;
        string json = File.ReadAllText(Path.Combine(assets, "Resources/Data/Maps/l2_lobby/Meta/LoginVista.json"));
        L2LoginVistaFile file = L2LoginVistaPose.Parse(json);
        ushort[] samples = ReadHeightBmp(Path.Combine(assets, "Resources/Data/Textures/lobby_terrain_t/Height.Height.bmp"));
        var location = new Vector3(file.terrain.location.x, file.terrain.location.y, file.terrain.location.z);
        var scale = new Vector3(file.terrain.scale.x, file.terrain.scale.y, file.terrain.scale.z);

        Assert.IsTrue(L2Ue2Terrain.TryHeightAt(
            samples, 256, 256, location, scale, -21095.084f, 250931.016f,
            out float farZ, out _, out _, out _));
        Assert.AreEqual(-6053.34863f, farZ, 1f);

        Assert.IsTrue(L2Ue2Terrain.TryHeightAt(
            samples, 256, 256, location, scale, file.location.x, file.location.y,
            out float eyeSheet, out _, out _, out _));
        Assert.Greater(eyeSheet - file.location.z, 50f);

        Vector3 corner = L2Ue2Terrain.UnityCorner(location, scale, file.terrain.sectors);
        float half = 256f * 64f * 0.5f;
        Assert.AreEqual((location.y - half) / 52.5f, corner.x, 0.01f);
        Assert.AreEqual((location.x - half) / 52.5f, corner.z, 0.01f);
    }

    static ushort[] ReadHeightBmp(string path)
    {
        byte[] raw = File.ReadAllBytes(path);
        int offset = System.BitConverter.ToInt32(raw, 10);
        int width = System.BitConverter.ToInt32(raw, 18);
        int height = System.BitConverter.ToInt32(raw, 22);
        Assert.AreEqual(256, width);
        Assert.AreEqual(256, height);
        var samples = new ushort[width * height];
        for (int i = 0; i < samples.Length; i++)
            samples[i] = System.BitConverter.ToUInt16(raw, offset + i * 2);
        return samples;
    }
}

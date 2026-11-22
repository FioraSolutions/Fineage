using System;
using System.IO;
using System.Text.RegularExpressions;
using FMOD;
using FMODUnity;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class LoginVistaBakeEditModeTests
{
    string _dir;

    [SetUp]
    public void SetUp()
    {
        _dir = Path.Combine(Path.GetTempPath(), "fineage-brush-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        L2FmodEditorEventResolver.ResetLookup();
    }

    [TearDown]
    public void TearDown()
    {
        L2FmodEditorEventResolver.ResetLookup();
        if (Directory.Exists(_dir))
        {
            Directory.Delete(_dir, true);
        }
    }

    [Test]
    public void Missing_brush_texture_names_package_and_file()
    {
        string json = WriteBrushJson("FX_E_T.WaterSurfaceShaderSet.WaterShader01");
        var ex = Assert.Throws<InvalidOperationException>(
            () => L2JSONBrushImporter.RequireBrushTexturesPresent(json, _dir));
        StringAssert.Contains("package='FX_E_T'", ex.Message);
        StringAssert.Contains("texture='WaterShader01.png'", ex.Message);
        StringAssert.Contains(Path.Combine(_dir, "FX_E_T", "Texture", "WaterShader01.png"), ex.Message);
    }

    [Test]
    public void Exported_texture_subdirectory_satisfies_the_gate()
    {
        string json = WriteBrushJson(
            "Castle_Kent_t.Kent_capet_m",
            "L2_Skies.Shaders.HazeRing_Final");
        string png = Path.Combine(_dir, "Castle_Kent_t", "Texture", "Kent_capet_m.png");
        Directory.CreateDirectory(Path.GetDirectoryName(png));
        File.WriteAllBytes(png, new byte[] { 0x89 });
        Assert.DoesNotThrow(() => L2JSONBrushImporter.RequireBrushTexturesPresent(json, _dir));
    }

    [Test]
    public void Missing_event_logs_path_and_actor()
    {
        L2FmodEditorEventResolver.Lookup = _ => null;
        LogAssert.Expect(
            LogType.Error,
            new Regex("event missing path=event:/AmbSound/AmbSound/an_wolf_01 actor=AmbientSoundObject9"));
        EventReference er = L2FmodEditorEventResolver.Resolve(
            "event:/AmbSound/AmbSound/an_wolf_01", "AmbientSoundObject9");
        Assert.IsTrue(er.Guid.IsNull);
        Assert.AreEqual("event:/AmbSound/AmbSound/an_wolf_01", er.Path);
    }

    [Test]
    public void Cache_hit_returns_the_editor_guid()
    {
        var guid = new GUID { Data1 = 6, Data2 = 1, Data3 = 5, Data4 = 261 };
        L2FmodEditorEventResolver.Lookup = path =>
            path == "event:/AmbSound/AmbSound/an_wolf_01" ? guid : (GUID?)null;
        EventReference er = L2FmodEditorEventResolver.Resolve(
            "event:/AmbSound/AmbSound/an_wolf_01", "AmbientSoundObject1");
        Assert.AreEqual(guid, er.Guid);
        Assert.AreEqual("event:/AmbSound/AmbSound/an_wolf_01", er.Path);
    }

    string WriteBrushJson(params string[] textures)
    {
        var faces = new string[textures.Length];
        for (int i = 0; i < textures.Length; i++)
        {
            faces[i] = "{\"texture\":\"" + textures[i] + "\"}";
        }
        string json = "{\"data\":[{\"model\":{\"poly\":{\"polyData\":["
            + string.Join(",", faces) + "]}}}]}";
        string path = Path.Combine(_dir, "Brushes.json");
        File.WriteAllText(path, json);
        return path;
    }
}

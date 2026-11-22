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
    public void WaterShader01_is_a_shader_material_not_a_texture_png()
    {
        L2BrushFaceClass.Face face = L2BrushFaceClass.Classify(
            "FX_E_T.WaterSurfaceShaderSet.WaterShader01");
        Assert.AreEqual(L2BrushFaceClass.Kind.Material, face.KindOf);
        Assert.AreEqual("Shader", face.UeClass);
        Assert.AreEqual("WaterSurfaceShaderSet", face.Group);
        Assert.AreEqual("WaterShader01", face.Name);
        // Props are already committed, so the gate does not ask for a PNG.
        string json = WriteBrushJson("FX_E_T.WaterSurfaceShaderSet.WaterShader01");
        Assert.DoesNotThrow(() => L2JSONBrushImporter.RequireBrushTexturesPresent(json, _dir));
    }

    [Test]
    public void Kent_capet_m_is_a_shader_and_the_gate_names_its_class()
    {
        L2BrushFaceClass.Face face = L2BrushFaceClass.Classify("Castle_Kent_t.Kent_capet_m");
        Assert.AreEqual(L2BrushFaceClass.Kind.Material, face.KindOf);
        Assert.AreEqual("Shader", face.UeClass);
        Assert.AreEqual("", face.Group);
        string json = WriteBrushJson("Castle_Kent_t.Kent_capet_m");
        var ex = Assert.Throws<InvalidOperationException>(
            () => L2JSONBrushImporter.RequireBrushTexturesPresent(json, _dir));
        StringAssert.Contains("package=Castle_Kent_t", ex.Message);
        StringAssert.Contains("name=Kent_capet_m", ex.Message);
        StringAssert.Contains("class=Shader", ex.Message);
        Assert.IsFalse(ex.Message.Contains("Kent_capet_m.png"), ex.Message);
    }

    [Test]
    public void Missing_object_names_package_group_and_absent_class()
    {
        string json = WriteBrushJson("Castle_Kent_t.NoSuchFace");
        var ex = Assert.Throws<InvalidOperationException>(
            () => L2JSONBrushImporter.RequireBrushTexturesPresent(json, _dir));
        StringAssert.Contains("package=Castle_Kent_t", ex.Message);
        StringAssert.Contains("name=NoSuchFace", ex.Message);
        StringAssert.Contains("class=absent", ex.Message);
    }

    [Test]
    public void Texture_class_face_still_requires_the_png_when_it_is_not_committed()
    {
        // g_01 is a Texture, and it is committed, so a committed texture passes.
        L2BrushFaceClass.Face face = L2BrushFaceClass.Classify("T_texture.Texture.g_01");
        Assert.AreEqual(L2BrushFaceClass.Kind.Texture, face.KindOf);
        Assert.AreEqual("Texture", face.Group);
        string json = WriteBrushJson("Castle_Kent_t.GL_CA_floortile");
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

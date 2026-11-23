using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

public class L2TerrainLayerUvEditModeTests
{
    [Test]
    public void Missing_scale_names_the_layer()
    {
        var errors = new List<string>();
        Assert.IsFalse(L2TerrainLayerUv.TryRepeat(0f, 32f, 1f, 1f, 0f, 0f, "T_oren.Obase_1", errors, out _));
        Assert.AreEqual(1, errors.Count);
        StringAssert.Contains("Obase_1", errors[0]);
        StringAssert.Contains("Scale", errors[0]);
    }

    [Test]
    public void Missing_uscale_is_not_replaced_with_one()
    {
        var errors = new List<string>();
        Assert.IsFalse(L2TerrainLayerUv.TryRepeat(32f, 32f, 0f, 1f, 0f, 0f, "T_sland.SL_S3", errors, out _));
        StringAssert.Contains("UScale", errors[0]);
        StringAssert.Contains("SL_S3", errors[0]);
    }

    [Test]
    public void Repeat_is_layer_scale_times_uscale()
    {
        var errors = new List<string>();
        Assert.IsTrue(L2TerrainLayerUv.TryRepeat(32f, 32f, 2f, 2f, -0.439f, 0.046f, "SL_WR", errors, out Vector4 repeat));
        Assert.AreEqual(0, errors.Count);
        Assert.AreEqual(64f, repeat.x, 1e-3f);
        Assert.AreEqual(64f, repeat.y, 1e-3f);
        Assert.AreEqual(-0.439f, repeat.z, 1e-4f);
        Assert.AreEqual(0.046f, repeat.w, 1e-4f);

        Vector2 uv = L2TerrainLayerUv.Sample(128f, 64f, 9f, repeat, 0f, 0);
        Assert.AreEqual(128f / 64f + repeat.z, uv.x, 1e-3f);
        Assert.AreEqual(64f / 64f + repeat.w, uv.y, 1e-3f);
    }

    [Test]
    public void Vertical_axes_use_world_height()
    {
        var repeat = new Vector4(32f, 32f, 0f, 0f);
        Vector2 xz = L2TerrainLayerUv.Sample(64f, 999f, 32f, repeat, 0f, 1);
        Assert.AreEqual(2f, xz.x, 1e-3f);
        Assert.AreEqual(1f, xz.y, 1e-3f);
        Vector2 yz = L2TerrainLayerUv.Sample(999f, 32f, 64f, repeat, 0f, 2);
        Assert.AreEqual(1f, yz.x, 1e-3f);
        Assert.AreEqual(2f, yz.y, 1e-3f);
    }

    [Test]
    public void Quarter_turn_rotates_the_repeat()
    {
        Vector2 uv = L2TerrainLayerUv.Sample(32f, 0f, 0f, new Vector4(32f, 32f, 0f, 0f), 16384f, 0);
        Assert.AreEqual(0f, uv.x, 1e-3f);
        Assert.AreEqual(1f, uv.y, 1e-3f);
    }

    [Test]
    public void Talking_island_mountain_overlay_repeats_every_32_uu()
    {
        List<LayerRow> layers = ReadLayers("17_25");
        LayerRow rock = Find(layers, "T_sland.SL_S3");
        Assert.AreEqual(32f, rock.RepeatU, 1e-3f);
        Assert.AreEqual(32f, rock.RepeatV, 1e-3f);
        LayerRow shore = Find(layers, "T_sland.SL_WR");
        Assert.AreEqual(64f, shore.RepeatU, 1e-3f);
        LayerRow baseDirt = Find(layers, "T_texture.Texture.Base");
        Assert.AreEqual(128f, baseDirt.RepeatU, 1e-3f);
        AssertMat("17_25", layers);
    }

    [Test]
    public void Gludio_obase_repeats_every_32_uu()
    {
        List<LayerRow> layers = ReadLayers("19_21");
        LayerRow rock = Find(layers, "T_oren.Obase_1");
        Assert.AreEqual(32f, rock.RepeatU, 1e-3f);
        Assert.AreEqual(32f, rock.RepeatV, 1e-3f);
        Assert.AreEqual(1, rock.Index);
        LayerRow cliff = Find(layers, "T_Gludio.GUC01");
        Assert.AreEqual(64f, cliff.RepeatU, 1e-3f);
        AssertMat("19_21", layers);
    }

    [Test]
    public void Every_shipped_terrain_mat_uses_the_official_repeat()
    {
        string maps = Path.Combine(Application.dataPath, "Resources/Data/Maps");
        int tiles = 0;
        foreach (string info in Directory.GetFiles(maps, "TerrainInfo0.txt", SearchOption.AllDirectories))
        {
            string tile = new DirectoryInfo(Path.GetDirectoryName(info)).Parent.Name;
            List<LayerRow> layers = ReadLayers(tile);
            Assert.LessOrEqual(layers.Count, L2TerrainLayerUv.ShaderLayerCap, tile);
            AssertMat(tile, layers);
            tiles++;
        }
        Assert.GreaterOrEqual(tiles, 40);
    }

    static void AssertMat(string tile, List<LayerRow> layers)
    {
        string matPath = Path.Combine(Application.dataPath, "Resources/Data/Maps", tile, tile + "_Mat.mat");
        Assert.IsTrue(File.Exists(matPath), matPath);
        string text = File.ReadAllText(matPath);
        for (int i = 0; i < layers.Count; i++)
        {
            LayerRow row = layers[i];
            Vector4 uv = ReadVec(text, "_Layer_" + i + "_UV");
            Assert.AreEqual(row.RepeatU, uv.x, 1e-3f, tile + " layer " + i + " " + row.Texture + " U");
            Assert.AreEqual(row.RepeatV, uv.y, 1e-3f, tile + " layer " + i + " " + row.Texture + " V");
            Assert.AreEqual(row.UPan, uv.z, 1e-3f, tile + " layer " + i + " pan U");
            Assert.AreEqual(row.VPan, uv.w, 1e-3f, tile + " layer " + i + " pan V");
            Vector4 xform = ReadVec(text, "_Layer_" + i + "_Xform");
            Assert.AreEqual(row.Rotation, xform.x, 1e-2f, tile + " layer " + i + " rotation");
            Assert.AreEqual(row.Axis, xform.y, 1e-3f, tile + " layer " + i + " axis");
        }
    }

    static Vector4 ReadVec(string text, string key)
    {
        var m = Regex.Match(text, key + @"_?: \{r: ([-0-9.]+), g: ([-0-9.]+), b: ([-0-9.]+), a: ([-0-9.]+)\}");
        Assert.IsTrue(m.Success, key);
        return new Vector4(
            float.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture),
            float.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture),
            float.Parse(m.Groups[3].Value, CultureInfo.InvariantCulture),
            float.Parse(m.Groups[4].Value, CultureInfo.InvariantCulture));
    }

    static LayerRow Find(List<LayerRow> layers, string texture)
    {
        for (int i = 0; i < layers.Count; i++)
        {
            if (layers[i].Texture == texture)
                return layers[i];
        }
        Assert.Fail("missing " + texture);
        return null;
    }

    static List<LayerRow> ReadLayers(string tile)
    {
        string path = Path.Combine(Application.dataPath, "Resources/Data/Maps", tile, "Meta", "TerrainInfo0.txt");
        var layers = new List<LayerRow>();
        foreach (string raw in File.ReadAllLines(path))
        {
            string line = raw.Trim();
            if (!line.StartsWith("Layers("))
                continue;
            if (line.IndexOf("Texture=None", System.StringComparison.OrdinalIgnoreCase) >= 0)
                continue;
            var tex = Regex.Match(line, @"Texture=Texture'([^']+)'");
            if (!tex.Success)
                continue;
            var scale = Regex.Match(line, @"Scale=\(X=([-0-9.]+),Y=([-0-9.]+)");
            var us = Regex.Match(line, @"UScale=([-0-9.]+)");
            var vs = Regex.Match(line, @"VScale=([-0-9.]+)");
            Assert.IsTrue(scale.Success && us.Success && vs.Success, tile + " " + tex.Groups[1].Value);
            float scaleX = float.Parse(scale.Groups[1].Value, CultureInfo.InvariantCulture);
            float scaleY = float.Parse(scale.Groups[2].Value, CultureInfo.InvariantCulture);
            float uScale = float.Parse(us.Groups[1].Value, CultureInfo.InvariantCulture);
            float vScale = float.Parse(vs.Groups[1].Value, CultureInfo.InvariantCulture);
            float uPan = MatchOrZero(line, @"UPan=([-0-9.]+)");
            float vPan = MatchOrZero(line, @"VPan=([-0-9.]+)");
            var errors = new List<string>();
            Assert.IsTrue(
                L2TerrainLayerUv.TryRepeat(scaleX, scaleY, uScale, vScale, uPan, vPan, tex.Groups[1].Value, errors, out Vector4 repeat),
                string.Join("; ", errors.ToArray()));
            var rot = Regex.Match(line, @"TextureRotation=([-0-9.]+)");
            var axis = Regex.Match(line, @"TextureMapAxis=(\w+)");
            int axisId = 0;
            if (axis.Success)
            {
                if (axis.Groups[1].Value == "TEXMAPAXIS_XZ")
                    axisId = 1;
                else if (axis.Groups[1].Value == "TEXMAPAXIS_YZ")
                    axisId = 2;
            }
            layers.Add(new LayerRow
            {
                Index = layers.Count,
                Texture = tex.Groups[1].Value,
                RepeatU = repeat.x,
                RepeatV = repeat.y,
                UPan = repeat.z,
                VPan = repeat.w,
                Rotation = rot.Success ? float.Parse(rot.Groups[1].Value, CultureInfo.InvariantCulture) : 0f,
                Axis = axisId,
            });
        }
        return layers;
    }

    static float MatchOrZero(string line, string pattern)
    {
        var m = Regex.Match(line, pattern);
        return m.Success ? float.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture) : 0f;
    }

    sealed class LayerRow
    {
        public int Index;
        public string Texture;
        public float RepeatU;
        public float RepeatV;
        public float UPan;
        public float VPan;
        public float Rotation;
        public int Axis;
    }
}

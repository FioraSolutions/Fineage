using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Shader Graphs/TerrainAlphamapDouble samples _Layer_0 as the opaque base and
/// blends overlay slices on top. An empty slot is the graph default color, so
/// every shipped tile must name the official TerrainInfo layer-0 diffuse.
/// </summary>
public class L2TerrainBaseDiffuseEditModeTests
{
    [Test]
    public void Every_tile_binds_the_official_layer0_diffuse()
    {
        string maps = Path.Combine(Application.dataPath, "Resources/Data/Maps");
        string textures = Path.Combine(Application.dataPath, "Resources/Data/Textures");
        int tiles = 0;
        foreach (string info in Directory.GetFiles(maps, "TerrainInfo0.txt", SearchOption.AllDirectories))
        {
            string tile = new DirectoryInfo(Path.GetDirectoryName(info)).Parent.Name;
            string layer0 = ReadLayer0(info);
            string guid = GuidForRef(textures, layer0);
            string matPath = Path.Combine(maps, tile, tile + "_Mat.mat");
            string mat = File.ReadAllText(matPath);
            var slot = Regex.Match(mat, @"- _Layer_0:\s+m_Texture:\s+\{([^}]+)\}");
            Assert.IsTrue(slot.Success, tile + " has no _Layer_0 slot");
            StringAssert.Contains("guid: " + guid, slot.Groups[1].Value, tile + " " + layer0);
            tiles++;
        }
        Assert.GreaterOrEqual(tiles, 40);
    }

    [Test]
    public void Empty_layer0_default_is_not_white()
    {
        string graph = Path.Combine(Application.dataPath, "Resources/Data/Shaders/TerrainAlphamapDouble.shadergraph");
        string text = File.ReadAllText(graph);
        int name = text.IndexOf("\"m_Name\": \"Layer_0\"");
        Assert.GreaterOrEqual(name, 0);
        int type = text.IndexOf("\"m_DefaultType\":", name);
        Assert.GreaterOrEqual(type, 0);
        Assert.Less(type, name + 800);
        var value = Regex.Match(text.Substring(type), @"m_DefaultType"":\s*(\d+)");
        Assert.IsTrue(value.Success);
        // Texture2DShaderProperty.DefaultType: 0 white, 1 black.
        Assert.AreNotEqual("0", value.Groups[1].Value, "empty _Layer_0 must not sample as white");
    }

    static string ReadLayer0(string infoPath)
    {
        foreach (string raw in File.ReadAllLines(infoPath))
        {
            string line = raw.Trim();
            if (!line.StartsWith("Layers(0)="))
                continue;
            var tex = Regex.Match(line, @"Texture=Texture'([^']+)'");
            Assert.IsTrue(tex.Success, infoPath);
            return tex.Groups[1].Value;
        }
        Assert.Fail("no layer 0 in " + infoPath);
        return null;
    }

    static string GuidForRef(string texturesRoot, string textureRef)
    {
        string[] parts = textureRef.Split('.');
        Assert.GreaterOrEqual(parts.Length, 2, textureRef);
        string folder = parts[0];
        string stem = parts.Length >= 3 ? parts[2] : parts[1];
        string dir = null;
        foreach (string child in Directory.GetDirectories(texturesRoot))
        {
            if (string.Equals(Path.GetFileName(child), folder, System.StringComparison.OrdinalIgnoreCase))
            {
                dir = child;
                break;
            }
        }
        Assert.IsNotNull(dir, textureRef + " package folder missing");
        string png = null;
        foreach (string file in Directory.GetFiles(dir, "*.png"))
        {
            if (string.Equals(Path.GetFileNameWithoutExtension(file), stem, System.StringComparison.OrdinalIgnoreCase))
            {
                png = file;
                break;
            }
        }
        Assert.IsNotNull(png, textureRef + " png missing under " + dir);
        foreach (string line in File.ReadAllLines(png + ".meta"))
        {
            if (line.StartsWith("guid:"))
                return line.Substring("guid:".Length).Trim();
        }
        Assert.Fail("no guid for " + png);
        return null;
    }
}

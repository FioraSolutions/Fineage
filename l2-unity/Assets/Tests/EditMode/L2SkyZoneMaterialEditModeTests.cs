using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Fineage.TerrainBsp;
using NUnit.Framework;
using UnityEngine;

public class L2SkyZoneMaterialEditModeTests
{
    [Test]
    public void Sky_brush_textures_are_sky_packages()
    {
        string maps = Path.Combine(Application.dataPath, "Resources/Data/Maps");
        int brushes = 0;
        var perTile = new Dictionary<string, int>();
        foreach (string t3d in Directory.GetFiles(maps, "*.t3d", SearchOption.AllDirectories))
        {
            string tile = new DirectoryInfo(Path.GetDirectoryName(t3d)).Parent.Name;
            string text = File.ReadAllText(t3d);
            string[] actors = text.Split(new[] { "Begin Actor " }, System.StringSplitOptions.None);
            for (int i = 1; i < actors.Length; i++)
            {
                string actor = actors[i];
                if (!actor.StartsWith("Class=Brush"))
                    continue;
                var textures = new List<string>();
                foreach (Match m in Regex.Matches(actor, @"Begin Polygon Texture=([A-Za-z0-9_.]+)"))
                    textures.Add(m.Groups[1].Value);
                if (textures.Count == 0)
                    continue;
                bool sky = false;
                var loc = Regex.Match(actor, @"Location=\(X=[-0-9.]+,Y=[-0-9.]+,Z=([-0-9.]+)\)");
                if (loc.Success && float.Parse(loc.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture) > 20000f)
                    sky = true;
                for (int t = 0; t < textures.Count; t++)
                {
                    if (L2BspPolyFlags.IsSkyZoneTexture(textures[t]))
                        sky = true;
                }
                if (!sky)
                    continue;
                brushes++;
                if (!perTile.ContainsKey(tile))
                    perTile[tile] = 0;
                perTile[tile]++;
                for (int t = 0; t < textures.Count; t++)
                {
                    string tex = textures[t];
                    Assert.IsTrue(L2BspPolyFlags.IsSkyZoneTexture(tex), tile + " sky brush texture " + tex);
                    Assert.IsFalse(L2BspPolyFlags.IsBrushTextureRenderable(tex), tile + " " + tex);
                }
            }
        }
        Assert.Greater(brushes, 0);
        Assert.IsTrue(perTile.ContainsKey("17_25") && perTile["17_25"] > 0, "Talking Island sky brushes");
        Assert.IsTrue(perTile.ContainsKey("19_21") && perTile["19_21"] > 0, "Gludio sky brushes");
    }

    [Test]
    public void Terrain_diffuse_is_not_a_sky_texture()
    {
        Assert.IsFalse(L2BspPolyFlags.IsSkyZoneTexture("T_texture.Texture.Base"));
        Assert.IsFalse(L2BspPolyFlags.IsSkyZoneTexture("T_oren.Obase_1"));
        Assert.IsTrue(L2BspPolyFlags.IsBrushTextureRenderable("T_oren.Obase_1"));
        Assert.IsTrue(L2BspPolyFlags.IsSkyZoneTexture("L2_Skies.Shaders.HazeRing_Final"));
    }

    [Test]
    public void Every_zone_has_a_sky_zone_and_flares_are_not_terrain()
    {
        string path = Path.Combine(Application.dataPath, "../docs/interlude-ref/world/zone-render.tsv");
        var maps = new HashSet<string>();
        var sky = new HashSet<string>();
        foreach (string raw in File.ReadAllLines(path))
        {
            if (raw.Length == 0 || raw.StartsWith("map\t"))
                continue;
            string[] cells = raw.Split('\t');
            if (cells.Length < 2)
                continue;
            maps.Add(cells[0]);
            if (cells[1] != "SkyZoneInfo")
                continue;
            sky.Add(cells[0]);
            if (cells.Length <= 14 || cells[14].Length == 0)
                continue;
            string[] flares = cells[14].Split('|');
            for (int i = 0; i < flares.Length; i++)
            {
                string token = flares[i];
                if (token.Length == 0)
                    continue;
                string package = token.Split('.')[0];
                Assert.IsTrue(package == "Engine" || package == "L2_Skies", cells[0] + " flare " + token);
                Assert.IsFalse(L2BspPolyFlags.IsBrushTextureRenderable(token), cells[0] + " " + token);
            }
        }
        var missing = new List<string>();
        foreach (string map in maps)
        {
            if (!sky.Contains(map))
                missing.Add(map);
        }
        missing.Sort(System.StringComparer.Ordinal);
        CollectionAssert.AreEqual(new[] { "entry", "ship_position" }, missing.ToArray());
    }
}

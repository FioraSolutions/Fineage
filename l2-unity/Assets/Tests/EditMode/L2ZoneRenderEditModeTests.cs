using System.Collections.Generic;
using System.Globalization;
using System.IO;
using NUnit.Framework;
using UnityEngine;

public class L2ZoneRenderEditModeTests
{
    const float Uu = 1f / 52.5f;

    [Test]
    public void Catalog_copies_match_and_parse()
    {
        string docs = File.ReadAllText(Path.Combine(Application.dataPath, "../docs/interlude-ref/world/zone-render.tsv"));
        string runtime = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "Data/Meta/World/zone-render.tsv"));
        Assert.AreEqual(docs, runtime);
        var errors = new List<string>();
        L2ZoneRenderTable table = L2ZoneRenderTable.Parse(docs, errors);
        Assert.AreEqual(0, errors.Count);
        Assert.IsTrue(table.Actors.Count > 1000);
    }

    [Test]
    public void Dion_terrain_fog_uses_authored_start_and_end()
    {
        L2ZoneRenderTable table = Load();
        var errors = new List<string>();
        Assert.IsTrue(table.TryDistanceFog("20_22", out L2ZoneRenderTable.DistanceFog fog, errors));
        Assert.AreEqual(0, errors.Count);
        Assert.AreEqual("ZoneInfo1", fog.Actor);
        Assert.IsTrue(fog.HasStart);
        Assert.IsTrue(fog.HasEnd);
        Assert.IsFalse(fog.HasColor);
        Assert.AreEqual(1500f, fog.StartUu, 1e-3f);
        Assert.AreEqual(10000f, fog.EndUu, 1e-3f);
        L2ZoneRenderTable.DistanceFogGlobals(fog, Uu, out float endMeters, out float scale, out bool enable);
        Assert.IsTrue(enable);
        Assert.AreEqual(10000f * Uu, endMeters, 1e-4f);
        Assert.AreEqual(1f / ((10000f - 1500f) * Uu), scale, 1e-4f);
    }

    [Test]
    public void Giran_selects_terrain_fog_and_keeps_the_holy_zone_in_the_table()
    {
        L2ZoneRenderTable table = Load();
        var errors = new List<string>();
        Assert.IsTrue(table.TryDistanceFog("22_22", out L2ZoneRenderTable.DistanceFog fog, errors));
        Assert.AreEqual("ZoneInfo3", fog.Actor);
        Assert.IsFalse(fog.HasStart);
        Assert.AreEqual(15000f, fog.EndUu, 1e-3f);
        Assert.AreEqual(1, errors.Count);
        Assert.IsTrue(errors[0].Contains("22_22"));
        Assert.IsTrue(errors[0].Contains("ZoneInfo3"));
        Assert.IsTrue(errors[0].Contains("DistanceFogStart"));

        bool holy = false;
        for (int i = 0; i < table.Actors.Count; i++)
        {
            L2ZoneRenderTable.Actor actor = table.Actors[i];
            if (actor.Map != "22_22" || actor.ZoneTag != "holy")
                continue;
            holy = true;
            Assert.AreEqual(500f, float.Parse(actor.FogStart, CultureInfo.InvariantCulture), 1e-3f);
            Assert.AreEqual(16000f, float.Parse(actor.FogEnd, CultureInfo.InvariantCulture), 1e-3f);
        }
        Assert.IsTrue(holy);
    }

    [Test]
    public void Fog_exceptions_come_from_the_catalog_rows()
    {
        L2ZoneRenderTable table = Load();
        var errors = new List<string>();
        Assert.IsTrue(table.TryDistanceFog("22_21", out L2ZoneRenderTable.DistanceFog south, errors));
        Assert.AreEqual(20000f, south.EndUu, 1e-3f);
        Assert.IsTrue(south.HasColor);
        Assert.AreEqual(121, south.R);
        Assert.AreEqual(120, south.G);
        Assert.AreEqual(135, south.B);

        errors.Clear();
        Assert.IsTrue(table.TryDistanceFog("21_21", out L2ZoneRenderTable.DistanceFog giran, errors));
        Assert.AreEqual(10000f, giran.EndUu, 1e-3f);
        Assert.IsFalse(giran.HasColor);

        errors.Clear();
        Assert.IsTrue(table.TryDistanceFog("17_25", out L2ZoneRenderTable.DistanceFog ti, errors));
        Assert.AreEqual("ZoneInfo1", ti.Actor);
        Assert.AreEqual(15000f, ti.EndUu, 1e-3f);
        Assert.AreEqual(174, ti.R);
        Assert.AreEqual(172, ti.G);
        Assert.AreEqual(164, ti.B);
        Assert.IsTrue(errors[0].Contains("DistanceFogStart"));
    }

    [Test]
    public void Sky_pan_and_sun_radius_are_the_dump_values()
    {
        L2ZoneRenderTable table = Load();
        var errors = new List<string>();
        Assert.IsTrue(table.TrySkyPan("20_22", out float u, out float v, errors));
        Assert.AreEqual(0, errors.Count);
        Assert.AreEqual(0.2f, u, 1e-6f);
        Assert.AreEqual(0.2f, v, 1e-6f);
        Assert.AreEqual("0.200000003", Sky(table, "20_22").PanU);
        Assert.AreEqual("350.0", Sun(table, "20_22").Radius);
        Assert.AreEqual(1f, table.BodyRadiusScale("20_22", "NSun", errors), 1e-5f);
        Assert.AreEqual(0, errors.Count);
        string[] flares = table.LensFlares("20_22");
        Assert.IsTrue(flares.Length > 0);
        Assert.AreEqual("Engine.Flare02", flares[0]);
    }

    [Test]
    public void Missing_map_is_an_error()
    {
        L2ZoneRenderTable table = Load();
        var errors = new List<string>();
        Assert.IsFalse(table.TryDistanceFog("no_such_tile", out _, errors));
        Assert.AreEqual(1, errors.Count);
        Assert.IsTrue(errors[0].Contains("no_such_tile"));
        Assert.IsTrue(errors[0].Contains("missing"));
    }

    [Test]
    public void Selection_prefers_terrain_fog_and_radius_scale_is_a_ratio()
    {
        string text = Header() + "\n" +
            ZoneRow("a", "ZoneInfo", "inner", "holy", "1", "", "500", "16000", "1", "2", "3", "0", "", "", "") + "\n" +
            ZoneRow("a", "ZoneInfo", "ground", "", "1", "1", "", "9000", "", "", "", "", "", "", "") + "\n" +
            ZoneRow("b", "ZoneInfo", "one", "", "1", "1", "", "1000", "", "", "", "", "", "", "") + "\n" +
            ZoneRow("b", "ZoneInfo", "two", "", "1", "1", "", "2000", "", "", "", "", "", "", "") + "\n" +
            ZoneRow("c", "NSun", "NSun0", "", "", "", "", "", "", "", "", "", "", "", "350") + "\n" +
            ZoneRow("c", "NSun", "NSun1", "", "", "", "", "", "", "", "", "", "", "", "350") + "\n" +
            ZoneRow("d", "NSun", "NSun0", "", "", "", "", "", "", "", "", "", "", "", "350") + "\n" +
            ZoneRow("e", "NSun", "NSun0", "", "", "", "", "", "", "", "", "", "", "", "700") + "\n" +
            ZoneRow("e", "SkyZoneInfo", "SkyZoneInfo0", "", "", "", "", "", "", "", "", "", "0.2", "0.2", "") + "\n" +
            ZoneRow("e", "SkyZoneInfo", "SkyZoneInfo1", "", "", "", "", "", "", "", "", "", "0.2", "0.2", "") + "\n";
        var errors = new List<string>();
        L2ZoneRenderTable table = L2ZoneRenderTable.Parse(text, errors);
        Assert.AreEqual(0, errors.Count);
        Assert.IsTrue(table.TryDistanceFog("a", out L2ZoneRenderTable.DistanceFog fog, errors));
        Assert.AreEqual("ground", fog.Actor);
        Assert.AreEqual(9000f, fog.EndUu, 1e-3f);
        Assert.IsTrue(errors[0].Contains("DistanceFogStart"));

        errors.Clear();
        Assert.IsFalse(table.TryDistanceFog("b", out _, errors));
        Assert.IsTrue(errors[0].Contains("ambiguous"));
        Assert.IsTrue(errors[0].Contains("one"));
        Assert.IsTrue(errors[0].Contains("two"));

        errors.Clear();
        Assert.AreEqual(2f, table.BodyRadiusScale("e", "NSun", errors), 1e-5f);
        Assert.AreEqual(0, errors.Count);
        Assert.IsTrue(table.TrySkyPan("e", out float panU, out float panV, errors));
        Assert.AreEqual(0.2f, panU, 1e-5f);
        Assert.AreEqual(0.2f, panV, 1e-5f);
        Assert.AreEqual(1f, table.BodyRadiusScale("c", "NSun", errors), 1e-5f);
    }

    static L2ZoneRenderTable Load()
    {
        string docs = File.ReadAllText(Path.Combine(Application.dataPath, "../docs/interlude-ref/world/zone-render.tsv"));
        var errors = new List<string>();
        L2ZoneRenderTable table = L2ZoneRenderTable.Parse(docs, errors);
        Assert.AreEqual(0, errors.Count);
        return table;
    }

    static L2ZoneRenderTable.Actor Sky(L2ZoneRenderTable table, string map)
    {
        return Find(table, map, "SkyZoneInfo");
    }

    static L2ZoneRenderTable.Actor Sun(L2ZoneRenderTable table, string map)
    {
        return Find(table, map, "NSun");
    }

    static L2ZoneRenderTable.Actor Find(L2ZoneRenderTable table, string map, string className)
    {
        for (int i = 0; i < table.Actors.Count; i++)
        {
            if (table.Actors[i].Map == map && table.Actors[i].ClassName == className)
                return table.Actors[i];
        }
        Assert.Fail(map + " " + className);
        return default;
    }

    static string Header()
    {
        return "map\tclass\tname\tzone_tag\tb_distance_fog\tb_terrain_zone\tfog_start\tfog_end\tfog_r\tfog_g\tfog_b\tfog_a\ttex_u_pan\ttex_v_pan\tlens_flares\tlens_offsets\tlens_scales\tradius\tlimit_max_radius\tb_directional\tdraw_scale\tlight_brightness\trot_pitch\trot_yaw\trot_roll";
    }

    static string ZoneRow(string map, string cls, string name, string tag, string fog, string terrain, string start, string end, string r, string g, string b, string a, string panU, string panV, string radius)
    {
        var cells = new string[25];
        for (int i = 0; i < cells.Length; i++)
            cells[i] = "";
        cells[0] = map;
        cells[1] = cls;
        cells[2] = name;
        cells[3] = tag;
        cells[4] = fog;
        cells[5] = terrain;
        cells[6] = start;
        cells[7] = end;
        cells[8] = r;
        cells[9] = g;
        cells[10] = b;
        cells[11] = a;
        cells[12] = panU;
        cells[13] = panV;
        cells[17] = radius;
        return string.Join("\t", cells);
    }
}

using System.Text;
using NUnit.Framework;
using UnityEngine;

public class OfficialWorldEmitterEditModeTests
{
    [SetUp]
    public void SetUp()
    {
        OfficialWorldEmitterTable.ClearForTests();
    }

    [TearDown]
    public void TearDown()
    {
        OfficialWorldEmitterTable.ClearForTests();
    }

    [Test]
    public void Table_loads_official_placement_and_converts_pose()
    {
        const string json = "{\"emitterCount\":1,\"prefabClasses\":1,\"emitters\":[{\"map\":\"17_25\",\"name\":\"Emitter2\",\"tileX\":17,\"tileY\":25,\"location\":{\"x\":-83337.1953,\"y\":243070.703,\"z\":-3179.0},\"rotation\":{\"x\":1472,\"y\":0,\"z\":0},\"drawScale\":2.0,\"drawScale3D\":{\"x\":1,\"y\":1,\"z\":1},\"hasDrawScale3D\":false,\"prefabClass\":\"we_test\",\"spawnSounds\":[],\"soundRadius\":0,\"soundVolume\":0,\"visibleLimitMin\":0,\"visibleLimitMax\":0,\"hasVisibleLimit\":false,\"moveEvent\":\"\",\"hasGameEvent\":false}]}";
        OfficialWorldEmitterTable.LoadFrom(Encoding.UTF8.GetBytes(json), "test://emitters");
        Assert.AreEqual(1, OfficialWorldEmitterTable.Count);
        OfficialWorldEmitterTable.Emitter e = OfficialWorldEmitterTable.ForMap("17_25")[0];
        Assert.AreEqual("Data/Effects/world/we_test/we_test", e.ResourcesPath);
        Assert.AreEqual(VectorUtils.ConvertPosToUnity(new Vector3(-83337.1953f, 243070.703f, -3179f)), e.UnityPosition);
        Assert.AreEqual(Vector3.one * 2f, e.UnityScale);
    }

    [Test]
    public void Missing_table_logs_path()
    {
        string logged = null;
        OfficialWorldEmitterTable.ErrorLogged = msg => logged = msg;
        OfficialWorldEmitterTable.LoadFrom(null, "test://missing.json");
        Assert.IsNotNull(logged);
        Assert.IsTrue(logged.Contains("test://missing.json"), logged);
        Assert.AreEqual(0, OfficialWorldEmitterTable.Count);
    }

    [Test]
    public void Nearby_search_includes_neighbor_tiles()
    {
        const string json = "{\"emitters\":[{\"map\":\"16_25\",\"name\":\"A\",\"tileX\":16,\"tileY\":25,\"location\":{\"x\":0,\"y\":0,\"z\":0},\"rotation\":{\"x\":0,\"y\":0,\"z\":0},\"drawScale\":1,\"drawScale3D\":{\"x\":1,\"y\":1,\"z\":1},\"prefabClass\":\"we_a\"},{\"map\":\"18_26\",\"name\":\"B\",\"tileX\":18,\"tileY\":26,\"location\":{\"x\":0,\"y\":0,\"z\":0},\"rotation\":{\"x\":0,\"y\":0,\"z\":0},\"drawScale\":1,\"drawScale3D\":{\"x\":1,\"y\":1,\"z\":1},\"prefabClass\":\"we_b\"},{\"map\":\"19_25\",\"name\":\"C\",\"tileX\":19,\"tileY\":25,\"location\":{\"x\":0,\"y\":0,\"z\":0},\"rotation\":{\"x\":0,\"y\":0,\"z\":0},\"drawScale\":1,\"drawScale3D\":{\"x\":1,\"y\":1,\"z\":1},\"prefabClass\":\"we_c\"}]}";
        OfficialWorldEmitterTable.LoadFrom(Encoding.UTF8.GetBytes(json), "test://nearby");
        var dest = new System.Collections.Generic.List<OfficialWorldEmitterTable.Emitter>();
        OfficialWorldEmitterTable.CollectNearby(17, 25, null, dest);
        Assert.AreEqual(2, dest.Count);
    }
}

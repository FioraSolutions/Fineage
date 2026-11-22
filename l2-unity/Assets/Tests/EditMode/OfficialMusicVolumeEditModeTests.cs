using System.Text;
using NUnit.Framework;
using UnityEngine;

public class OfficialMusicVolumeEditModeTests
{
    [SetUp]
    public void SetUp()
    {
        OfficialMusicInfo.ClearForTests();
        OfficialMusicBankEvents.ClearForTests();
        OfficialMusicVolumeTable.ClearForTests();
    }

    [TearDown]
    public void TearDown()
    {
        OfficialMusicInfo.ClearForTests();
        OfficialMusicBankEvents.ClearForTests();
        OfficialMusicVolumeTable.ClearForTests();
    }

    [Test]
    public void Musicinfo_and_bank_resolve_official_speaking_track()
    {
        OfficialMusicInfo.LoadFrom("id\tcnt\ttrack\n172501\t1\tNT_Speaking\n", "test://musicinfo");
        OfficialMusicBankEvents.LoadFrom(
            "track\tevent_path\nNT_Speaking\tevent:/Music/l2_lobby/nt_speaking\n",
            "test://bank");
        Assert.AreEqual("NT_Speaking", OfficialMusicInfo.TracksFor(172501)[0]);
        Assert.IsTrue(OfficialMusicBankEvents.TryResolve("NT_Speaking", out string path, out _));
        Assert.AreEqual("event:/Music/l2_lobby/nt_speaking", path);
        Assert.IsFalse(OfficialMusicBankEvents.TryResolve("F07_F", out _, out string error));
        Assert.IsTrue(error.Contains("track=\"F07_F\""), error);
    }

    [Test]
    public void Point_in_official_brush_uses_world_space_polys()
    {
        var verts = new[]
        {
            new Vector3(0, 0, 0),
            new Vector3(10, 0, 0),
            new Vector3(10, 10, 0),
            new Vector3(0, 10, 0)
        };
        // six axis-aligned box faces would be better; use a unit cube via two tests of AABB+planes
        var polys = new[]
        {
            new OfficialBrushSolid.Poly(Vector3.back, verts),
            new OfficialBrushSolid.Poly(Vector3.forward, new[]
            {
                new Vector3(0, 0, 10), new Vector3(0, 10, 10), new Vector3(10, 10, 10), new Vector3(10, 0, 10)
            }),
            new OfficialBrushSolid.Poly(Vector3.left, new[]
            {
                new Vector3(0, 0, 0), new Vector3(0, 0, 10), new Vector3(0, 10, 10), new Vector3(0, 10, 0)
            }),
            new OfficialBrushSolid.Poly(Vector3.right, new[]
            {
                new Vector3(10, 0, 0), new Vector3(10, 10, 0), new Vector3(10, 10, 10), new Vector3(10, 0, 10)
            }),
            new OfficialBrushSolid.Poly(Vector3.down, new[]
            {
                new Vector3(0, 0, 0), new Vector3(10, 0, 0), new Vector3(10, 0, 10), new Vector3(0, 0, 10)
            }),
            new OfficialBrushSolid.Poly(Vector3.up, new[]
            {
                new Vector3(0, 10, 0), new Vector3(0, 10, 10), new Vector3(10, 10, 10), new Vector3(10, 10, 0)
            })
        };
        var min = new Vector3(0, 0, 0);
        var max = new Vector3(10, 10, 10);
        Assert.IsTrue(OfficialBrushSolid.Contains(new Vector3(5, 5, 5), min, max, polys));
        Assert.IsFalse(OfficialBrushSolid.Contains(new Vector3(15, 5, 5), min, max, polys));
        Assert.IsFalse(OfficialBrushSolid.Contains(new Vector3(-1, 5, 5), min, max, polys));
    }
}

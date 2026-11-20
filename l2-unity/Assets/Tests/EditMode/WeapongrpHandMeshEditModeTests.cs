using System.Linq;
using NUnit.Framework;

public sealed class WeapongrpHandMeshEditModeTests
{
    const string GreatPataMesh =
        "{{[LineageWeapons.great_pata_m00_wp];[LineageWeapons.great_pata_m00_wp]};{1;1}}";
    const string GreatPataTex =
        "{[LineageWeaponsTex.great_pata_t00_wp];[LineageWeaponsTex.great_pata_t00_wp]}";

    [Test]
    public void ParseClassicWpMesh_StoresRightAndLeftMeshKeys()
    {
        var meshes = WeapongrpHandMeshes.ParseClassicMeshList(GreatPataMesh);
        Assert.AreEqual(2, meshes.Count);
        Assert.AreEqual("LineageWeapons.great_pata_m00_wp", meshes[0]);
        Assert.AreEqual("LineageWeapons.great_pata_m00_wp", meshes[1]);
    }

    [Test]
    public void ApplyClassicMeshes_PopulatesModelLeft()
    {
        var grp = new Weapongrp();
        WeapongrpHandMeshes.ApplyClassicMeshes(grp, GreatPataMesh, GreatPataTex);
        Assert.AreEqual("LineageWeapons.great_pata_m00_wp", grp.Model);
        Assert.AreEqual("LineageWeapons.great_pata_m00_wp", grp.ModelLeft);
        Assert.AreEqual("LineageWeaponsTex.great_pata_t00_wp", grp.Texture);
        Assert.AreEqual("LineageWeaponsTex.great_pata_t00_wp", grp.TextureLeft);
    }

    [Test]
    public void ApplyInterludeMeshes_DualSwords_UseDistinctLeftMesh()
    {
        var grp = new Weapongrp();
        WeapongrpHandMeshes.ApplyInterludeMeshes(
            grp,
            "LineageWeapons.saber_m00_wp",
            "LineageWeapons.bastard_sword_m00_wp",
            "LineageWeaponsTex.saber_t00_wp",
            "LineageWeaponsTex.bastard_sword_t00_wp");

        Assert.AreEqual("LineageWeapons.saber_m00_wp", grp.Model);
        Assert.AreEqual("LineageWeapons.bastard_sword_m00_wp", grp.ModelLeft);
        Assert.IsFalse(WeapongrpHandMeshes.ShouldMirrorLeftHand(grp));
    }

    [Test]
    public void EnumerateModelKeys_IncludesDistinctLeftOnlyOnceEach()
    {
        var grp = new Weapongrp
        {
            Model = "LineageWeapons.a",
            ModelLeft = "LineageWeapons.b",
        };
        var keys = WeapongrpHandMeshes.EnumerateModelKeys(grp).ToList();
        Assert.AreEqual(2, keys.Count);
        CollectionAssert.Contains(keys, "LineageWeapons.a");
        CollectionAssert.Contains(keys, "LineageWeapons.b");
    }
}
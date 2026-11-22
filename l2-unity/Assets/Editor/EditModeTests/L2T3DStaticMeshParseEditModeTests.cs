#if UNITY_EDITOR
using System;
using System.IO;
using NUnit.Framework;

/// <summary>
/// The shared t3d static-mesh reader (all tiles) must exclude UE2 tombstone actors
/// (bDeleteMe/bPendingDelete — destroyed on map load) and must fail with a named
/// error when a live actor carries no StaticMesh= property. The lobby bake NRE'd
/// on exactly that tombstone shape (Lobby.unr StaticMeshActor487, #932 step 3).
/// </summary>
public sealed class L2T3DStaticMeshParseEditModeTests
{
    static string WriteTempT3d(string body)
    {
        string path = Path.Combine(Path.GetTempPath(), "fineage_t3d_" + Guid.NewGuid().ToString("N") + ".t3d");
        File.WriteAllText(path, body);
        return path;
    }

    [Test]
    public void Deleted_actors_are_excluded_live_ones_keep_their_name()
    {
        string path = WriteTempT3d(
            "Begin Map\n"
            + "Begin Actor Class=StaticMeshActor Name=StaticMeshActorAlive\n"
            + "    StaticMesh=StaticMesh'Pkg_S.Group.mesh01'\n"
            + "    Location=(X=1.000000,Y=2.000000,Z=3.000000)\n"
            + "End Actor\n"
            + "Begin Actor Class=StaticMeshActor Name=StaticMeshActorDeadFlag1\n"
            + "    StaticMesh=StaticMesh'Pkg_S.Group.dead01'\n"
            + "    bDeleteMe=True\n"
            + "End Actor\n"
            + "Begin Actor Class=StaticMeshActor Name=StaticMeshActorDeadFlag2\n"
            + "    bPendingDelete=True\n"
            + "End Actor\n"
            + "End Map\n");
        try
        {
            L2TerrainInfo info = L2T3DInfoParser.LoadStaticMeshInfo(path);
            Assert.NotNull(info);
            Assert.AreEqual(1, info.staticMeshes.Count);
            Assert.AreEqual("Pkg_S.Group.mesh01", info.staticMeshes[0].staticMesh);
            Assert.AreEqual("StaticMeshActorAlive", info.staticMeshes[0].actorName);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public void Live_actor_without_mesh_is_a_named_error_not_an_nre()
    {
        string path = WriteTempT3d(
            "Begin Map\n"
            + "Begin Actor Class=StaticMeshActor Name=StaticMeshActor487\n"
            + "    Location=(X=1.000000,Y=2.000000,Z=3.000000)\n"
            + "End Actor\n"
            + "End Map\n");
        try
        {
            InvalidDataException ex = Assert.Throws<InvalidDataException>(
                () => L2T3DInfoParser.LoadStaticMeshInfo(path));
            StringAssert.Contains("StaticMeshActor487", ex.Message);
            StringAssert.Contains("StaticMesh=", ex.Message);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public void GetFolderAndFileFromInfo_rejects_missing_or_shapeless_refs()
    {
        Assert.Throws<ArgumentException>(() => L2MetaDataUtils.GetFolderAndFileFromInfo(null));
        Assert.Throws<ArgumentException>(() => L2MetaDataUtils.GetFolderAndFileFromInfo(""));
        Assert.Throws<FormatException>(() => L2MetaDataUtils.GetFolderAndFileFromInfo("NoDotsHere"));
    }

    [Test]
    public void GetFolderAndFileFromInfo_splits_package_group_name()
    {
        Assert.AreEqual(new[] { "Pkg_S", "mesh01" }, L2MetaDataUtils.GetFolderAndFileFromInfo("Pkg_S.mesh01"));
        Assert.AreEqual(new[] { "Pkg_S", "mesh01" }, L2MetaDataUtils.GetFolderAndFileFromInfo("StaticMesh'Pkg_S.Group.mesh01'"));
        Assert.AreEqual(new[] { "Pkg_T", "tex01" }, L2MetaDataUtils.GetFolderAndFileFromInfo("Texture=Texture'Pkg_T.Group.tex01'"));
    }
}
#endif

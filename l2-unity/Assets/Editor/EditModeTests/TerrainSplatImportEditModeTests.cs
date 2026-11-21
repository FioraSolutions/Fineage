#if UNITY_EDITOR
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Splat/alphamap PNGs ship non-readable (import policy). Runtime play mode uses baked
/// terrain materials / Texture2DArray assets, not CPU GetPixels on these sources.
/// Editor rebuild tools must still sample them (via disk/blit helpers).
/// </summary>
public sealed class TerrainSplatImportEditModeTests
{
    const string SampleSplat = "Assets/Resources/Data/Textures/T_24_12/24_12_splat1.png";

    [Test]
    public void Splat_texture_import_is_not_cpu_readable()
    {
        var importer = AssetImporter.GetAtPath(SampleSplat) as TextureImporter;
        Assert.NotNull(importer, "Missing splat at " + SampleSplat);
        Assert.IsFalse(importer.isReadable, "Splat must stay non-readable in player builds");
    }

    [Test]
    public void Terrain_utils_can_flip_non_readable_splat_for_editor_pipeline()
    {
        Texture2D source = AssetDatabase.LoadAssetAtPath<Texture2D>(SampleSplat);
        Assert.NotNull(source);
        Assert.IsFalse(source.isReadable);

        Texture2D flipped = TextureUtils.FlipTextureVertically(source);
        Assert.NotNull(flipped);
        Assert.Greater(flipped.width, 0);
        Assert.Greater(flipped.height, 0);
        Assert.DoesNotThrow(() => flipped.GetPixel(0, 0));
    }

    [Test]
    public void Terrain_utils_load_from_info_works_when_importer_not_readable()
    {
        Texture2D loaded = TextureUtils.LoadTexture2DFromInfo("T_24_12.24_12_splat1", 256);
        Assert.NotNull(loaded);
        Assert.Greater(loaded.width, 0);
        Assert.DoesNotThrow(() => loaded.GetPixel(0, 0));
    }

    [Test]
    public void GetTexturePath_resolves_case_insensitive_png_on_disk()
    {
        string path = TextureUtils.GetTexturePath("Texture'T_23_13.sch_sn002'");
        Assert.That(path, Does.Contain("SCh_sn002.png").Or.Contain("sch_sn002.png"));
        Assert.IsTrue(System.IO.File.Exists(System.IO.Path.GetFullPath(
            UnityEngine.Application.dataPath + "/../" + path)), "Missing " + path);
    }
}
#endif
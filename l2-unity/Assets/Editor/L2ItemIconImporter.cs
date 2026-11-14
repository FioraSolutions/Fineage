#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Inventory icons are 32×32 UI art. DXT on that size is 4×4 blocks, and a point
/// filter breaks when the panel scales off 1280×720. Keep them uncompressed and bilinear.
/// </summary>
public sealed class L2ItemIconImporter : AssetPostprocessor
{
    const string IconFolder = "/SysTextures/Icon/";

    void OnPreprocessTexture()
    {
        string path = assetPath.Replace('\\', '/');
        if (path.IndexOf(IconFolder, System.StringComparison.Ordinal) < 0)
            return;

        var importer = (TextureImporter)assetImporter;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.crunchedCompression = false;
        importer.filterMode = FilterMode.Bilinear;
        importer.mipmapEnabled = false;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.wrapMode = TextureWrapMode.Clamp;

        var standalone = importer.GetPlatformTextureSettings("Standalone");
        standalone.overridden = true;
        standalone.format = TextureImporterFormat.RGBA32;
        standalone.textureCompression = TextureImporterCompression.Uncompressed;
        standalone.crunchedCompression = false;
        importer.SetPlatformTextureSettings(standalone);
    }
}
#endif

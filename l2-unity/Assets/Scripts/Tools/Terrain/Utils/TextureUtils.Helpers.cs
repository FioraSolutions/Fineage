#if (UNITY_EDITOR) 
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class TextureUtils
{
    public static Texture LoadTextureFromInfo(string info, int size) {
        Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(GetTexturePath(info));

        return texture;
    }

    public static string GetTexturePath(string value) {
        if (string.IsNullOrEmpty(value)) {
            return null;
        }
        string[] folderTexture = L2MetaDataUtils.GetFolderAndFileFromInfo(value);
        string folder = folderTexture[0];
        string fileStem = folderTexture[1];
        string canonical = Path.Combine("Assets", "Resources", "Data", "Textures", folder, fileStem + ".png")
            .Replace('\\', '/');
        string fullFolder = Path.GetFullPath(
            Path.Combine(Application.dataPath, "Resources", "Data", "Textures", folder));
        if (Directory.Exists(fullFolder)) {
            foreach (string candidate in Directory.GetFiles(fullFolder, "*.png")) {
                string name = Path.GetFileNameWithoutExtension(candidate);
                if (string.Equals(name, fileStem, StringComparison.OrdinalIgnoreCase)) {
                    return Path.Combine("Assets", "Resources", "Data", "Textures", folder, name + ".png")
                        .Replace('\\', '/');
                }
            }
        }
        return canonical;
    }

    public static string GetMaterialPath(string value) {
        if (string.IsNullOrEmpty(value)) {
            return null;
        }

        string[] folderTexture = L2MetaDataUtils.GetFolderAndFileFromInfo(value);
        return Path.Combine("Assets/Resources/Data/Textures", folderTexture[0], "Materials", folderTexture[1] + ".mat");
    }

    public static string GetHeightMapPath(string value) {
        if (string.IsNullOrEmpty(value)) {
            return null;
        }
        string[] folderTexture = L2MetaDataUtils.GetFolderAndFileFromInfo(value);
        return Path.Combine("Assets/Resources/Data/Textures", folderTexture[0], "Height." + folderTexture[1] + ".bmp");
    }
}
#endif

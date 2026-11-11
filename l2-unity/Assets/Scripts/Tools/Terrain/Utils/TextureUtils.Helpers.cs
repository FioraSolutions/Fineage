#if (UNITY_EDITOR) 
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
        return Path.Combine("Assets/Resources/Data/Textures", folderTexture[0], folderTexture[1] + ".png");
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

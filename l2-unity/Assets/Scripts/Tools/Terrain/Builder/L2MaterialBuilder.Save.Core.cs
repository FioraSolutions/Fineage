#if (UNITY_EDITOR) 
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class L2MaterialBuilder
{
    static Texture2D LoadTexture(string materialPath, string textureName) {
        string materialDirectory = Path.GetDirectoryName(materialPath);
        string parentFolder = Directory.GetParent(materialDirectory).FullName;
        string texturePath = Path.Combine(parentFolder, textureName + ".png");
        texturePath = Path.Combine("Assets", Path.GetRelativePath(Application.dataPath, texturePath));

        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        return texture;
    }

    static Material BuildTransprentMaterial() {
        Material material = new Material(Shader.Find("Universal Render Pipeline/Nature/SpeedTree8_PBRLit"));
        material.SetFloat("_AlphaClip", 0.5f);
        material.SetInt("_WindQuality", 0);
        material.SetInt("EFFECT_EXTRA_TEX", 0);
        material.SetInt("_NormalMapKwToggle", 0);
        material.SetInt("_HueVariationKwToggle", 0);
        material.SetFloat("_AlphaClipThreshold", 0.5f);
        material.SetFloat("_Glossiness", 0f);
        // Debug.Log($"_AlphaClip: {material.GetFloat("_AlphaClip")}");
        // Debug.Log($"_WindQuality: {material.GetInt("_WindQuality")}");
        // Debug.Log($"EFFECT_EXTRA_TEX: {material.GetInt("EFFECT_EXTRA_TEX")}");
        // Debug.Log($"_NormalMapKwToggle: {material.GetInt("_NormalMapKwToggle")}");
        // Debug.Log($"_HueVariationKwToggle: {material.GetInt("_HueVariationKwToggle")}");
        // Debug.Log($"_AlphaClipThreshold: {material.GetFloat("_AlphaClipThreshold")}");

        return material;
    }
}
#endif
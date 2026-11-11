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

    static Material BuildLitMaterial(Texture2D specularMap, bool isDoubleFace) {
        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        float state = isDoubleFace ? 0f : 2f;
        material.SetFloat("_Cull", state);
        material.SetColor("_BaseColor", Color.white);
        material.SetFloat("_Smoothness", 0);
        material.SetFloat("_EnvironmentReflections", 0f);
        material.SetFloat("_SpecularHighlights", 0f);

        if (specularMap != null) {
            material.EnableKeyword("_METALLICSPECGLOSSMAP");
            material.EnableKeyword("_SPECULAR_SETUP");
            material.SetFloat("_WorkflowMode", 0);
            material.SetFloat("_Smoothness", 1);
            material.SetTexture("_SpecGlossMap", specularMap);
            material.SetTexture("_METALLICSPECGLOSSMAP", specularMap);
            material.SetTexture("_Specular", specularMap);
        }

        return material;
    }

    static Material BuildUnlitMaterial(bool isDoubleFace) {
        Material material = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
        material.SetFloat("_Surface", 1f);
        material.SetFloat("_Blend", 2f);
        material.SetFloat("_BlendOp", 0f);
        material.SetFloat("_ColorMode", 0f);
        material.SetFloat("_DstBlend", 1f);
        material.SetFloat("_DstBlendAlpha", 1f);
        float state = isDoubleFace ? 0f : 2f;
        material.SetFloat("_Cull", state);
        //material.SetFloat("_AlphaClip", 1f);
        return material;
    }

    static void CreateBaseMaterials(bool overwrite) {
        string[] textureGUIDs = AssetDatabase.FindAssets("t:Texture2D", new string[] { "Assets/Resources/Data/Textures", "Assets/Resources/Data/SysTextures" });
        for(int i = 0; i < textureGUIDs.Length; i++) {
            string texturePath = AssetDatabase.GUIDToAssetPath(textureGUIDs[i]);
            string materialDirectory = Path.Combine(Path.GetDirectoryName(texturePath), "Materials");
            string materialPath = Path.Combine(materialDirectory, Path.GetFileNameWithoutExtension(texturePath) + ".mat");

            string ignorePath = Path.Combine(Path.GetDirectoryName(texturePath), ".ignore");
            if(File.Exists(ignorePath)) {
                // Debug.Log("Ignoring folder");
                continue;
            }

            if(materialPath.EndsWith("_ori.mat") || materialPath.EndsWith("_sp.mat")) {
                // Debug.Log("Skipping materials with props");
                continue;
            }

            if (!overwrite && File.Exists(materialPath)) {
                continue;
            }

            if(!Directory.Exists(materialDirectory)) {
                Directory.CreateDirectory(materialDirectory);
            }

            Material material;
            if (materialPath.EndsWith("_h.mat") || 
                materialPath.EndsWith("_ah.mat") || 
                materialPath.EndsWith("_bh.mat") || 
                materialPath.EndsWith("_ah_u00.mat") || 
                materialPath.EndsWith("_bh_u00.mat")) {
                material = BuildTransprentMaterial();
            } else {
                material = BuildLitMaterial(null, false);
            }

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            material.mainTexture = texture;
            AssetDatabase.CreateAsset(material, materialPath);
        }
    }
}
#endif
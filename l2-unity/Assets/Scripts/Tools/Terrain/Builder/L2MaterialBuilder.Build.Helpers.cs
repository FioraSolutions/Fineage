#if (UNITY_EDITOR) 
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class L2MaterialBuilder
{
    static void ClearMaterials() {
        string[] materialGUIDs = AssetDatabase.FindAssets("t:Material", new string[] { "Assets/Resources/Data/Textures", "Assets/Resources/Data/SysTextures" });
        for(int i = 0; i < materialGUIDs.Length; i++) {
            string materialPath = AssetDatabase.GUIDToAssetPath(materialGUIDs[i]);

            string ignorePath = Path.Combine(Path.GetDirectoryName(materialPath), ".ignore");
            if (File.Exists(ignorePath)) {
                // Debug.Log("Ignoring folder");
                continue;
            }

            AssetDatabase.DeleteAsset(materialPath);
        }
    }

    static void ProcessProps(bool overwrite) {
        string[] propsTxtGUIDs = AssetDatabase.FindAssets("t:TextAsset", new string[] { "Assets/Resources/Data/Textures", "Assets/Resources/Data/SysTextures" });
        //Debug.Log("Found " + propsTxtGUIDs.Length + " props.");

        for (int i = 0; i < propsTxtGUIDs.Length; i++) {
            string propsPath = AssetDatabase.GUIDToAssetPath(propsTxtGUIDs[i]);
            string materialPath = Path.Combine(
                   Path.GetDirectoryName(propsPath),
                   Path.GetFileNameWithoutExtension(propsPath)
                       .Replace(".props", string.Empty)
                       .Replace("_sh", string.Empty) + ".mat");

            if (File.Exists(materialPath)) {
                if (!overwrite) {
                    continue;
                }
            }

            bool isTransparent = false;
            bool isSpecular = false;
            bool isDoubleFace = false;
            bool isUnlit = false;
            string textureName = null;
            string specularTextureName = null;

            using (StreamReader reader = new StreamReader(propsPath)) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    string[] parts = line.Split("=");
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    if (key.StartsWith("Diffuse") || key.StartsWith("Material")) {
                        if (value.StartsWith("Texture")) {
                            string texRef = value.Substring(8);
                            texRef = texRef.Substring(0, texRef.Length - 1);
                            string[] texRefEntries = texRef.Split('.');
                            textureName = texRefEntries[texRefEntries.Length - 1];
                            // Debug.Log("Texture: " + textureName);
                        }
                    } else if (key.StartsWith("SpecularityMask")) {
                        if (value.StartsWith("Texture")) {
                            string texRef = value.Substring(8);
                            texRef = texRef.Substring(0, texRef.Length - 1);
                            string[] texRefEntries = texRef.Split('.');
                            specularTextureName = texRefEntries[texRefEntries.Length - 1];
                            isSpecular = true;
                            // Debug.Log("Specular texture: " + specularTextureName);
                        }
                    } else if (key.StartsWith("Opacity")) {
                        if (value.StartsWith("Texture")) {
                            // Debug.LogWarning("Transparent: " + textureName);
                            isTransparent = true;
                        }
                    } else if (key.StartsWith("TwoSided") || key == "TreatAsTwoSided") {
                        if (value == "true")
                            isDoubleFace = true;
                    } else if (key.StartsWith("AlphaTest")) {
                        if(!isTransparent) {
                            isTransparent = (value == "true");
                        }
                        // Debug.Log("AlphaTest:" + value);
                    } else if (key.StartsWith("OutputBlending")) {
                        if (L2Ue2MaterialChain.TryOutputBlending(value, out string blend)) {
                            if (blend == "OB_Brighten") {
                                isUnlit = true;
                            } else if (blend == "OB_Masked") {
                                isTransparent = true;
                            }
                        }
                    }
                }
            }

            string propsFull = Path.GetFullPath(Path.Combine(Application.dataPath, "..", propsPath));
            L2Ue2MaterialChain.Eval chain = L2Ue2MaterialChain.EvaluateAt(propsFull);
            if (chain.AlphaTest)
                isTransparent = false;
            if (chain.TwoSided)
                isDoubleFace = true;
            if (string.IsNullOrEmpty(textureName) && !string.IsNullOrEmpty(chain.DiffuseTexture)) {
                textureName = L2Ue2MaterialChain.ShortName(chain.DiffuseTexture);
            }
            if (string.IsNullOrEmpty(textureName)) {
                Debug.LogError(
                    "L2MaterialBuilder: material chain has no texture; " + propsPath
                    + (chain.Errors.Count == 0 ? "" : "\n" + string.Join("\n", chain.Errors)));
                continue;
            }

            Texture2D texture = LoadTexture(materialPath, textureName);
            if (texture == null) {
                Debug.LogError(
                    "L2MaterialBuilder: texture file missing package folder for "
                    + textureName + " referenced by " + propsPath);
                continue;
            }
            Material material;

            // AlphaTest / OB_Masked is cutout, not the SpeedTree premultiply path.
            if (chain.AlphaTest) {
                material = BuildLitMaterial(null, isDoubleFace, chain.Tint);
                if (material.HasProperty("_AlphaClip"))
                    material.SetFloat("_AlphaClip", 1f);
                float cutoff = chain.HasAlphaRef ? chain.AlphaRef / 255f : 0f;
                if (material.HasProperty("_Cutoff"))
                    material.SetFloat("_Cutoff", cutoff <= 0f ? 1f / 255f : cutoff);
                material.EnableKeyword("_ALPHATEST_ON");
                material.SetOverrideTag("RenderType", "TransparentCutout");
                material.SetFloat("_Cull", isDoubleFace ? 0f : 2f);
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
            } else if (isTransparent) {
                material = BuildTransprentMaterial();
            } else if (isUnlit) {
                material = BuildUnlitMaterial(isDoubleFace);
            } else {
                Texture2D specularMap = null;
                if (isSpecular) {
                    specularMap = LoadTexture(materialPath, specularTextureName);
                }
                material = BuildLitMaterial(specularMap, isDoubleFace, chain.Tint);
            }

            material.color = chain.Tint;
            if (material.HasProperty("_BaseColor")) {
                material.SetColor("_BaseColor", chain.Tint);
            }
            material.mainTexture = texture;

            if (File.Exists(materialPath)) {
                if (!isTransparent && !overwrite) {
                    continue;
                }
                // Debug.LogWarning("Delete " + texture);
                AssetDatabase.DeleteAsset(materialPath);
            }

            if (!Directory.Exists(Path.GetDirectoryName(materialPath))) {
                Directory.CreateDirectory(Path.GetDirectoryName(materialPath));
            }

            AssetDatabase.CreateAsset(material, materialPath);
        }
    }
}
#endif
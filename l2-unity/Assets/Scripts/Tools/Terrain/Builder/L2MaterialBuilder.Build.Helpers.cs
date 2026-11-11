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
                    } else if (key.StartsWith("TwoSided")) {
                        isDoubleFace = (value == "true");
                    } else if (key.StartsWith("AlphaTest")) {
                        if(!isTransparent) {
                            isTransparent = (value == "true");
                        }
                        // Debug.Log("AlphaTest:" + value);
                    } else if (key.StartsWith("OutputBlending")) {
                        // Debug.Log("OutputBlending:");
                        if (value.StartsWith("OB_Brighten")) {
                            isUnlit = true;
                        } else if (value.StartsWith("OB_Masked")) {
                            isTransparent = true;
                        }
                    }
                }
            }

            Texture2D texture = LoadTexture(materialPath, textureName);
            Material material;

            // Build Material
            if (isTransparent) {
                material = BuildTransprentMaterial();
            } else if (isUnlit) {
                material = BuildUnlitMaterial(isDoubleFace);
            } else {
                Texture2D specularMap = null;
                if (isSpecular) {
                    specularMap = LoadTexture(materialPath, specularTextureName);
                }
                material = BuildLitMaterial(specularMap, isDoubleFace);
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
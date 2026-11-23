#if (UNITY_EDITOR)
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class TerrainConverter
{
    private static Mesh SaveMesh(string saveFolder, GameObject destObject, bool safenet) {
        Mesh convertedTerrainMesh = destObject.GetComponent<MeshFilter>().sharedMesh;
        string filename = mapToConvert + "_Mesh.asset";
        if(safenet) {
            filename = "Safenet_Mesh.asset";
        }
        string saveMeshPath = Path.Combine(saveFolder, filename).Replace("\\", "/");
        if (AssetDatabase.LoadAssetAtPath<Mesh>(saveMeshPath) != null) {
            AssetDatabase.DeleteAsset(saveMeshPath);
        }
        MeshSaverEditor.SaveMeshToPath(convertedTerrainMesh, saveMeshPath, false, true);
        return AssetDatabase.LoadAssetAtPath<Mesh>(saveMeshPath);
    }

    private static Material GenerateMaterial(string saveFolder, L2TerrainInfo terrainInfo) {
        // Create material
        Material terrainMat = new Material(Shader.Find("Shader Graphs/TerrainAlphamapDouble"));

        // Generate alphamap
        Texture2D[] alphaMaps = new Texture2D[terrainInfo.uvLayers.Count - 1];
        Texture2D[] layers = new Texture2D[terrainInfo.uvLayers.Count - 1];

        for (int i = 0; i < terrainInfo.uvLayers.Count; i++) {
            L2TerrainLayer uvl = terrainInfo.uvLayers[i];
            if (i > 0) {
                alphaMaps[i - 1] = uvl.alphaMap;
                layers[i - 1] = uvl.texture;
                // Overlay without an official AlphaMap is logged at parse and stays off.
                bool paint = uvl.texture != null && uvl.alphaMap != null;
                terrainMat.SetFloat("_Layer_" + i + "_Enabled", paint ? 1.0f : 0.0f);
            } else if (uvl.texture == null) {
                // Shader Graphs/TerrainAlphamapDouble samples _Layer_0 as the opaque base.
                // An empty slot uses the graph default color, which is not the official diffuse.
                Debug.LogError(
                    "[TerrainUv] " + mapToConvert + " layer 0 diffuse missing ("
                    + uvl.diffuseTextureRef
                    + "). _Layer_0 is left empty; the graph default is not the official base.");
            } else {
                terrainMat.SetTexture("_Layer_0", uvl.texture);
            }

            if (i >= L2TerrainLayerUv.ShaderLayerCap) {
                Debug.LogError("[TerrainUv] layer " + i + " " + uvl.diffuseTextureRef + " exceeds shader property _Layer_" + i);
                continue;
            }
            var repeatErrors = new List<string>();
            if (!L2TerrainLayerUv.TryRepeat(
                    uvl.layerScale.x, uvl.layerScale.y, uvl.uScale, uvl.vScale, uvl.uPan, uvl.vPan,
                    uvl.diffuseTextureRef, repeatErrors, out Vector4 repeat)) {
                for (int e = 0; e < repeatErrors.Count; e++)
                    Debug.LogError(repeatErrors[e] + " on " + mapToConvert);
                continue;
            }
            terrainMat.SetVector("_Layer_" + i + "_UV", repeat);
            terrainMat.SetVector("_Layer_" + i + "_Xform", new Vector4(uvl.textureRotation, uvl.textureMapAxis, uvl.useAlpha ? 1f : 0f, 0f));
        }

        // Slice i is official overlay layer i+1. Do not cap the count. Trailing
        // overlays that have no AlphaMap (already LogError at parse) are omitted
        // from the arrays and stay disabled. A hole before a later valid overlay
        // is filled with a 1x1 clear placeholder so later slices keep their
        // official index; those hole layers stay disabled and are not a substitute mask.
        int lastPaintable = -1;
        for (int i = 0; i < alphaMaps.Length; i++) {
            if (alphaMaps[i] != null && layers[i] != null)
                lastPaintable = i;
        }
        if (lastPaintable >= 0) {
            Texture2D[] tightAlpha = new Texture2D[lastPaintable + 1];
            Texture2D[] tightLayers = new Texture2D[lastPaintable + 1];
            int layerW = 0, layerH = 0, maskW = 0, maskH = 0;
            for (int i = 0; i <= lastPaintable; i++) {
                if (layers[i] != null) {
                    layerW = layers[i].width;
                    layerH = layers[i].height;
                }
                if (alphaMaps[i] != null) {
                    maskW = alphaMaps[i].width;
                    maskH = alphaMaps[i].height;
                }
            }
            Texture2D holeLayer = null;
            Texture2D holeMask = null;
            for (int i = 0; i <= lastPaintable; i++) {
                tightAlpha[i] = alphaMaps[i];
                tightLayers[i] = layers[i];
                if (tightLayers[i] == null) {
                    if (holeLayer == null)
                        holeLayer = MakeIndexPlaceholderTexture(layerW, layerH);
                    tightLayers[i] = holeLayer;
                }
                if (tightAlpha[i] == null) {
                    if (holeMask == null)
                        holeMask = MakeIndexPlaceholderTexture(maskW, maskH);
                    tightAlpha[i] = holeMask;
                }
            }
            alphaMaps = tightAlpha;
            layers = tightLayers;
        } else {
            alphaMaps = new Texture2D[0];
            layers = new Texture2D[0];
        }

        // Build Texture2D array for textures
        if (layers.Length > 0) {
            texture2DArrayGenerator.sourceTextures = layers;
            string saveAlphamapPath = Path.Combine(saveFolder, mapToConvert + "_Layer.asset");
            Texture2DArray alphamaps = texture2DArrayGenerator.GenerateTexture2DArray(saveAlphamapPath);
            terrainMat.SetTexture("_Layers", alphamaps);
        }

        // Build Texture2D array for alphamaps.
        // Official 1K splat masks are grayscale (R=G=B). Stored as R8 without mips so
        // many official layers stay well under GitHub's 100MB file cap. Diffuse _Layers
        // arrays stay RGBA32 and mipmapped.
        if (alphaMaps.Length > 0) {
            bool prevMips = texture2DArrayGenerator.generateMipmaps;
            TextureFormat prevFmt = texture2DArrayGenerator.format;
            texture2DArrayGenerator.generateMipmaps = false;
            texture2DArrayGenerator.format = TextureFormat.R8;
            texture2DArrayGenerator.sourceTextures = alphaMaps;
            string saveAlphamapPath = Path.Combine(saveFolder, mapToConvert + "_Alphamap.asset");
            Texture2DArray alphamaps = texture2DArrayGenerator.GenerateTexture2DArray(saveAlphamapPath);
            texture2DArrayGenerator.generateMipmaps = prevMips;
            texture2DArrayGenerator.format = prevFmt;
            terrainMat.SetTexture("_Splatmaps", alphamaps);
        }

        terrainMat.SetVector("_Global_UV_Size", new Vector2(uvScale, uvScale));

        return terrainMat;
    }

    // 1x1 clear texel used only to keep Texture2DArray slice indices when an
    // official overlay in the middle of the stack is missing a texture or mask.
    // The corresponding _Layer_N_Enabled stays 0; this is not an official mask.
    static Texture2D MakeIndexPlaceholderTexture(int width, int height) {
        int w = Mathf.Max(1, width);
        int h = Mathf.Max(1, height);
        var t = new Texture2D(w, h, TextureFormat.RGBA32, false);
        t.name = "InterludeMissingOverlayPlaceholder";
        t.SetPixels(new Color[w * h]);
        t.Apply(false, true);
        return t;
    }
}
#endif

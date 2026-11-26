#if (UNITY_EDITOR) 
using System.IO;
using UnityEditor;
using UnityEngine;

public class Texture2DArrayGenerator : MonoBehaviour {
    public Texture2D[] sourceTextures; // Assign your source textures in the Inspector

    public bool generateMipmaps = true;
    public TextureFormat format = TextureFormat.RGBA32;

    // Start is called before the first frame update
    void Start() {
        if(sourceTextures == null || sourceTextures.Length == 0) {
            // Debug.LogWarning("No source textures assigned.");
            return;
        }

        GenerateTexture2DArray(Path.Combine("Assets", "array.asset"));
    }

    // CPU-side fill: Graphics.CopyTexture is GPU-only, and the following Apply() re-uploaded the
    // uninitialised CPU buffer (0xCD) -> saved array was garbage => white/flat ground.
    public Texture2DArray GenerateTexture2DArray(string path) {
        if (sourceTextures == null || sourceTextures.Length == 0) {
            Debug.LogError("[Interlude terrain] Texture2DArrayGenerator: no source textures for " + path);
            return null;
        }
        for (int i = 0; i < sourceTextures.Length; i++) {
            if (sourceTextures[i] == null) {
                Debug.LogError("[Interlude terrain] Texture2DArrayGenerator: sourceTextures[" + i + "] is null for " + path);
                return null;
            }
        }
        int width = sourceTextures[0].width;
        int height = sourceTextures[0].height;
        TextureFormat fmt = format;
        if (fmt == 0) fmt = TextureFormat.RGBA32;
        Texture2DArray textureArray = new Texture2DArray(width, height, sourceTextures.Length, fmt, generateMipmaps);

        for(int i = 0; i < sourceTextures.Length; i++) {
            Texture2D newTex = Resize(sourceTextures[i], width, height);
            if (fmt == TextureFormat.R8 || fmt == TextureFormat.Alpha8 || fmt == TextureFormat.BC4) {
                Color[] src = newTex.GetPixels(0);
                var g = new Color[src.Length];
                for (int p = 0; p < src.Length; p++) {
                    float r = src[p].r;
                    g[p] = new Color(r, r, r, r);
                }
                textureArray.SetPixels(g, i, 0);
            } else {
                textureArray.SetPixels32(newTex.GetPixels32(0), i, 0);
            }
            Object.DestroyImmediate(newTex);
        }

        textureArray.Apply(generateMipmaps, false);

        textureArray.filterMode = FilterMode.Bilinear;
        textureArray.wrapMode = TextureWrapMode.Repeat;
        textureArray.mipMapBias = 0;

        if (AssetDatabase.LoadAssetAtPath<Texture2DArray>(path) != null) AssetDatabase.DeleteAsset(path);
        AssetDatabase.CreateAsset(textureArray, path);
        AssetDatabase.SaveAssets();

        return textureArray;
    }

    Texture2D Resize(Texture2D texture2D, int targetX, int targetY) {
        RenderTexture rt = new RenderTexture(targetX, targetY, 24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D, rt);
        Texture2D result = new Texture2D(targetX, targetY, TextureFormat.RGBA32, false);
        result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
        result.Apply();
        RenderTexture.active = null;
        rt.Release();
        return result;
    }
}
#endif
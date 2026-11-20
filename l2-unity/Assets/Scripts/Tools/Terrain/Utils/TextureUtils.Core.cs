#if (UNITY_EDITOR) 
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class TextureUtils
{
    public static Texture2D FlipTextureVertically(Texture2D originalTexture) {
        originalTexture = DuplicateReadable(originalTexture);
        int width = originalTexture.width;
        int height = originalTexture.height;

        Texture2D flippedTexture = new Texture2D(width, height);
        Color[] originalPixels = originalTexture.GetPixels();
        Color[] flippedPixels = new Color[originalPixels.Length];

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                int sourceIndex = x + (height - y - 1) * width;
                int targetIndex = x + y * width;
                flippedPixels[targetIndex] = originalPixels[sourceIndex];
            }
        }

        flippedTexture.SetPixels(flippedPixels);
        flippedTexture.Apply();

        return flippedTexture;
    }

    public static Texture2D RotateTexture(Texture2D original) {
        original = DuplicateReadable(original);
        int width = original.width;
        int height = original.height;

        Color[] pixels = original.GetPixels();
        Color[] rotatedPixels = new Color[width * height];

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                // Calculate the new coordinates for the rotated pixel
                int newX = height - y - 1;
                int newY = x;

                // Calculate the indices in the 1D pixel arrays
                int originalIndex = y * width + x;
                int rotatedIndex = newY * height + newX;

                // Copy the pixel data
                rotatedPixels[rotatedIndex] = pixels[originalIndex];
            }
        }

        // Create a new Texture2D to store the rotated pixels
        Texture2D rotatedTexture = new Texture2D(height, width);
        rotatedTexture.SetPixels(rotatedPixels);
        rotatedTexture.Apply();

        return rotatedTexture;
    }

    public static Texture2D LoadTexture2DFromInfo(string info, int size) {
        string path = GetTexturePath(info);
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (texture != null && texture.isReadable)
            return texture;

        string fullPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", path));
        if (!File.Exists(fullPath))
            return texture;

        var loaded = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!loaded.LoadImage(File.ReadAllBytes(fullPath)))
            return texture;
        return loaded;
    }

    internal static Texture2D DuplicateReadable(Texture2D source) {
        if (source == null)
            return null;
        if (source.isReadable)
            return source;

        string path = AssetDatabase.GetAssetPath(source);
        if (!string.IsNullOrEmpty(path)) {
            string fullPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", path));
            if (File.Exists(fullPath)) {
                var loaded = new Texture2D(2, 2, TextureFormat.RGBA32, source.mipmapCount > 1, source.isDataSRGB);
                if (loaded.LoadImage(File.ReadAllBytes(fullPath)))
                    return loaded;
            }
        }

        var rt = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit(source, rt);
        var prev = RenderTexture.active;
        RenderTexture.active = rt;
        var copy = new Texture2D(source.width, source.height, TextureFormat.RGBA32, source.mipmapCount > 1, source.isDataSRGB);
        copy.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
        copy.Apply();
        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(rt);
        return copy;
    }
}
#endif

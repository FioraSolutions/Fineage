using System;
using UnityEngine;

public partial class L2StarDome : MonoBehaviour
{
    static void ConfigureTex(Texture2D tex)
    {
        tex.wrapModeU = TextureWrapMode.Repeat;
        tex.wrapModeV = TextureWrapMode.Repeat;
    }

    static Material MakeMat(Shader shader, Texture2D tex, Color tint, int queue)
    {
        var mat = new Material(shader);
        mat.hideFlags = HideFlags.DontSave;
        mat.SetTexture("_MainTex", tex);
        mat.SetVector(StarColorId, new Vector4(tint.r, tint.g, tint.b, tint.a));
        mat.renderQueue = queue;
        return mat;
    }

    float ResolveSkyDistance(Camera cam)
    {
        float far = cam != null ? cam.farClipPlane : 500f;
        return Mathf.Clamp(far * 0.88f, 80f, 4000f);
    }
}

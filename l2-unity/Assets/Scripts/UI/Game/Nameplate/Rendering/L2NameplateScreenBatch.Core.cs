using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

public sealed partial class L2NameplateScreenBatch : IDisposable
{
    public void EnsureMaterial(Texture atlas)
    {
        if (_material != null)
        {
            if (atlas != null)
            {
                _material.mainTexture = atlas;
                _material.SetTexture("_MainTex", atlas);
            }

            return;
        }

        Shader shader = Resources.Load<Shader>(ShaderResourcePath);
        if (shader == null)
        {
            shader = Shader.Find(ShaderName);
        }

        if (shader == null)
        {
            // Debug.LogError($"[L2NameplateScreenBatch] Shader '{ShaderName}' not found.");
            return;
        }

        _material = new Material(shader)
        {
            name = "L2ScreenBitmapFont (Shared)",
            hideFlags = HideFlags.HideAndDontSave,
            renderQueue = 5000
        };
        // Default: always on top (lobby / fallback). The compositor enables LEqual
        // when the scene depth target is bound.
        _material.SetFloat("_ZTest", (float)(int)CompareFunction.Always);
        if (atlas != null)
        {
            _material.mainTexture = atlas;
            _material.SetTexture("_MainTex", atlas);
        }
    }
}

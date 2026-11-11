using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public abstract partial class L2Window : MonoBehaviour
{
    protected void EnsureAssetsLoaded()
    {
        if (_windowTemplate != null)
            return;
        LoadAssets();
    }

    protected VisualTreeAsset LoadAsset(string assetPath)
    {
        VisualTreeAsset asset = Resources.Load<VisualTreeAsset>(assetPath);
        if (asset == null)
        {
            Debug.LogError($"[L2Window] Could not load UI template '{assetPath}' on {GetType().Name}.");
        }

        return asset;
    }
}

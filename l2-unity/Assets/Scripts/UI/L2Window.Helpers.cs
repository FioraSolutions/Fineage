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

    /// <summary>
    /// Horizontal resize without painted grip art. Shared by every L2 window that should
    /// stay resizable while the handle itself stays invisible (TargetWindow, and any
    /// later window that does not stamp Interlude chrome onto the grip).
    /// </summary>
    protected void AttachInvisibleHorizontalResize(VisualElement handle, float minWidth, float maxWidth)
    {
        if (handle == null || _windowEle == null)
            return;

        handle.style.backgroundImage = StyleKeyword.None;
        handle.style.backgroundColor = Color.clear;
        handle.AddManipulator(new HorizontalResizeManipulator(handle, _windowEle, minWidth, maxWidth));
    }
}
